using PayRollApi.Domain.Entities.Payroll;

namespace PayRollApi.Domain.Services.Payroll
{
    public static class AttendanceRuleResolver
    {
        // The rule where FromDays <= absentDays <= (ToDays ?? unbounded). No match => null (no adjustment).
        public static AttendanceRule? Resolve(IEnumerable<AttendanceRule> rules, int absentDays) =>
            rules.FirstOrDefault(r => r.FromDays <= absentDays && absentDays <= (r.ToDays ?? byte.MaxValue));
    }
}
