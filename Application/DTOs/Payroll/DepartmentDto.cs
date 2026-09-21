using PayRollApi.Application.Common;

namespace PayRollApi.Application.DTOs.Payroll
{
    public class DepartmentDto : IHasId
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal IncentivePercent { get; set; }
    }

    public class DepartmentLookupDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
