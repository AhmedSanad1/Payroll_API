using PayRollApi.Domain.Enums;

namespace PayRollApi.Application.DTOs.Payroll
{
    public class GeneratePayrollRunRequest
    {
        public int Year { get; set; }
        public int Month { get; set; }
    }

    public class PayrollRunListItemDto
    {
        public int Id { get; set; }
        public short PeriodYear { get; set; }
        public byte PeriodMonth { get; set; }
        public PayrollRunStatus Status { get; set; }
        public PayrollCalculationMode CalculationMode { get; set; }
        public DateTime GeneratedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int EmployeeCount { get; set; }
        public decimal TotalNetSalary { get; set; }
    }

    public class PayrollRunDetailDto
    {
        public int Id { get; set; }
        public short PeriodYear { get; set; }
        public byte PeriodMonth { get; set; }
        public PayrollRunStatus Status { get; set; }
        public PayrollCalculationMode CalculationMode { get; set; }
        public DateTime GeneratedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int EmployeeCount { get; set; }
        public decimal TotalBaseSalary { get; set; }
        public decimal TotalDeptIncentive { get; set; }
        public decimal TotalServiceIncentive { get; set; }
        public decimal TotalAttendanceAdjustment { get; set; }
        public decimal TotalNetSalary { get; set; }
    }

    public class PayrollItemDto
    {
        public long Id { get; set; }
        public int PayrollRunId { get; set; }
        public int EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public int DepartmentId { get; set; }
        public required string DepartmentName { get; set; }
        public int JobGradeId { get; set; }
        public required string GradeName { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal DeptIncentivePercent { get; set; }
        public decimal DeptIncentiveAmount { get; set; }
        public byte ServiceYears { get; set; }
        public decimal ServiceIncentivePercent { get; set; }
        public decimal ServiceIncentiveAmount { get; set; }
        public byte AbsentDays { get; set; }
        public AttendanceAdjustmentType? AttendanceAdjustmentType { get; set; }
        public decimal AttendancePercent { get; set; }
        public decimal AttendanceAmount { get; set; }
        public decimal NetSalary { get; set; }
    }

    // Projection used only while generating a run — not returned to the API as-is.
    public record PayrollEmployeeSnapshot(
        int EmployeeId,
        string FullName,
        int DepartmentId,
        string DepartmentName,
        decimal DeptIncentivePercent,
        int JobGradeId,
        string GradeName,
        decimal BaseSalary,
        DateOnly HireDate);
}
