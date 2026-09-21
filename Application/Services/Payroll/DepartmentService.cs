using System.Net;
using AutoMapper;
using FluentValidation;
using PayRollApi.Application.Common;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Helpers;
using PayRollApi.Application.Interfaces;
using PayRollApi.Application.Interfaces.Caching;
using PayRollApi.Application.Interfaces.Payroll;
using PayRollApi.Domain.Entities.Payroll;

namespace PayRollApi.Application.Services.Payroll
{
    internal class DepartmentService(
        ICRUDinterface<Department> crud,
        ICRUDinterface<Employee> employeeCrud,
        IDepartmentRepository repository,
        IMapper mapper,
        ILocalizer localizer,
        IValidator<DepartmentDto> validator,
        ICacheService cache) : IDepartmentService
    {
        public async Task<ApiResponse<PageList<DepartmentDto>>> GetPagedAsync(int pageNumber, int pageSize, string? search, string? sortBy, bool sortDescending)
        {
            var variantKey = $"{pageNumber}:{pageSize}:{search}:{sortBy}:{sortDescending}";
            var mapped = await cache.GetOrCreatePagedAsync<Department, PageList<DepartmentDto>>(variantKey, async () =>
            {
                var page = await repository.GetPagedAsync(pageNumber, pageSize, search, sortBy, sortDescending);
                return new PageList<DepartmentDto>(mapper.Map<List<DepartmentDto>>(page.Items), page.TotalCount, page.PageNumber, page.PageSize);
            });
            return new SuccessResponse<PageList<DepartmentDto>>(localizer.Get("SuccessRetrieving"), mapped);
        }

        public async Task<ApiResponse<List<DepartmentLookupDto>>> GetLookupAsync()
        {
            var lookup = await cache.GetOrCreateAsync(CacheKeys.DepartmentsLookup, async () =>
            {
                var departments = await crud.GetAll();
                return mapper.Map<List<DepartmentLookupDto>>(departments.OrderBy(d => d.Name).ToList());
            });
            return new SuccessResponse<List<DepartmentLookupDto>>(localizer.Get("SuccessRetrieving"), lookup);
        }

        public async Task<ApiResponse<DepartmentDto>> GetByIdAsync(int id)
        {
            var department = await crud.GetById(id);
            if (department is null)
                return new FailResponse<DepartmentDto>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound);

            return new SuccessResponse<DepartmentDto>(localizer.Get("SuccessRetrieving"), mapper.Map<DepartmentDto>(department));
        }

        public async Task<ApiResponse<DepartmentDto>> CreateAsync(DepartmentDto dto)
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return validation.ToFailResponse<DepartmentDto>();

            if (await NameExistsAsync(dto.Name, excludeId: null))
                return new FailResponse<DepartmentDto>(localizer.Get("DepartmentNameDuplicate"), HttpStatusCode.Conflict);

            var entity = mapper.Map<Department>(dto);
            await crud.Create(entity);
            await crud.SaveChangesAsync();
            await cache.RemoveAsync(CacheKeys.DepartmentsLookup);
            await cache.InvalidatePagedAsync<Department>();
            return new SuccessResponse<DepartmentDto>(localizer.Get("AddingSuccess"), mapper.Map<DepartmentDto>(entity));
        }

        public async Task<ApiResponse<DepartmentDto>> UpdateAsync(int id, DepartmentDto dto)
        {
            if (id != dto.Id)
                return new FailResponse<DepartmentDto>(localizer.Get("IdMismatch"), HttpStatusCode.BadRequest);

            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return validation.ToFailResponse<DepartmentDto>();

            var entity = await crud.GetById(id);
            if (entity is null)
                return new FailResponse<DepartmentDto>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound);

            if (await NameExistsAsync(dto.Name, excludeId: id))
                return new FailResponse<DepartmentDto>(localizer.Get("DepartmentNameDuplicate"), HttpStatusCode.Conflict);

            mapper.Map(dto, entity);
            await crud.Update(entity);
            await crud.SaveChangesAsync();
            await cache.RemoveAsync(CacheKeys.DepartmentsLookup);
            await cache.InvalidatePagedAsync<Department>();
            // Name/IncentivePercent are denormalized into the cached employee list DTO.
            await cache.InvalidatePagedAsync<Employee>();
            return new SuccessResponse<DepartmentDto>(localizer.Get("SuccessUpdating"), mapper.Map<DepartmentDto>(entity));
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var entity = await crud.GetById(id);
            if (entity is null)
                return new FailResponse<bool>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound);

            var employees = await employeeCrud.GetAll();
            if (employees.Any(e => e.DepartmentId == id))
                return new FailResponse<bool>(localizer.Get("DepartmentHasActiveEmployees"), HttpStatusCode.Conflict);

            entity.IsDeleted = true;
            await crud.Update(entity);
            await crud.SaveChangesAsync();
            await cache.RemoveAsync(CacheKeys.DepartmentsLookup);
            await cache.InvalidatePagedAsync<Department>();
            return new SuccessResponse<bool>(localizer.Get("DELETESuccess"));
        }

        private async Task<bool> NameExistsAsync(string name, int? excludeId)
        {
            var departments = await crud.GetAll();
            return departments.Any(d => d.Id != excludeId && string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
