using Microsoft.EntityFrameworkCore;
using PayRollApi.Application.Interfaces.Payroll;
using PayRollApi.Domain.Entities.Payroll;
using PayRollApi.Infrastructure.Persistence.Contexts;

namespace PayRollApi.Infrastructure.Persistence.Repositories
{
    public class AbsenceRepository(PayRollDbContext context) : IAbsenceRepository
    {
        public async Task<List<EmployeeAbsence>> GetInRangeAsync(DateOnly start, DateOnly end, int? employeeId = null)
        {
            var query = context.EmployeeAbsences.Where(a => a.AbsenceDate >= start && a.AbsenceDate <= end);
            if (employeeId.HasValue)
                query = query.Where(a => a.EmployeeId == employeeId.Value);

            return await query.ToListAsync();
        }

        public async Task<Dictionary<int, int>> GetAbsentDayCountsAsync(DateOnly start, DateOnly end) =>
            await context.EmployeeAbsences
                .Where(a => a.AbsenceDate >= start && a.AbsenceDate <= end)
                .GroupBy(a => a.EmployeeId)
                .Select(g => new { EmployeeId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.EmployeeId, x => x.Count);

        public void AddRange(IEnumerable<EmployeeAbsence> absences) => context.EmployeeAbsences.AddRange(absences);

        public void RemoveRange(IEnumerable<EmployeeAbsence> absences) => context.EmployeeAbsences.RemoveRange(absences);

        public async Task SaveChangesAsync() => await context.SaveChangesAsync();
    }
}
