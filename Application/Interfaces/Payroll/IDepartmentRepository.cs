using PayRollApi.Application.Helpers;
using PayRollApi.Domain.Entities.Payroll;

namespace PayRollApi.Application.Interfaces.Payroll
{
    // ICRUDinterface<T> has no search/sort/paging composition — this fills that one gap.
    public interface IDepartmentRepository
    {
        Task<PageList<Department>> GetPagedAsync(int pageNumber, int pageSize, string? search, string? sortBy, bool sortDescending);
    }
}
