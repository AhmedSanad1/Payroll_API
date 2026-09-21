using PayRollApi.Domain.Enums;

namespace PayRollApi.Domain.Services.Payroll
{
    public class PayrollCalculator
    {
        private readonly IPayrollCalculationStrategy additive = new AdditiveCalculationStrategy();
        private readonly IPayrollCalculationStrategy compound = new CompoundCalculationStrategy();

        public PayrollLineResult Calculate(PayrollCalculationMode mode, PayrollLineInput input)
        {
            var strategy = mode == PayrollCalculationMode.Compound ? compound : additive;
            return strategy.Calculate(input);
        }
    }
}
