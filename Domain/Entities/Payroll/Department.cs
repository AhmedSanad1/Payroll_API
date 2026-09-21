using PayRollApi.Domain.Common;

namespace PayRollApi.Domain.Entities.Payroll
{
    public class Department : IAuditable
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal IncentivePercent { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}
