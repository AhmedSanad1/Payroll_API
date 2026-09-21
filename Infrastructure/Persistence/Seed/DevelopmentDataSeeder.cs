using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PayRollApi.Domain.Entities.Payroll;
using PayRollApi.Infrastructure.Persistence.Contexts;

namespace PayRollApi.Infrastructure.Persistence.Seed
{
    // Dev-only sample data, exercising every service-tier and attendance-rule range. Idempotent.
    public static class DevelopmentDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PayRollDbContext>();

            if (await context.Employees.AnyAsync())
                return;

            var departments = new List<Department>
            {
                new() { Name = "Engineering", IncentivePercent = 10m },
                new() { Name = "Finance", IncentivePercent = 5m },
                new() { Name = "Human Resources", IncentivePercent = 7m },
                new() { Name = "Sales", IncentivePercent = 12m }
            };
            context.Departments.AddRange(departments);
            await context.SaveChangesAsync();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var jobGradeIds = new[] { 1, 2, 3 };

            // Spread hire dates across <5, 5-7, and >7 years to exercise every service tier.
            var hireYearsAgoOptions = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 15 };

            var employees = new List<Employee>();
            for (var i = 0; i < 25; i++)
            {
                var department = departments[i % departments.Count];
                var hireYearsAgo = hireYearsAgoOptions[i % hireYearsAgoOptions.Length];
                var hireDate = today.AddYears(-hireYearsAgo).AddDays(i);

                employees.Add(new Employee
                {
                    FullName = $"Employee {i + 1:00}",
                    BirthDate = hireDate.AddYears(-25),
                    Address = $"{100 + i} Sample Street, Cairo",
                    Phone = $"010{i:0000000}",
                    Email = $"employee{i + 1:00}@payroll.local",
                    JobGradeId = jobGradeIds[i % jobGradeIds.Length],
                    DepartmentId = department.Id,
                    HireDate = hireDate
                });
            }
            context.Employees.AddRange(employees);
            await context.SaveChangesAsync();

            // Absences in the previous month, one employee per attendance-rule range: 0, 2, 4, 8, 12, 17 days.
            var previousMonth = today.AddMonths(-1);
            var firstOfPreviousMonth = new DateOnly(previousMonth.Year, previousMonth.Month, 1);
            var absenceCounts = new[] { 0, 2, 4, 8, 12, 17 };

            var absences = new List<EmployeeAbsence>();
            for (var i = 0; i < absenceCounts.Length; i++)
            {
                for (var d = 0; d < absenceCounts[i]; d++)
                {
                    absences.Add(new EmployeeAbsence
                    {
                        EmployeeId = employees[i].Id,
                        AbsenceDate = firstOfPreviousMonth.AddDays(d)
                    });
                }
            }
            context.EmployeeAbsences.AddRange(absences);
            await context.SaveChangesAsync();
        }
    }
}
