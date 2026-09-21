using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;

namespace PayRollApi.Application.Interfaces.Payroll
{
    public interface IAbsenceService
    {
        Task<ApiResponse<AbsenceMonthGridDto>> GetMonthGridAsync(int year, int month, int? departmentId);
        Task<ApiResponse<List<EmployeeAbsenceDto>>> GetEmployeeAbsencesAsync(int employeeId, int year, int month);
        Task<ApiResponse<bool>> ApplyBatchAsync(AbsenceBatchRequest request);
    }
}
