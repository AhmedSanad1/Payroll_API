using FluentAssertions;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces;
using PayRollApi.Application.Validators.Payroll;
using PayRollApi.Domain.Enums;

public class AttendanceRuleDtoValidatorTests
{
    private static readonly AttendanceRuleDtoValidator Validator = new(new StubLocalizer());

    [Fact]
    public void Valid_bounded_range_passes()
    {
        var dto = new AttendanceRuleDto { FromDays = 3, ToDays = 5, AdjustmentType = AttendanceAdjustmentType.Deduction, Percent = 5m };

        Validator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Unbounded_range_passes()
    {
        var dto = new AttendanceRuleDto { FromDays = 16, ToDays = null, AdjustmentType = AttendanceAdjustmentType.Deduction, Percent = 30m };

        Validator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Rejects_ToDays_less_than_FromDays()
    {
        var dto = new AttendanceRuleDto { FromDays = 10, ToDays = 5, AdjustmentType = AttendanceAdjustmentType.Deduction, Percent = 5m };

        Validator.Validate(dto).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Rejects_percent_above_100()
    {
        var dto = new AttendanceRuleDto { FromDays = 1, ToDays = 2, AdjustmentType = AttendanceAdjustmentType.Bonus, Percent = 150m };

        Validator.Validate(dto).IsValid.Should().BeFalse();
    }

    private class StubLocalizer : ILocalizer
    {
        public string Get(string key) => key;
    }
}
