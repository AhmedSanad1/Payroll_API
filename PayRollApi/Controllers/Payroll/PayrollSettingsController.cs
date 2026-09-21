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
    // Single row, seeded at Id = 1 — no create/delete.
    [Authorize]
    [Route("api/payroll-settings")]
    [ApiController]
    public class PayrollSettingsController(
        ICRUDinterface<PayrollSettings> crud,
        IMapper mapper,
        ILocalizer localizer,
        IValidator<PayrollSettingsDto> validator,
        ICacheService cache) : ControllerBase
    {
        private static string CacheKey => CacheKeys.Singleton<PayrollSettings>();

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var dto = await cache.GetOrCreateAsync<PayrollSettingsDto?>(CacheKey, async () =>
            {
                var settings = await crud.GetById(PayrollSettings.SingletonId);
                return settings is null ? null : mapper.Map<PayrollSettingsDto>(settings);
            });

            if (dto is null)
                return Respond(new FailResponse<PayrollSettingsDto>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound));

            return Ok(new SuccessResponse<PayrollSettingsDto>(localizer.Get("SuccessRetrieving"), dto));
        }

        [HttpPut]
        public async Task<IActionResult> Update(PayrollSettingsDto dto)
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return Respond(validation.ToFailResponse<PayrollSettingsDto>());

            var settings = await crud.GetById(PayrollSettings.SingletonId);
            if (settings is null)
                return Respond(new FailResponse<PayrollSettingsDto>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound));

            mapper.Map(dto, settings);
            await crud.Update(settings);
            await crud.SaveChangesAsync();
            await cache.RemoveAsync(CacheKey);
            return Ok(new SuccessResponse<PayrollSettingsDto>(localizer.Get("SuccessUpdating"), mapper.Map<PayrollSettingsDto>(settings)));
        }

        private IActionResult Respond<T>(ApiResponse<T> response) => StatusCode((int)response.StatusCode, response);
    }
}
