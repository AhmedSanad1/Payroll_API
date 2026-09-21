using System.Net;
using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Helpers;
using PayRollApi.Application.Interfaces;
using PayRollApi.Application.Interfaces.Caching;
using PayRollApi.Application.Interfaces.Payroll;
using PayRollApi.Application.Logging;
using PayRollApi.Domain.Common;
using PayRollApi.Domain.Entities.Payroll;
using PayRollApi.Domain.Enums;
using PayRollApi.Domain.Services.Payroll;

namespace PayRollApi.Application.Services.Payroll
{
    internal class PayrollRunService(
        IPayrollRunRepository runRepository,
        IAbsenceRepository absenceRepository,
        ICRUDinterface<PayrollRun> runCrud,
        ICRUDinterface<ServiceIncentiveTier> tierCrud,
        ICRUDinterface<AttendanceRule> ruleCrud,
        ICRUDinterface<PayrollSettings> settingsCrud,
        PayrollCalculator calculator,
        ILocalizer localizer,
        IApplicationLogger logger,
        ICacheService cache) : IPayrollRunService
    {
        public async Task<ApiResponse<PageList<PayrollRunListItemDto>>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var variantKey = $"{pageNumber}:{pageSize}";
            var page = await cache.GetOrCreatePagedAsync<PayrollRun, PageList<PayrollRunListItemDto>>(
                variantKey, () => runRepository.GetPagedAsync(pageNumber, pageSize));
            return new SuccessResponse<PageList<PayrollRunListItemDto>>(localizer.Get("SuccessRetrieving"), page);
        }

        public async Task<ApiResponse<PayrollRunDetailDto>> GetByIdAsync(int id)
        {
            var detail = await runRepository.GetDetailAsync(id);
            if (detail is null)
                return new FailResponse<PayrollRunDetailDto>(localizer.Get("PayrollRunNotFound"), HttpStatusCode.NotFound);

            return new SuccessResponse<PayrollRunDetailDto>(localizer.Get("SuccessRetrieving"), detail);
        }

        public async Task<ApiResponse<PageList<PayrollItemDto>>> GetItemsAsync(int runId, int pageNumber, int pageSize, int? departmentId, string? search)
        {
            var run = await runCrud.GetById(runId);
            if (run is null)
                return new FailResponse<PageList<PayrollItemDto>>(localizer.Get("PayrollRunNotFound"), HttpStatusCode.NotFound);

            var page = await runRepository.GetItemsAsync(runId, pageNumber, pageSize, departmentId, search);
            return new SuccessResponse<PageList<PayrollItemDto>>(localizer.Get("SuccessRetrieving"), page);
        }

        public async Task<ApiResponse<PayrollItemDto>> GetPayslipAsync(int employeeId, int runId)
        {
            var payslip = await runRepository.GetPayslipAsync(employeeId, runId);
            if (payslip is null)
                return new FailResponse<PayrollItemDto>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound);

            return new SuccessResponse<PayrollItemDto>(localizer.Get("SuccessRetrieving"), payslip);
        }

        public async Task<ApiResponse<PayrollRunDetailDto>> GenerateAsync(int year, int month)
        {
            if (month is < 1 or > 12)
                return new FailResponse<PayrollRunDetailDto>(
                    string.Format(localizer.Get("Val_MinValue"), "Month", 1), HttpStatusCode.BadRequest);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (year > today.Year || (year == today.Year && month > today.Month))
                return new FailResponse<PayrollRunDetailDto>(localizer.Get("PayrollRunFutureMonth"), HttpStatusCode.BadRequest);

            var existingRuns = await runCrud.GetAll();
            var existingRun = existingRuns.FirstOrDefault(r => r.PeriodYear == year && r.PeriodMonth == month);

            // Approved runs are frozen — no regenerating.
            if (existingRun is not null && existingRun.Status == PayrollRunStatus.Approved)
                return new FailResponse<PayrollRunDetailDto>(localizer.Get("PayrollRunAlreadyApproved"), HttpStatusCode.Conflict);

            var periodStart = new DateOnly(year, month, 1);
            var periodEnd = new DateOnly(year, month, DateTime.DaysInMonth(year, month));

            var employees = await runRepository.GetEligibleEmployeesAsync(periodEnd);
            if (employees.Count == 0)
                return new FailResponse<PayrollRunDetailDto>(localizer.Get("PayrollRunNoActiveEmployees"), HttpStatusCode.BadRequest);

            var absentCounts = await absenceRepository.GetAbsentDayCountsAsync(periodStart, periodEnd);
            var tiers = await cache.GetOrCreateAsync(CacheKeys.RawEntityList<ServiceIncentiveTier>(), () => tierCrud.GetAll());
            var rules = await cache.GetOrCreateAsync(CacheKeys.RawEntityList<AttendanceRule>(), () => ruleCrud.GetAll());
            var settings = await settingsCrud.GetById(PayrollSettings.SingletonId);
            var mode = settings?.CalculationMode ?? PayrollCalculationMode.Additive;

            var items = employees.Select(e => BuildItem(e, periodEnd, mode, tiers, rules, absentCounts, year, month)).ToList();

            int runId;
            if (existingRun is not null)
            {
                // Regenerating a draft: wipe the old items and rebuild them.
                await runRepository.DeleteItemsForRunAsync(existingRun.Id);
                existingRun.CalculationMode = mode;
                existingRun.GeneratedAt = DateTime.UtcNow;
                await runCrud.Update(existingRun);
                runId = existingRun.Id;
            }
            else
            {
                var run = new PayrollRun
                {
                    PeriodYear = (short)year,
                    PeriodMonth = (byte)month,
                    Status = PayrollRunStatus.Draft,
                    CalculationMode = mode,
                    GeneratedAt = DateTime.UtcNow
                };
                await runCrud.Create(run);
                await runCrud.SaveChangesAsync();
                runId = run.Id;
            }

            foreach (var item in items)
                item.PayrollRunId = runId;

            runRepository.AddItems(items);
            await runRepository.SaveChangesAsync();
            await cache.InvalidatePagedAsync<PayrollRun>();

            var detail = await runRepository.GetDetailAsync(runId);
            return new SuccessResponse<PayrollRunDetailDto>(localizer.Get("Op_Created"), detail!);
        }

        public async Task<ApiResponse<PayrollRunDetailDto>> ApproveAsync(int id)
        {
            var run = await runCrud.GetById(id);
            if (run is null)
                return new FailResponse<PayrollRunDetailDto>(localizer.Get("PayrollRunNotFound"), HttpStatusCode.NotFound);

            if (run.Status == PayrollRunStatus.Approved)
                return new FailResponse<PayrollRunDetailDto>(localizer.Get("PayrollRunAlreadyApproved"), HttpStatusCode.Conflict);

            // Once approved, the run and its absence month are locked for good.
            run.Status = PayrollRunStatus.Approved;
            run.ApprovedAt = DateTime.UtcNow;
            await runCrud.Update(run);
            await runCrud.SaveChangesAsync();
            await cache.InvalidatePagedAsync<PayrollRun>();

            var detail = await runRepository.GetDetailAsync(id);
            return new SuccessResponse<PayrollRunDetailDto>(localizer.Get("Op_Updated"), detail!);
        }

        private PayrollItem BuildItem(
            PayrollEmployeeSnapshot e, DateOnly periodEnd, PayrollCalculationMode mode,
            ICollection<ServiceIncentiveTier> tiers, ICollection<AttendanceRule> rules, Dictionary<int, int> absentCounts,
            int year, int month)
        {
            var servicePercent = ServiceIncentiveTierResolver.ResolvePercent(tiers, e.HireDate, periodEnd);
            var serviceYears = DateCalculations.CompletedYearsBetween(e.HireDate, periodEnd);
            var absentDays = absentCounts.GetValueOrDefault(e.EmployeeId);
            var rule = AttendanceRuleResolver.Resolve(rules, absentDays);

            var input = new PayrollLineInput(e.BaseSalary, e.DeptIncentivePercent, servicePercent, rule?.AdjustmentType, rule?.Percent ?? 0m);
            var result = calculator.Calculate(mode, input);

            var rawNet = e.BaseSalary + result.DeptIncentiveAmount + result.ServiceIncentiveAmount + result.AttendanceAmount;
            // The calculator already clamped net to zero — this is just so it shows up in the logs.
            if (rawNet < 0)
                logger.LogWarning("Net salary clamped to zero for employee {0} in {1}-{2}", e.EmployeeId, year, month);

            return new PayrollItem
            {
                EmployeeId = e.EmployeeId,
                EmployeeName = e.FullName,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.DepartmentName,
                JobGradeId = e.JobGradeId,
                GradeName = e.GradeName,
                BaseSalary = e.BaseSalary,
                DeptIncentivePercent = e.DeptIncentivePercent,
                DeptIncentiveAmount = result.DeptIncentiveAmount,
                ServiceYears = (byte)Math.Clamp(serviceYears, 0, byte.MaxValue),
                ServiceIncentivePercent = servicePercent,
                ServiceIncentiveAmount = result.ServiceIncentiveAmount,
                AbsentDays = (byte)Math.Clamp(absentDays, 0, byte.MaxValue),
                AttendanceAdjustmentType = rule?.AdjustmentType,
                AttendancePercent = rule?.Percent ?? 0m,
                AttendanceAmount = result.AttendanceAmount,
                NetSalary = result.NetSalary
            };
        }
    }
}
