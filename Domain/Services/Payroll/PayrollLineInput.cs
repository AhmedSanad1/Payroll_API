using PayRollApi.Domain.Enums;

namespace PayRollApi.Domain.Services.Payroll
{
    public record PayrollLineInput(
        decimal BaseSalary,
        decimal DeptPercent,
        decimal ServicePercent,
        AttendanceAdjustmentType? AttendanceType,
        decimal AttendancePercent);
}
