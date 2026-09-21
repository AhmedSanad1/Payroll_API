using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces;
using PayRollApi.Application.Interfaces.Payroll;
using PayRollApi.Domain.Entities.Payroll;

namespace PayRollApi.Application.Services.Payroll
{
    internal class ReportService(
        IAbsenceRepository absenceRepository,
        ICRUDinterface<Employee> employeeCrud,
        ICRUDinterface<Department> departmentCrud,
        ICRUDinterface<JobGrade> jobGradeCrud,
        IPayrollRunRepository runRepository,
        ILocalizer localizer) : IReportService
    {
        public async Task<ApiResponse<List<AttendanceReportRowDto>>> GetAttendanceReportAsync(int year, int month, int? departmentId)
        {
            var (start, end) = MonthRange(year, month);
            var counts = await absenceRepository.GetAbsentDayCountsAsync(start, end);

            var employees = (await employeeCrud.GetAll())
                .Where(e => departmentId == null || e.DepartmentId == departmentId)
                .ToList();
            var departments = (await departmentCrud.GetAll()).ToDictionary(d => d.Id, d => d.Name);

            var rows = employees
                .Select(e => new AttendanceReportRowDto
                {
                    EmployeeId = e.Id,
                    FullName = e.FullName,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = departments.GetValueOrDefault(e.DepartmentId, string.Empty),
                    AbsentDays = counts.GetValueOrDefault(e.Id)
                })
                .OrderBy(r => r.FullName)
                .ToList();

            return new SuccessResponse<List<AttendanceReportRowDto>>(localizer.Get("SuccessRetrieving"), rows);
        }

        public async Task<ApiResponse<List<IncentiveDeductionReportRowDto>>> GetIncentivesDeductionsReportAsync(int runId, int? departmentId)
        {
            var items = await runRepository.GetAllItemsAsync(runId, departmentId);

            var rows = items
                .Select(i => new IncentiveDeductionReportRowDto
                {
                    EmployeeId = i.EmployeeId,
                    EmployeeName = i.EmployeeName,
                    DepartmentId = i.DepartmentId,
                    DepartmentName = i.DepartmentName,
                    DeptIncentiveAmount = i.DeptIncentiveAmount,
                    ServiceIncentiveAmount = i.ServiceIncentiveAmount,
                    AttendanceAmount = i.AttendanceAmount,
                    NetSalary = i.NetSalary
                })
                .ToList();

            return new SuccessResponse<List<IncentiveDeductionReportRowDto>>(localizer.Get("SuccessRetrieving"), rows);
        }

        public async Task<ApiResponse<List<EmployeeReportRowDto>>> GetEmployeesReportAsync(int? departmentId, int? jobGradeId)
        {
            var employees = await employeeCrud.GetAll();
            var departments = (await departmentCrud.GetAll()).ToDictionary(d => d.Id);
            var grades = (await jobGradeCrud.GetAll()).ToDictionary(g => g.Id);

            var rows = employees
                .Where(e => (departmentId == null || e.DepartmentId == departmentId) && (jobGradeId == null || e.JobGradeId == jobGradeId))
                .Select(e => new EmployeeReportRowDto
                {
                    EmployeeId = e.Id,
                    FullName = e.FullName,
                    Email = e.Email,
                    Phone = e.Phone,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = departments.TryGetValue(e.DepartmentId, out var d) ? d.Name : string.Empty,
                    JobGradeId = e.JobGradeId,
                    GradeName = grades.TryGetValue(e.JobGradeId, out var g) ? g.NameEn : string.Empty,
                    HireDate = e.HireDate
                })
                .OrderBy(r => r.FullName)
                .ToList();

            return new SuccessResponse<List<EmployeeReportRowDto>>(localizer.Get("SuccessRetrieving"), rows);
        }

        public async Task<ApiResponse<List<SalaryReportRowDto>>> GetSalariesReportAsync(int runId, int? departmentId)
        {
            var items = await runRepository.GetAllItemsAsync(runId, departmentId);

            var rows = items
                .Select(i => new SalaryReportRowDto
                {
                    EmployeeId = i.EmployeeId,
                    EmployeeName = i.EmployeeName,
                    DepartmentId = i.DepartmentId,
                    DepartmentName = i.DepartmentName,
                    BaseSalary = i.BaseSalary,
                    NetSalary = i.NetSalary
                })
                .ToList();

            return new SuccessResponse<List<SalaryReportRowDto>>(localizer.Get("SuccessRetrieving"), rows);
        }

        private static (DateOnly Start, DateOnly End) MonthRange(int year, int month)
        {
            var start = new DateOnly(year, month, 1);
            return (start, start.AddMonths(1).AddDays(-1));
        }
    }
}
