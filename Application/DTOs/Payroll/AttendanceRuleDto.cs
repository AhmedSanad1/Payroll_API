using PayRollApi.Application.Common;
using PayRollApi.Domain.Enums;

namespace PayRollApi.Application.DTOs.Payroll
{
    public class AttendanceRuleDto : IHasId
    {
        public int Id { get; set; }
        public byte FromDays { get; set; }
        public byte? ToDays { get; set; }
        public AttendanceAdjustmentType AdjustmentType { get; set; }
        public decimal Percent { get; set; }
    }
}
