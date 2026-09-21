using PayRollApi.Domain.Common;

namespace PayRollApi.Domain.Entities.Payroll
{
    public class ServiceIncentiveTier : IAuditable
    {
        public int Id { get; set; }
        public byte MinYearsExceeded { get; set; }
        public decimal Percent { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}
