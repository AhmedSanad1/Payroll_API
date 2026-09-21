using System.Net;
using PayRollApi.Application.Common;
using PayRollApi.Application.Interfaces;
using PayRollApi.Application.Interfaces.Caching;
using AutoMapper;
using PayRollApi.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PayRollApi.Controllers.Common
{
    // No class-level [Route]: each concrete controller sets its own literal REST path.
    [Authorize]
    [ApiController]
    public abstract class CrudControllerBase<TEntity, TDto>(
        ICRUDinterface<TEntity> crud,
        IMapper mapper,
        ILocalizer localizer,
        ICacheService cache) : ControllerBase
        where TEntity : class, IAuditable
        where TDto : class, IHasId
    {
        private string ListCacheKey => CacheKeys.EntityList<TEntity>();

        // Some services cache the raw entities under a second key — a write has to clear both.
        private string RawListCacheKey => CacheKeys.RawEntityList<TEntity>();

        private Task InvalidateListCachesAsync() =>
            Task.WhenAll(cache.RemoveAsync(ListCacheKey), cache.RemoveAsync(RawListCacheKey));

        [HttpGet]
        public virtual async Task<IActionResult> GetAll()
        {
            var dtos = await cache.GetOrCreateAsync(ListCacheKey, async () =>
            {
                var entities = await crud.GetAll();
                return mapper.Map<ICollection<TDto>>(entities);
            });
            return Ok(new SuccessResponse<ICollection<TDto>>(localizer.Get("SuccessRetrieving"), dtos));
        }

        [HttpGet("{id:int}")]
        public virtual async Task<IActionResult> Get(int id)
        {
            var entity = await crud.GetById(id);
            if (entity is null)
                return Respond(new FailResponse<TDto>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound));

            return Ok(new SuccessResponse<TDto>(localizer.Get("SuccessRetrieving"), mapper.Map<TDto>(entity)));
        }

        [HttpPost]
        public virtual async Task<IActionResult> AddNew(TDto dto)
        {
            var entity = mapper.Map<TEntity>(dto);
            await crud.Create(entity);
            await crud.SaveChangesAsync();
            await InvalidateListCachesAsync();
            return Ok(new SuccessResponse<TDto>(localizer.Get("AddingSuccess"), mapper.Map<TDto>(entity)));
        }

        [HttpPut("{id:int}")]
        public virtual async Task<IActionResult> Update(int id, TDto dto)
        {
            if (id != dto.Id)
                return Respond(new FailResponse<TDto>(localizer.Get("IdMismatch"), HttpStatusCode.BadRequest));

            var entity = await crud.GetById(id);
            if (entity is null)
                return Respond(new FailResponse<TDto>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound));

            // TDto carries no audit fields, so map onto the tracked instance, not a new one.
            mapper.Map(dto, entity);
            await crud.Update(entity);
            await crud.SaveChangesAsync();
            await InvalidateListCachesAsync();
            return Ok(new SuccessResponse<TDto>(localizer.Get("SuccessUpdating"), mapper.Map<TDto>(entity)));
        }

        [HttpDelete("{id:int}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            // crud.Delete would Remove() a null entity without this check.
            var entity = await crud.GetById(id);
            if (entity is null)
                return Respond(new FailResponse<bool>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound));

            await crud.Delete(id);
            await crud.SaveChangesAsync();
            await InvalidateListCachesAsync();
            return Ok(new SuccessResponse<bool>(localizer.Get("DELETESuccess")));
        }

        protected IActionResult Respond<T>(ApiResponse<T> response) => StatusCode((int)response.StatusCode, response);
    }
}
