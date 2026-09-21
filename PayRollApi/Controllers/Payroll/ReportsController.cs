using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayRollApi.Application.Common;
using PayRollApi.Application.Interfaces.Payroll;

namespace PayRollApi.Controllers.Payroll
{
    [Authorize]
    [Route("api/reports")]
    [ApiController]
    public class ReportsController(IReportService service, IRdlcReportRenderer renderer) : ControllerBase
    {
        [HttpGet("attendance")]
        public async Task<IActionResult> Attendance([FromQuery] int year, [FromQuery] int month, [FromQuery] int? departmentId)
        {
            var result = await service.GetAttendanceReportAsync(year, month, departmentId);
            return Respond(result);
        }

        [HttpGet("incentives-deductions")]
        public async Task<IActionResult> IncentivesDeductions([FromQuery] int runId, [FromQuery] int? departmentId)
        {
            var result = await service.GetIncentivesDeductionsReportAsync(runId, departmentId);
            return Respond(result);
        }

        [HttpGet("employees")]
        public async Task<IActionResult> Employees([FromQuery] int? departmentId, [FromQuery] int? jobGradeId)
        {
            var result = await service.GetEmployeesReportAsync(departmentId, jobGradeId);
            return Respond(result);
        }

        [HttpGet("salaries")]
        public async Task<IActionResult> Salaries([FromQuery] int runId, [FromQuery] int? departmentId)
        {
            var result = await service.GetSalariesReportAsync(runId, departmentId);
            return Respond(result);
        }

        [HttpGet("attendance/pdf")]
        public async Task<IActionResult> AttendancePdf([FromQuery] int year, [FromQuery] int month, [FromQuery] int? departmentId)
        {
            var result = await service.GetAttendanceReportAsync(year, month, departmentId);
            return RespondPdf(result, "AttendanceReport", "AttendanceDataSet", "تقرير الحضور", $"AttendanceReport_{year}-{month:D2}.pdf");
        }

        [HttpGet("incentives-deductions/pdf")]
        public async Task<IActionResult> IncentivesDeductionsPdf([FromQuery] int runId, [FromQuery] int? departmentId)
        {
            var result = await service.GetIncentivesDeductionsReportAsync(runId, departmentId);
            return RespondPdf(result, "IncentivesDeductionsReport", "IncentivesDeductionsDataSet", "تقرير الحوافز والخصومات", $"IncentivesDeductionsReport_{runId}.pdf");
        }

        [HttpGet("employees/pdf")]
        public async Task<IActionResult> EmployeesPdf([FromQuery] int? departmentId, [FromQuery] int? jobGradeId)
        {
            var result = await service.GetEmployeesReportAsync(departmentId, jobGradeId);
            return RespondPdf(result, "EmployeesReport", "EmployeesDataSet", "تقرير الموظفين", "EmployeesReport.pdf");
        }

        [HttpGet("salaries/pdf")]
        public async Task<IActionResult> SalariesPdf([FromQuery] int runId, [FromQuery] int? departmentId)
        {
            var result = await service.GetSalariesReportAsync(runId, departmentId);
            return RespondPdf(result, "SalariesReport", "SalariesDataSet", "تقرير الرواتب", $"SalariesReport_{runId}.pdf");
        }

        private IActionResult Respond<T>(ApiResponse<T> response) => StatusCode((int)response.StatusCode, response);

        private IActionResult RespondPdf<T>(ApiResponse<List<T>> response, string reportName, string dataSetName, string reportTitle, string fileName)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                return Respond(response);

            var pdf = renderer.RenderPdf(reportName, dataSetName, reportTitle, response.Object ?? []);
            return File(pdf, "application/pdf", fileName);
        }
    }
}
