using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Helpers;

namespace PayRollApi.Application.Interfaces.Payroll
{
    public interface IDepartmentService
    {
        Task<ApiResponse<PageList<DepartmentDto>>> GetPagedAsync(int pageNumber, int pageSize, string? search, string? sortBy, bool sortDescending);
        Task<ApiResponse<List<DepartmentLookupDto>>> GetLookupAsync();
        Task<ApiResponse<DepartmentDto>> GetByIdAsync(int id);
        Task<ApiResponse<DepartmentDto>> CreateAsync(DepartmentDto dto);
        Task<ApiResponse<DepartmentDto>> UpdateAsync(int id, DepartmentDto dto);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}
