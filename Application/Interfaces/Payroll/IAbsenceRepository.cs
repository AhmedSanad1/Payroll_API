using PayRollApi.Domain.Entities.Payroll;

namespace PayRollApi.Application.Interfaces.Payroll
{
    // EmployeeAbsence's bigint key can't go through ICRUDinterface<T> — bespoke repository instead.
    public interface IAbsenceRepository
    {
        Task<List<EmployeeAbsence>> GetInRangeAsync(DateOnly start, DateOnly end, int? employeeId = null);
        Task<Dictionary<int, int>> GetAbsentDayCountsAsync(DateOnly start, DateOnly end);
        void AddRange(IEnumerable<EmployeeAbsence> absences);
        void RemoveRange(IEnumerable<EmployeeAbsence> absences);
        Task SaveChangesAsync();
    }
}
