using PayRollApi.Application.Common;

namespace PayRollApi.Application.DTOs.Payroll
{
    public class EmployeeDto : IHasId
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public DateOnly BirthDate { get; set; }
        public required string Address { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
        public int JobGradeId { get; set; }
        public int DepartmentId { get; set; }
        public DateOnly HireDate { get; set; }
    }

    // Grid row shape — includes the joined names so the frontend doesn't need N+1 lookups.
    public class EmployeeListItemDto
    {
        public int Id { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public int DepartmentId { get; set; }
        public required string DepartmentName { get; set; }
        public int JobGradeId { get; set; }
        public required string GradeName { get; set; }
        public DateOnly HireDate { get; set; }
    }
}
