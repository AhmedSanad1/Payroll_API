using FluentAssertions;
using PayRollApi.Domain.Enums;
using PayRollApi.Domain.Services.Payroll;

public class PayrollCalculatorTests
{
    private readonly PayrollCalculator calculator = new();

    [Fact]
    public void Reference_case_1_additive()
    {
        var input = new PayrollLineInput(10000m, 10m, 5m, AttendanceAdjustmentType.Deduction, 5m);

        calculator.Calculate(PayrollCalculationMode.Additive, input).NetSalary.Should().Be(11000.00m);
    }

    [Fact]
    public void Reference_case_1_compound()
    {
        var input = new PayrollLineInput(10000m, 10m, 5m, AttendanceAdjustmentType.Deduction, 5m);

        calculator.Calculate(PayrollCalculationMode.Compound, input).NetSalary.Should().Be(10972.50m);
    }

    [Fact]
    public void Reference_case_service_and_attendance_deduction_additive()
    {
        var input = new PayrollLineInput(8000m, 10m, 5m, AttendanceAdjustmentType.Deduction, 5m);

        calculator.Calculate(PayrollCalculationMode.Additive, input).NetSalary.Should().Be(8800.00m);
    }

    [Fact]
    public void Reference_case_bonus_only_additive()
    {
        var input = new PayrollLineInput(8000m, 0m, 0m, AttendanceAdjustmentType.Bonus, 5m);

        calculator.Calculate(PayrollCalculationMode.Additive, input).NetSalary.Should().Be(8400.00m);
    }

    [Fact]
    public void Reference_case_no_attendance_rule_matched_additive()
    {
        var input = new PayrollLineInput(8000m, 10m, 0m, null, 0m);

        calculator.Calculate(PayrollCalculationMode.Additive, input).NetSalary.Should().Be(8800.00m);
    }

    [Fact]
    public void Additive_rounds_midpoint_amounts_away_from_zero()
    {
        // 100 * 0.125% = 0.125 -> the 3rd decimal is exactly 5, must round the 2nd decimal up.
        var input = new PayrollLineInput(100m, 0.125m, 0m, null, 0m);

        calculator.Calculate(PayrollCalculationMode.Additive, input).DeptIncentiveAmount.Should().Be(0.13m);
    }

    [Fact]
    public void Compound_rounds_midpoint_amounts_away_from_zero()
    {
        var input = new PayrollLineInput(100m, 0m, 0.125m, null, 0m);

        calculator.Calculate(PayrollCalculationMode.Compound, input).ServiceIncentiveAmount.Should().Be(0.13m);
    }

    [Fact]
    public void Net_salary_never_goes_below_zero()
    {
        var input = new PayrollLineInput(100m, 0m, 0m, AttendanceAdjustmentType.Deduction, 500m);

        calculator.Calculate(PayrollCalculationMode.Additive, input).NetSalary.Should().Be(0m);
    }

    [Fact]
    public void No_attendance_rule_contributes_zero_regardless_of_percent()
    {
        var input = new PayrollLineInput(8000m, 0m, 0m, null, 999m);

        calculator.Calculate(PayrollCalculationMode.Additive, input).AttendanceAmount.Should().Be(0m);
    }
}
