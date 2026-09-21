using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces.Payroll;

namespace PayRollApi.Controllers.Payroll
{
    [Authorize]
    [Route("api/departments")]
    [ApiController]
    public class DepartmentsController(IDepartmentService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null, [FromQuery] bool sortDescending = false)
        {
            var result = await service.GetPagedAsync(pageNumber, pageSize, search, sortBy, sortDescending);
            return Respond(result);
        }

        [HttpGet("lookup")]
        public async Task<IActionResult> Lookup()
        {
            var result = await service.GetLookupAsync();
            return Respond(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await service.GetByIdAsync(id);
            return Respond(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentDto dto)
        {
            var result = await service.CreateAsync(dto);
            return Respond(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, DepartmentDto dto)
        {
            var result = await service.UpdateAsync(id, dto);
            return Respond(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);
            return Respond(result);
        }

        private IActionResult Respond<T>(ApiResponse<T> response) => StatusCode((int)response.StatusCode, response);
    }
}
