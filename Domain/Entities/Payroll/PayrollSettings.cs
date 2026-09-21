using PayRollApi.Domain.Common;
using PayRollApi.Domain.Enums;

namespace PayRollApi.Domain.Entities.Payroll
{
    // Single row — Id is always 1.
    public class PayrollSettings : IAuditable
    {
        public const int SingletonId = 1;

        public int Id { get; set; } = SingletonId;
        public PayrollCalculationMode CalculationMode { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}
