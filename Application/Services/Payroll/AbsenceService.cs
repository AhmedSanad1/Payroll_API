using System.Net;
using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces;
using PayRollApi.Application.Interfaces.Payroll;
using PayRollApi.Domain.Entities.Payroll;
using PayRollApi.Domain.Enums;

namespace PayRollApi.Application.Services.Payroll
{
    internal class AbsenceService(
        IAbsenceRepository absenceRepository,
        ICRUDinterface<Employee> employeeCrud,
        ICRUDinterface<PayrollRun> payrollRunCrud,
        ILocalizer localizer) : IAbsenceService
    {
        public async Task<ApiResponse<AbsenceMonthGridDto>> GetMonthGridAsync(int year, int month, int? departmentId)
        {
            var (start, end) = MonthRange(year, month);

            var employees = (await employeeCrud.GetAll())
                .Where(e => departmentId == null || e.DepartmentId == departmentId)
                .OrderBy(e => e.FullName)
                .ToList();

            var absences = await absenceRepository.GetInRangeAsync(start, end);

            var rows = employees.Select(e => new AbsenceGridEmployeeRow
            {
                EmployeeId = e.Id,
                FullName = e.FullName,
                AbsenceDates = absences
                    .Where(a => a.EmployeeId == e.Id)
                    .Select(a => a.AbsenceDate)
                    .OrderBy(d => d)
                    .ToList()
            }).ToList();

            var isLocked = await IsMonthApprovedAsync(year, month);

            return new SuccessResponse<AbsenceMonthGridDto>(localizer.Get("SuccessRetrieving"),
                new AbsenceMonthGridDto { Employees = rows, IsLocked = isLocked });
        }

        public async Task<ApiResponse<List<EmployeeAbsenceDto>>> GetEmployeeAbsencesAsync(int employeeId, int year, int month)
        {
            var employee = await employeeCrud.GetById(employeeId);
            if (employee is null)
                return new FailResponse<List<EmployeeAbsenceDto>>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound);

            var (start, end) = MonthRange(year, month);
            var absences = await absenceRepository.GetInRangeAsync(start, end, employeeId);

            var dtos = absences
                .OrderBy(a => a.AbsenceDate)
                .Select(a => new EmployeeAbsenceDto { Id = a.Id, AbsenceDate = a.AbsenceDate, Notes = a.Notes })
                .ToList();

            return new SuccessResponse<List<EmployeeAbsenceDto>>(localizer.Get("SuccessRetrieving"), dtos);
        }

        public async Task<ApiResponse<bool>> ApplyBatchAsync(AbsenceBatchRequest request)
        {
            if (request.Add.Count == 0 && request.Remove.Count == 0)
                return new FailResponse<bool>(localizer.Get("Batch_NoOperations"), HttpStatusCode.BadRequest);

            var employees = (await employeeCrud.GetAll()).ToDictionary(e => e.Id);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var monthLockCache = new Dictionary<(int Year, int Month), bool>();

            async Task<bool> IsLockedAsync(DateOnly date)
            {
                var key = (date.Year, date.Month);
                if (!monthLockCache.TryGetValue(key, out var locked))
                {
                    locked = await IsMonthApprovedAsync(date.Year, date.Month);
                    monthLockCache[key] = locked;
                }
                return locked;
            }

            var allEntries = request.Add.Concat(request.Remove).ToList();

            foreach (var entry in allEntries)
            {
                if (!employees.TryGetValue(entry.EmployeeId, out var employee))
                    return new FailResponse<bool>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound);

                if (entry.Date > today)
                    return new FailResponse<bool>(localizer.Get("AbsenceDateInFuture"), HttpStatusCode.BadRequest);

                if (entry.Date < employee.HireDate)
                    return new FailResponse<bool>(localizer.Get("AbsenceBeforeHireDate"), HttpStatusCode.BadRequest);

                // Can't touch absences once that month's payroll is approved.
                if (await IsLockedAsync(entry.Date))
                    return new FailResponse<bool>(localizer.Get("PayrollRunApproved"), HttpStatusCode.Conflict);
            }

            var minDate = allEntries.Min(e => e.Date);
            var maxDate = allEntries.Max(e => e.Date);
            var existing = await absenceRepository.GetInRangeAsync(minDate, maxDate);

            var toAdd = request.Add
                .Where(entry => !existing.Any(a => a.EmployeeId == entry.EmployeeId && a.AbsenceDate == entry.Date))
                .Select(entry => new EmployeeAbsence { EmployeeId = entry.EmployeeId, AbsenceDate = entry.Date })
                .ToList();

            var toRemove = request.Remove
                .Select(entry => existing.FirstOrDefault(a => a.EmployeeId == entry.EmployeeId && a.AbsenceDate == entry.Date))
                .Where(a => a is not null)
                .Select(a => a!)
                .ToList();

            absenceRepository.AddRange(toAdd);
            absenceRepository.RemoveRange(toRemove);
            await absenceRepository.SaveChangesAsync();

            return new SuccessResponse<bool>(localizer.Get("Batch_Applied"));
        }

        private async Task<bool> IsMonthApprovedAsync(int year, int month)
        {
            var runs = await payrollRunCrud.GetAll();
            return runs.Any(r => r.PeriodYear == year && r.PeriodMonth == month && r.Status == PayrollRunStatus.Approved);
        }

        private static (DateOnly Start, DateOnly End) MonthRange(int year, int month)
        {
            var start = new DateOnly(year, month, 1);
            return (start, start.AddMonths(1).AddDays(-1));
        }
    }
}
