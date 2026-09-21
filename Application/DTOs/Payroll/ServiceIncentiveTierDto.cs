using PayRollApi.Application.Common;

namespace PayRollApi.Application.DTOs.Payroll
{
    public class ServiceIncentiveTierDto : IHasId
    {
        public int Id { get; set; }
        public byte MinYearsExceeded { get; set; }
        public decimal Percent { get; set; }
    }
}
