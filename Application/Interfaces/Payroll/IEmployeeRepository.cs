using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Helpers;

namespace PayRollApi.Application.Interfaces.Payroll
{
    public interface IEmployeeRepository
    {
        Task<PageList<EmployeeListItemDto>> GetPagedAsync(
            int pageNumber, int pageSize, string? search, int? departmentId, int? jobGradeId,
            string? sortBy, bool sortDescending);
    }
}
