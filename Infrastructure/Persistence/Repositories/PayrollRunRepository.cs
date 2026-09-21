using Microsoft.EntityFrameworkCore;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Helpers;
using PayRollApi.Application.Interfaces.Payroll;
using PayRollApi.Domain.Entities.Payroll;
using PayRollApi.Infrastructure.Persistence.Contexts;

namespace PayRollApi.Infrastructure.Persistence.Repositories
{
    public class PayrollRunRepository(PayRollDbContext context) : IPayrollRunRepository
    {
        public async Task<List<PayrollEmployeeSnapshot>> GetEligibleEmployeesAsync(DateOnly periodEnd) =>
            await (
                from e in context.Employees.AsNoTracking()
                join d in context.Departments.AsNoTracking() on e.DepartmentId equals d.Id
                join g in context.JobGrades.AsNoTracking() on e.JobGradeId equals g.Id
                where e.HireDate <= periodEnd
                select new PayrollEmployeeSnapshot(
                    e.Id, e.FullName, d.Id, d.Name, d.IncentivePercent, g.Id, g.NameEn, g.BaseSalary, e.HireDate)
            ).ToListAsync();

        public async Task<PageList<PayrollRunListItemDto>> GetPagedAsync(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = PageList.ClampPageSize(pageSize);

            var query =
                from r in context.PayrollRuns.AsNoTracking()
                orderby r.PeriodYear descending, r.PeriodMonth descending
                select new PayrollRunListItemDto
                {
                    Id = r.Id,
                    PeriodYear = r.PeriodYear,
                    PeriodMonth = r.PeriodMonth,
                    Status = r.Status,
                    CalculationMode = r.CalculationMode,
                    GeneratedAt = r.GeneratedAt,
                    ApprovedAt = r.ApprovedAt,
                    EmployeeCount = context.PayrollItems.Count(i => i.PayrollRunId == r.Id),
                    TotalNetSalary = context.PayrollItems.Where(i => i.PayrollRunId == r.Id).Sum(i => (decimal?)i.NetSalary) ?? 0m
                };

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PageList<PayrollRunListItemDto>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<PayrollRunDetailDto?> GetDetailAsync(int runId)
        {
            var run = await context.PayrollRuns.AsNoTracking().FirstOrDefaultAsync(r => r.Id == runId);
            if (run is null)
                return null;

            var items = context.PayrollItems.AsNoTracking().Where(i => i.PayrollRunId == runId);

            return new PayrollRunDetailDto
            {
                Id = run.Id,
                PeriodYear = run.PeriodYear,
                PeriodMonth = run.PeriodMonth,
                Status = run.Status,
                CalculationMode = run.CalculationMode,
                GeneratedAt = run.GeneratedAt,
                ApprovedAt = run.ApprovedAt,
                EmployeeCount = await items.CountAsync(),
                TotalBaseSalary = await items.SumAsync(i => (decimal?)i.BaseSalary) ?? 0m,
                TotalDeptIncentive = await items.SumAsync(i => (decimal?)i.DeptIncentiveAmount) ?? 0m,
                TotalServiceIncentive = await items.SumAsync(i => (decimal?)i.ServiceIncentiveAmount) ?? 0m,
                TotalAttendanceAdjustment = await items.SumAsync(i => (decimal?)i.AttendanceAmount) ?? 0m,
                TotalNetSalary = await items.SumAsync(i => (decimal?)i.NetSalary) ?? 0m
            };
        }

        public async Task<PageList<PayrollItemDto>> GetItemsAsync(int runId, int pageNumber, int pageSize, int? departmentId, string? search)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = PageList.ClampPageSize(pageSize);

            var query = context.PayrollItems.AsNoTracking().Where(i => i.PayrollRunId == runId);

            if (departmentId.HasValue)
                query = query.Where(i => i.DepartmentId == departmentId.Value);

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(i => i.EmployeeName.Contains(search));

            var totalCount = await query.CountAsync();

            // Materialize first — EF can't translate ToDto().
            var entities = await query
                .OrderBy(i => i.EmployeeName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageList<PayrollItemDto>(entities.Select(ToDto).ToList(), totalCount, pageNumber, pageSize);
        }

        public async Task<List<PayrollItemDto>> GetAllItemsAsync(int runId, int? departmentId)
        {
            var query = context.PayrollItems.AsNoTracking().Where(i => i.PayrollRunId == runId);
            if (departmentId.HasValue)
                query = query.Where(i => i.DepartmentId == departmentId.Value);

            var entities = await query.OrderBy(i => i.EmployeeName).ToListAsync();
            return entities.Select(ToDto).ToList();
        }

        public async Task<PayrollItemDto?> GetPayslipAsync(int employeeId, int runId)
        {
            var entity = await context.PayrollItems.AsNoTracking()
                .FirstOrDefaultAsync(i => i.EmployeeId == employeeId && i.PayrollRunId == runId);

            return entity is null ? null : ToDto(entity);
        }

        public async Task DeleteItemsForRunAsync(int runId)
        {
            var items = await context.PayrollItems.Where(i => i.PayrollRunId == runId).ToListAsync();
            context.PayrollItems.RemoveRange(items);
        }

        public void AddItems(IEnumerable<PayrollItem> items) => context.PayrollItems.AddRange(items);

        public async Task SaveChangesAsync() => await context.SaveChangesAsync();

        private static PayrollItemDto ToDto(PayrollItem i) => new()
        {
            Id = i.Id,
            PayrollRunId = i.PayrollRunId,
            EmployeeId = i.EmployeeId,
            EmployeeName = i.EmployeeName,
            DepartmentId = i.DepartmentId,
            DepartmentName = i.DepartmentName,
            JobGradeId = i.JobGradeId,
            GradeName = i.GradeName,
            BaseSalary = i.BaseSalary,
            DeptIncentivePercent = i.DeptIncentivePercent,
            DeptIncentiveAmount = i.DeptIncentiveAmount,
            ServiceYears = i.ServiceYears,
            ServiceIncentivePercent = i.ServiceIncentivePercent,
            ServiceIncentiveAmount = i.ServiceIncentiveAmount,
            AbsentDays = i.AbsentDays,
            AttendanceAdjustmentType = i.AttendanceAdjustmentType,
            AttendancePercent = i.AttendancePercent,
            AttendanceAmount = i.AttendanceAmount,
            NetSalary = i.NetSalary
        };
    }
}
