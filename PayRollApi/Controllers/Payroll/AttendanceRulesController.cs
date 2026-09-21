using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces;
using PayRollApi.Application.Interfaces.Caching;
using PayRollApi.Controllers.Common;
using PayRollApi.Domain.Entities.Payroll;

namespace PayRollApi.Controllers.Payroll
{
    [Route("api/attendance-rules")]
    public class AttendanceRulesController(
        ICRUDinterface<AttendanceRule> crud,
        IMapper mapper,
        ILocalizer localizer,
        ICacheService cache)
        : CrudControllerBase<AttendanceRule, AttendanceRuleDto>(crud, mapper, localizer, cache)
    {
        public override async Task<IActionResult> AddNew(AttendanceRuleDto dto)
        {
            if (await OverlapsAsync(dto, excludeId: null))
                return Respond(new FailResponse<AttendanceRuleDto>(localizer.Get("AttendanceRuleRangeOverlap"), HttpStatusCode.Conflict));

            return await base.AddNew(dto);
        }

        public override async Task<IActionResult> Update(int id, AttendanceRuleDto dto)
        {
            if (await OverlapsAsync(dto, excludeId: id))
                return Respond(new FailResponse<AttendanceRuleDto>(localizer.Get("AttendanceRuleRangeOverlap"), HttpStatusCode.Conflict));

            return await base.Update(id, dto);
        }

        // Interval overlap test; null ToDays means unbounded (byte.MaxValue).
        private async Task<bool> OverlapsAsync(AttendanceRuleDto dto, int? excludeId)
        {
            var rules = await crud.GetAll();
            var newFrom = dto.FromDays;
            var newTo = dto.ToDays ?? byte.MaxValue;

            return rules.Any(r =>
                r.Id != excludeId &&
                newFrom <= (r.ToDays ?? byte.MaxValue) &&
                r.FromDays <= newTo);
        }
    }
}
