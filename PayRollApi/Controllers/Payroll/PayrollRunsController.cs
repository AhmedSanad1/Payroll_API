using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces.Payroll;

namespace PayRollApi.Controllers.Payroll
{
    [Authorize]
    [Route("api/payroll-runs")]
    [ApiController]
    public class PayrollRunsController(IPayrollRunService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            var result = await service.GetPagedAsync(pageNumber, pageSize);
            return Respond(result);
        }

        [HttpPost]
        public async Task<IActionResult> Generate(GeneratePayrollRunRequest request)
        {
            var result = await service.GenerateAsync(request.Year, request.Month);
            return Respond(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await service.GetByIdAsync(id);
            return Respond(result);
        }

        [HttpGet("{id:int}/items")]
        public async Task<IActionResult> GetItems(
            int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50,
            [FromQuery] int? departmentId = null, [FromQuery] string? search = null)
        {
            var result = await service.GetItemsAsync(id, pageNumber, pageSize, departmentId, search);
            return Respond(result);
        }

        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await service.ApproveAsync(id);
            return Respond(result);
        }

        private IActionResult Respond<T>(ApiResponse<T> response) => StatusCode((int)response.StatusCode, response);
    }
}
