using PayRollApi.Domain.Common;

namespace PayRollApi.Domain.Entities.Payroll
{
    public class EmployeeAbsence : IAuditable
    {
        public long Id { get; set; }
        public int EmployeeId { get; set; }
        public DateOnly AbsenceDate { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}
