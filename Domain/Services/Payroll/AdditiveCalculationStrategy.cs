using PayRollApi.Domain.Enums;

namespace PayRollApi.Domain.Services.Payroll
{
    // Every percentage applies to the base salary independently.
    public class AdditiveCalculationStrategy : IPayrollCalculationStrategy
    {
        public PayrollLineResult Calculate(PayrollLineInput input)
        {
            var dept = Round(input.BaseSalary * input.DeptPercent / 100m);
            var service = Round(input.BaseSalary * input.ServicePercent / 100m);
            var attendance = CalculateAttendance(input.BaseSalary, input.AttendanceType, input.AttendancePercent);

            // Never pay a negative salary.
            var net = Math.Max(input.BaseSalary + dept + service + attendance, 0m);

            return new PayrollLineResult(dept, service, attendance, net);
        }

        internal static decimal CalculateAttendance(decimal amountBase, AttendanceAdjustmentType? type, decimal percent)
        {
            if (type is null)
                return 0m;

            var raw = Round(amountBase * percent / 100m);
            return type == AttendanceAdjustmentType.Deduction ? -raw : raw;
        }

        // Money rounding — 0.005 goes up, not to the nearest even.
        internal static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
