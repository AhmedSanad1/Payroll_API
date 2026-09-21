namespace PayRollApi.Domain.Services.Payroll
{
    // Fixed order dept -> service -> attendance, each applied to the running total.
    public class CompoundCalculationStrategy : IPayrollCalculationStrategy
    {
        public PayrollLineResult Calculate(PayrollLineInput input)
        {
            var r1 = input.BaseSalary;
            var dept = AdditiveCalculationStrategy.Round(r1 * input.DeptPercent / 100m);
            var r2 = r1 + dept;

            var service = AdditiveCalculationStrategy.Round(r2 * input.ServicePercent / 100m);
            var r3 = r2 + service;

            var attendance = AdditiveCalculationStrategy.CalculateAttendance(r3, input.AttendanceType, input.AttendancePercent);

            // Never pay a negative salary.
            var net = Math.Max(r3 + attendance, 0m);

            return new PayrollLineResult(dept, service, attendance, net);
        }
    }
}
