namespace PayRollApi.Application.DTOs.Payroll
{
    public class AttendanceReportRowDto
    {
        public int EmployeeId { get; set; }
        public required string FullName { get; set; }
        public int DepartmentId { get; set; }
        public required string DepartmentName { get; set; }
        public int AbsentDays { get; set; }
    }

    public class IncentiveDeductionReportRowDto
    {
        public int EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public int DepartmentId { get; set; }
        public required string DepartmentName { get; set; }
        public decimal DeptIncentiveAmount { get; set; }
        public decimal ServiceIncentiveAmount { get; set; }
        public decimal AttendanceAmount { get; set; }
        public decimal NetSalary { get; set; }
    }

    public class EmployeeReportRowDto
    {
        public int EmployeeId { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public int DepartmentId { get; set; }
        public required string DepartmentName { get; set; }
        public int JobGradeId { get; set; }
        public required string GradeName { get; set; }
        public DateOnly HireDate { get; set; }
    }

    public class SalaryReportRowDto
    {
        public int EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public int DepartmentId { get; set; }
        public required string DepartmentName { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal NetSalary { get; set; }
    }
}
