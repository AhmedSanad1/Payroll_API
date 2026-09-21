using PayRollApi.Domain.Common;
using PayRollApi.Domain.Enums;

namespace PayRollApi.Domain.Entities.Payroll
{
    public class AttendanceRule : IAuditable
    {
        public int Id { get; set; }
        public byte FromDays { get; set; }
        public byte? ToDays { get; set; }
        public AttendanceAdjustmentType AdjustmentType { get; set; }
        public decimal Percent { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}
