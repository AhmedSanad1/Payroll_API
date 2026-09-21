using Microsoft.EntityFrameworkCore;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Helpers;
using PayRollApi.Application.Interfaces.Payroll;
using PayRollApi.Infrastructure.Persistence.Contexts;

namespace PayRollApi.Infrastructure.Persistence.Repositories
{
    public class EmployeeRepository(PayRollDbContext context) : IEmployeeRepository
    {
        public async Task<PageList<EmployeeListItemDto>> GetPagedAsync(
            int pageNumber, int pageSize, string? search, int? departmentId, int? jobGradeId,
            string? sortBy, bool sortDescending)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = PageList.ClampPageSize(pageSize);

            var query =
                from e in context.Employees.AsNoTracking()
                join d in context.Departments.AsNoTracking() on e.DepartmentId equals d.Id
                join g in context.JobGrades.AsNoTracking() on e.JobGradeId equals g.Id
                select new EmployeeListItemDto
                {
                    Id = e.Id,
                    FullName = e.FullName,
                    Email = e.Email,
                    Phone = e.Phone,
                    DepartmentId = d.Id,
                    DepartmentName = d.Name,
                    JobGradeId = g.Id,
                    GradeName = g.NameEn,
                    HireDate = e.HireDate
                };

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.FullName.Contains(search) || x.Email.Contains(search));

            if (departmentId.HasValue)
                query = query.Where(x => x.DepartmentId == departmentId.Value);

            if (jobGradeId.HasValue)
                query = query.Where(x => x.JobGradeId == jobGradeId.Value);

            query = sortBy?.ToLowerInvariant() switch
            {
                "email" => sortDescending ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email),
                "hiredate" => sortDescending ? query.OrderByDescending(x => x.HireDate) : query.OrderBy(x => x.HireDate),
                "department" => sortDescending ? query.OrderByDescending(x => x.DepartmentName) : query.OrderBy(x => x.DepartmentName),
                "grade" => sortDescending ? query.OrderByDescending(x => x.GradeName) : query.OrderBy(x => x.GradeName),
                _ => sortDescending ? query.OrderByDescending(x => x.FullName) : query.OrderBy(x => x.FullName)
            };

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PageList<EmployeeListItemDto>(items, totalCount, pageNumber, pageSize);
        }
    }
}
