namespace PayRollApi.Domain.Common
{
    public static class DateCalculations
    {
        // Completed full years between two dates (anniversary must have passed).
        public static int CompletedYearsBetween(DateOnly from, DateOnly to)
        {
            var years = to.Year - from.Year;
            if (to < from.AddYears(years))
                years--;
            return years;
        }
    }
}
