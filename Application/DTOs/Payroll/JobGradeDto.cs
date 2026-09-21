using PayRollApi.Application.Common;

namespace PayRollApi.Application.DTOs.Payroll
{
    public class JobGradeDto : IHasId
    {
        public int Id { get; set; }
        public required string NameAr { get; set; }
        public required string NameEn { get; set; }
        public decimal BaseSalary { get; set; }
    }
}
