using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Helpers;
using PayRollApi.Domain.Entities.Payroll;

namespace PayRollApi.Application.Interfaces.Payroll
{
    // PayrollItem's bigint key can't go through ICRUDinterface<T> — bespoke repository instead.
    public interface IPayrollRunRepository
    {
        Task<List<PayrollEmployeeSnapshot>> GetEligibleEmployeesAsync(DateOnly periodEnd);
        Task<PageList<PayrollRunListItemDto>> GetPagedAsync(int pageNumber, int pageSize);
        Task<PayrollRunDetailDto?> GetDetailAsync(int runId);
        Task<PageList<PayrollItemDto>> GetItemsAsync(int runId, int pageNumber, int pageSize, int? departmentId, string? search);
        Task<List<PayrollItemDto>> GetAllItemsAsync(int runId, int? departmentId);
        Task<PayrollItemDto?> GetPayslipAsync(int employeeId, int runId);
        Task DeleteItemsForRunAsync(int runId);
        void AddItems(IEnumerable<PayrollItem> items);
        Task SaveChangesAsync();
    }
}
