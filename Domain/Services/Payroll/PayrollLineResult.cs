namespace PayRollApi.Domain.Services.Payroll
{
    public record PayrollLineResult(
        decimal DeptIncentiveAmount,
        decimal ServiceIncentiveAmount,
        decimal AttendanceAmount,
        decimal NetSalary);
}
