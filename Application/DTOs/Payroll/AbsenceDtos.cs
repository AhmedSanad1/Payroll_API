namespace PayRollApi.Application.DTOs.Payroll
{
    public class AbsenceMonthGridDto
    {
        public required List<AbsenceGridEmployeeRow> Employees { get; set; }
        public bool IsLocked { get; set; }
    }

    public class AbsenceGridEmployeeRow
    {
        public int EmployeeId { get; set; }
        public required string FullName { get; set; }
        public required List<DateOnly> AbsenceDates { get; set; }
    }

    public class AbsenceBatchRequest
    {
        public List<AbsenceEntry> Add { get; set; } = [];
        public List<AbsenceEntry> Remove { get; set; } = [];
    }

    public class AbsenceEntry
    {
        public int EmployeeId { get; set; }
        public DateOnly Date { get; set; }
    }

    public class EmployeeAbsenceDto
    {
        public long Id { get; set; }
        public DateOnly AbsenceDate { get; set; }
        public string? Notes { get; set; }
    }
}
