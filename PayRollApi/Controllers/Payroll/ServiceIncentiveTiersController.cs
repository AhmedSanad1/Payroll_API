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
    [Route("api/service-incentive-tiers")]
    public class ServiceIncentiveTiersController(
        ICRUDinterface<ServiceIncentiveTier> crud,
        IMapper mapper,
        ILocalizer localizer,
        ICacheService cache)
        : CrudControllerBase<ServiceIncentiveTier, ServiceIncentiveTierDto>(crud, mapper, localizer, cache)
    {
        public override async Task<IActionResult> AddNew(ServiceIncentiveTierDto dto)
        {
            if (await YearsExistAsync(dto.MinYearsExceeded, excludeId: null))
                return Respond(new FailResponse<ServiceIncentiveTierDto>(localizer.Get("ServiceTierDuplicateYears"), HttpStatusCode.Conflict));

            return await base.AddNew(dto);
        }

        public override async Task<IActionResult> Update(int id, ServiceIncentiveTierDto dto)
        {
            if (await YearsExistAsync(dto.MinYearsExceeded, excludeId: id))
                return Respond(new FailResponse<ServiceIncentiveTierDto>(localizer.Get("ServiceTierDuplicateYears"), HttpStatusCode.Conflict));

            return await base.Update(id, dto);
        }

        private async Task<bool> YearsExistAsync(byte minYearsExceeded, int? excludeId)
        {
            var tiers = await crud.GetAll();
            return tiers.Any(t => t.Id != excludeId && t.MinYearsExceeded == minYearsExceeded);
        }
    }
}
