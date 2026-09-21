using PayRollApi.Domain.Common;

namespace PayRollApi.Domain.Entities.Payroll
{
    // Fixed 3 rows (Id 1-3), seeded once — no create/delete, only BaseSalary and names are editable.
    public class JobGrade : IAuditable
    {
        public int Id { get; set; }
        public required string NameAr { get; set; }
        public required string NameEn { get; set; }
        public decimal BaseSalary { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}
