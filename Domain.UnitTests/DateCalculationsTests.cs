using FluentAssertions;
using PayRollApi.Domain.Common;

public class DateCalculationsTests
{
    [Fact]
    public void CompletedYearsBetween_returns_zero_for_same_date()
    {
        var date = new DateOnly(2020, 3, 10);

        DateCalculations.CompletedYearsBetween(date, date).Should().Be(0);
    }

    [Fact]
    public void CompletedYearsBetween_does_not_count_a_year_until_the_anniversary_passes()
    {
        var from = new DateOnly(2000, 3, 10);
        var dayBeforeAnniversary = new DateOnly(2018, 3, 9);

        DateCalculations.CompletedYearsBetween(from, dayBeforeAnniversary).Should().Be(17);
    }

    [Fact]
    public void CompletedYearsBetween_counts_a_year_exactly_on_the_anniversary()
    {
        var from = new DateOnly(2000, 3, 10);
        var anniversary = new DateOnly(2018, 3, 10);

        DateCalculations.CompletedYearsBetween(from, anniversary).Should().Be(18);
    }

    [Fact]
    public void CompletedYearsBetween_handles_a_leap_day_birth_date()
    {
        // DateOnly.AddYears rolls Feb 29 to Feb 28 in a non-leap target year, so that's
        // effectively the anniversary in 2019.
        var from = new DateOnly(2000, 2, 29);
        var dayBeforeRolledAnniversary = new DateOnly(2019, 2, 27);
        var onRolledAnniversary = new DateOnly(2019, 2, 28);

        DateCalculations.CompletedYearsBetween(from, dayBeforeRolledAnniversary).Should().Be(18);
        DateCalculations.CompletedYearsBetween(from, onRolledAnniversary).Should().Be(19);
    }
}
