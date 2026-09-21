using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces.Payroll;

namespace PayRollApi.Controllers.Payroll
{
    [Authorize]
    [Route("api/employees")]
    [ApiController]
    public class EmployeesController(
        IEmployeeService service, IAbsenceService absenceService, IPayrollRunService payrollRunService) : ControllerBase
    {
        [HttpGet("{id:int}/absences")]
        public async Task<IActionResult> GetAbsences(int id, [FromQuery] int year, [FromQuery] int month)
        {
            var result = await absenceService.GetEmployeeAbsencesAsync(id, year, month);
            return Respond(result);
        }

        [HttpGet("{id:int}/payslip")]
        public async Task<IActionResult> GetPayslip(int id, [FromQuery] int runId)
        {
            var result = await payrollRunService.GetPayslipAsync(id, runId);
            return Respond(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50, [FromQuery] string? search = null,
            [FromQuery] int? departmentId = null, [FromQuery] int? jobGradeId = null,
            [FromQuery] string? sortBy = null, [FromQuery] bool sortDescending = false)
        {
            var result = await service.GetPagedAsync(pageNumber, pageSize, search, departmentId, jobGradeId, sortBy, sortDescending);
            return Respond(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await service.GetByIdAsync(id);
            return Respond(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeDto dto)
        {
            var result = await service.CreateAsync(dto);
            return Respond(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, EmployeeDto dto)
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
