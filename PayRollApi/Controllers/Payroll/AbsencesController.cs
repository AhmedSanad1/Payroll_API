using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces.Payroll;

namespace PayRollApi.Controllers.Payroll
{
    [Authorize]
    [Route("api/absences")]
    [ApiController]
    public class AbsencesController(IAbsenceService service) : ControllerBase
    {
        [HttpGet("month")]
        public async Task<IActionResult> GetMonthGrid([FromQuery] int year, [FromQuery] int month, [FromQuery] int? departmentId)
        {
            var result = await service.GetMonthGridAsync(year, month, departmentId);
            return Respond(result);
        }

        [HttpPost("batch")]
        public async Task<IActionResult> ApplyBatch(AbsenceBatchRequest request)
        {
            var result = await service.ApplyBatchAsync(request);
            return Respond(result);
        }

        private IActionResult Respond<T>(ApiResponse<T> response) => StatusCode((int)response.StatusCode, response);
    }
}
