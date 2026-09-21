using FluentAssertions;
using PayRollApi.Domain.Entities.Payroll;
using PayRollApi.Domain.Enums;
using PayRollApi.Domain.Services.Payroll;

public class AttendanceRuleResolverTests
{
    private static readonly List<AttendanceRule> Rules =
    [
        new AttendanceRule { Id = 1, FromDays = 0, ToDays = 0, AdjustmentType = AttendanceAdjustmentType.Bonus, Percent = 5m },
        new AttendanceRule { Id = 2, FromDays = 3, ToDays = 5, AdjustmentType = AttendanceAdjustmentType.Deduction, Percent = 5m },
        new AttendanceRule { Id = 3, FromDays = 6, ToDays = 10, AdjustmentType = AttendanceAdjustmentType.Deduction, Percent = 10m },
        new AttendanceRule { Id = 4, FromDays = 11, ToDays = 15, AdjustmentType = AttendanceAdjustmentType.Deduction, Percent = 20m },
        new AttendanceRule { Id = 5, FromDays = 16, ToDays = null, AdjustmentType = AttendanceAdjustmentType.Deduction, Percent = 30m }
    ];

    [Fact]
    public void Zero_days_matches_the_bonus_rule()
    {
        var rule = AttendanceRuleResolver.Resolve(Rules, 0);

        rule.Should().NotBeNull();
        rule!.Percent.Should().Be(5m);
        rule.AdjustmentType.Should().Be(AttendanceAdjustmentType.Bonus);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void One_and_two_days_match_no_rule(int days)
    {
        AttendanceRuleResolver.Resolve(Rules, days).Should().BeNull();
    }

    [Fact]
    public void Three_days_enters_the_first_deduction_bracket()
    {
        AttendanceRuleResolver.Resolve(Rules, 3)!.Percent.Should().Be(5m);
    }

    [Fact]
    public void Five_days_is_the_top_of_the_first_deduction_bracket()
    {
        AttendanceRuleResolver.Resolve(Rules, 5)!.Percent.Should().Be(5m);
    }

    [Fact]
    public void Six_days_enters_the_second_deduction_bracket()
    {
        AttendanceRuleResolver.Resolve(Rules, 6)!.Percent.Should().Be(10m);
    }

    [Fact]
    public void Fifteen_days_is_the_top_of_the_third_bracket()
    {
        AttendanceRuleResolver.Resolve(Rules, 15)!.Percent.Should().Be(20m);
    }

    [Fact]
    public void Sixteen_days_enters_the_unbounded_bracket()
    {
        AttendanceRuleResolver.Resolve(Rules, 16)!.Percent.Should().Be(30m);
    }

    [Fact]
    public void A_very_large_absence_count_still_matches_the_unbounded_bracket()
    {
        AttendanceRuleResolver.Resolve(Rules, 200)!.Percent.Should().Be(30m);
    }
}
