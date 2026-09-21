using FluentAssertions;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Infrastructure.Services;

public class RdlcReportRendererTests
{
    private static readonly byte[] PdfMagic = "%PDF-"u8.ToArray();

    private static void ShouldBePdf(byte[] bytes)
    {
        bytes.Should().NotBeEmpty();
        bytes.Take(PdfMagic.Length).Should().Equal(PdfMagic);
    }

    [Fact]
    public void RenderPdf_renders_the_attendance_report()
    {
        var rows = new[] { new AttendanceReportRowDto { EmployeeId = 1, FullName = "أحمد سند", DepartmentId = 1, DepartmentName = "الموارد البشرية", AbsentDays = 2 } };

        var pdf = new RdlcReportRenderer().RenderPdf("AttendanceReport", "AttendanceDataSet", "تقرير الحضور", rows);

        ShouldBePdf(pdf);
    }

    [Fact]
    public void RenderPdf_renders_the_incentives_deductions_report()
    {
        var rows = new[] { new IncentiveDeductionReportRowDto { EmployeeId = 1, EmployeeName = "أحمد سند", DepartmentId = 1, DepartmentName = "المبيعات", DeptIncentiveAmount = 100m, ServiceIncentiveAmount = 50m, AttendanceAmount = 25m, NetSalary = 5175m } };

        var pdf = new RdlcReportRenderer().RenderPdf("IncentivesDeductionsReport", "IncentivesDeductionsDataSet", "تقرير الحوافز والخصومات", rows);

        ShouldBePdf(pdf);
    }

    [Fact]
    public void RenderPdf_renders_the_employees_report_including_a_DateOnly_hire_date()
    {
        var rows = new[] { new EmployeeReportRowDto { EmployeeId = 1, FullName = "أحمد سند", Email = "a@b.c", Phone = "0100", DepartmentId = 1, DepartmentName = "المبيعات", JobGradeId = 1, GradeName = "أولى", HireDate = new DateOnly(2025, 9, 17) } };

        var pdf = new RdlcReportRenderer().RenderPdf("EmployeesReport", "EmployeesDataSet", "تقرير الموظفين", rows);

        ShouldBePdf(pdf);
    }

    [Fact]
    public void RenderPdf_renders_the_salaries_report()
    {
        var rows = new[] { new SalaryReportRowDto { EmployeeId = 1, EmployeeName = "أحمد سند", DepartmentId = 1, DepartmentName = "المبيعات", BaseSalary = 5000m, NetSalary = 5175m } };

        var pdf = new RdlcReportRenderer().RenderPdf("SalariesReport", "SalariesDataSet", "تقرير الرواتب", rows);

        ShouldBePdf(pdf);
    }
}
