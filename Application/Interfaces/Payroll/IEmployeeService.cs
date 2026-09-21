using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Helpers;

namespace PayRollApi.Application.Interfaces.Payroll
{
    public interface IEmployeeService
    {
        Task<ApiResponse<PageList<EmployeeListItemDto>>> GetPagedAsync(
            int pageNumber, int pageSize, string? search, int? departmentId, int? jobGradeId,
            string? sortBy, bool sortDescending);
        Task<ApiResponse<EmployeeDto>> GetByIdAsync(int id);
        Task<ApiResponse<EmployeeDto>> CreateAsync(EmployeeDto dto);
        Task<ApiResponse<EmployeeDto>> UpdateAsync(int id, EmployeeDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
