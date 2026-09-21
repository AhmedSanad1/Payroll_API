namespace PayRollApi.Domain.Services.Payroll
{
    public interface IPayrollCalculationStrategy
    {
        PayrollLineResult Calculate(PayrollLineInput input);
    }
}
