using PayRollApi.Domain.Entities.Payroll;

namespace PayRollApi.Domain.Services.Payroll
{
    public static class ServiceIncentiveTierResolver
    {
        // Highest matching tier wins — tiers never stack. No match => 0%.
        public static decimal ResolvePercent(IEnumerable<ServiceIncentiveTier> tiers, DateOnly hireDate, DateOnly periodEndDate) =>
            tiers
                .Where(t => periodEndDate > hireDate.AddYears(t.MinYearsExceeded))
                .OrderByDescending(t => t.MinYearsExceeded)
                .Select(t => t.Percent)
                .FirstOrDefault();
    }
}
