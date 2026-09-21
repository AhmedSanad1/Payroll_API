using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Helpers;

namespace PayRollApi.Application.Interfaces.Payroll
{
    public interface IPayrollRunService
    {
        Task<ApiResponse<PageList<PayrollRunListItemDto>>> GetPagedAsync(int pageNumber, int pageSize);
        Task<ApiResponse<PayrollRunDetailDto>> GenerateAsync(int year, int month);
        Task<ApiResponse<PayrollRunDetailDto>> GetByIdAsync(int id);
        Task<ApiResponse<PageList<PayrollItemDto>>> GetItemsAsync(int runId, int pageNumber, int pageSize, int? departmentId, string? search);
        Task<ApiResponse<PayrollRunDetailDto>> ApproveAsync(int id);
        Task<ApiResponse<PayrollItemDto>> GetPayslipAsync(int employeeId, int runId);
    }
}
