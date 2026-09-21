using PayRollApi.Domain.Common;

namespace PayRollApi.Domain.Entities.Payroll
{
    public class Employee : IAuditable
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
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}
