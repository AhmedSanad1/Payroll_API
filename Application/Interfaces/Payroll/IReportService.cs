using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;

namespace PayRollApi.Application.Interfaces.Payroll
{
    public interface IReportService
    {
        Task<ApiResponse<List<AttendanceReportRowDto>>> GetAttendanceReportAsync(int year, int month, int? departmentId);
        Task<ApiResponse<List<IncentiveDeductionReportRowDto>>> GetIncentivesDeductionsReportAsync(int runId, int? departmentId);
        Task<ApiResponse<List<EmployeeReportRowDto>>> GetEmployeesReportAsync(int? departmentId, int? jobGradeId);
        Task<ApiResponse<List<SalaryReportRowDto>>> GetSalariesReportAsync(int runId, int? departmentId);
    }
}
