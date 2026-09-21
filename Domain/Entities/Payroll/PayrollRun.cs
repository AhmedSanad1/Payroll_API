using PayRollApi.Domain.Common;
using PayRollApi.Domain.Enums;

namespace PayRollApi.Domain.Entities.Payroll
{
    public class PayrollRun : IAuditable
    {
        public int Id { get; set; }
        public short PeriodYear { get; set; }
        public byte PeriodMonth { get; set; }
        public PayrollRunStatus Status { get; set; }
        public PayrollCalculationMode CalculationMode { get; set; }
        public DateTime GeneratedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}
