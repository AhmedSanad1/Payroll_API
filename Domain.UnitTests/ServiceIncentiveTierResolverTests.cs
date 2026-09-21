using FluentAssertions;
using PayRollApi.Domain.Entities.Payroll;
using PayRollApi.Domain.Services.Payroll;

public class ServiceIncentiveTierResolverTests
{
    private static readonly List<ServiceIncentiveTier> Tiers =
    [
        new ServiceIncentiveTier { Id = 1, MinYearsExceeded = 5, Percent = 5m },
        new ServiceIncentiveTier { Id = 2, MinYearsExceeded = 7, Percent = 10m }
    ];

    [Fact]
    public void No_tier_applies_before_five_years()
    {
        var hireDate = new DateOnly(2020, 1, 1);
        var periodEnd = new DateOnly(2024, 6, 1);

        ServiceIncentiveTierResolver.ResolvePercent(Tiers, hireDate, periodEnd).Should().Be(0m);
    }

    [Fact]
    public void Exactly_five_years_does_not_yet_qualify()
    {
        var hireDate = new DateOnly(2015, 1, 1);
        var periodEnd = hireDate.AddYears(5);

        ServiceIncentiveTierResolver.ResolvePercent(Tiers, hireDate, periodEnd).Should().Be(0m);
    }

    [Fact]
    public void One_day_past_five_years_qualifies_for_tier_one()
    {
        var hireDate = new DateOnly(2015, 1, 1);
        var periodEnd = hireDate.AddYears(5).AddDays(1);

        ServiceIncentiveTierResolver.ResolvePercent(Tiers, hireDate, periodEnd).Should().Be(5m);
    }

    [Fact]
    public void Eight_years_uses_only_the_highest_matching_tier_not_stacked()
    {
        var hireDate = new DateOnly(2012, 1, 1);
        var periodEnd = hireDate.AddYears(8);

        ServiceIncentiveTierResolver.ResolvePercent(Tiers, hireDate, periodEnd).Should().Be(10m);
    }

    [Fact]
    public void One_day_past_seven_years_qualifies_for_tier_two()
    {
        var hireDate = new DateOnly(2010, 1, 1);
        var periodEnd = hireDate.AddYears(7).AddDays(1);

        ServiceIncentiveTierResolver.ResolvePercent(Tiers, hireDate, periodEnd).Should().Be(10m);
    }
}
