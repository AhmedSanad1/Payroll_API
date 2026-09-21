using PayRollApi.Domain.Enums;

namespace PayRollApi.Domain.Entities.Payroll
{
    // Write-once snapshot row — a regenerate deletes and recreates, never updates in place.
    public class PayrollItem
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
}
