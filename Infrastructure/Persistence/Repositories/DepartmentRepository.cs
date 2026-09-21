using Microsoft.EntityFrameworkCore;
using PayRollApi.Application.Helpers;
using PayRollApi.Application.Interfaces.Payroll;
using PayRollApi.Domain.Entities.Payroll;
using PayRollApi.Infrastructure.Persistence.Contexts;

namespace PayRollApi.Infrastructure.Persistence.Repositories
{
    public class DepartmentRepository(PayRollDbContext context) : IDepartmentRepository
    {
        public async Task<PageList<Department>> GetPagedAsync(int pageNumber, int pageSize, string? search, string? sortBy, bool sortDescending)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = PageList.ClampPageSize(pageSize);

            var query = context.Departments.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(d => d.Name.Contains(search));

            query = sortBy?.ToLowerInvariant() switch
            {
                "incentivepercent" => sortDescending
                    ? query.OrderByDescending(d => d.IncentivePercent)
                    : query.OrderBy(d => d.IncentivePercent),
                _ => sortDescending ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name)
            };

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PageList<Department>(items, totalCount, pageNumber, pageSize);
        }
    }
}
