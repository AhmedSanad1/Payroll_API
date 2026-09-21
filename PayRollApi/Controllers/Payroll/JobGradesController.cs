using System.Net;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces;
using PayRollApi.Application.Interfaces.Caching;
using PayRollApi.Domain.Entities.Payroll;

namespace PayRollApi.Controllers.Payroll
{
    // No create/delete: exactly 3 fixed rows (seeded), only names and BaseSalary are editable.
    [Authorize]
    [Route("api/job-grades")]
    [ApiController]
    public class JobGradesController(
        ICRUDinterface<JobGrade> crud,
        IMapper mapper,
        ILocalizer localizer,
        IValidator<JobGradeDto> validator,
        ICacheService cache) : ControllerBase
    {
        private static string CacheKey => CacheKeys.EntityList<JobGrade>();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dtos = await cache.GetOrCreateAsync(CacheKey, async () =>
            {
                var grades = await crud.GetAll();
                return mapper.Map<ICollection<JobGradeDto>>(grades.OrderBy(g => g.Id).ToList());
            });
            return Ok(new SuccessResponse<ICollection<JobGradeDto>>(localizer.Get("SuccessRetrieving"), dtos));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, JobGradeDto dto)
        {
            if (id != dto.Id)
                return Respond(new FailResponse<JobGradeDto>(localizer.Get("IdMismatch"), HttpStatusCode.BadRequest));

            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return Respond(validation.ToFailResponse<JobGradeDto>());

            var entity = await crud.GetById(id);
            if (entity is null)
                return Respond(new FailResponse<JobGradeDto>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound));

            mapper.Map(dto, entity);
            await crud.Update(entity);
            await crud.SaveChangesAsync();
            await cache.RemoveAsync(CacheKey);
            // NameEn/BaseSalary are denormalized into the cached employee list DTO.
            await cache.InvalidatePagedAsync<Employee>();
            return Ok(new SuccessResponse<JobGradeDto>(localizer.Get("SuccessUpdating"), mapper.Map<JobGradeDto>(entity)));
        }

        private IActionResult Respond<T>(ApiResponse<T> response) => StatusCode((int)response.StatusCode, response);
    }
}
