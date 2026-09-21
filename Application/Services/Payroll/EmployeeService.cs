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
    internal class EmployeeService(
        ICRUDinterface<Employee> crud,
        ICRUDinterface<Department> departmentCrud,
        ICRUDinterface<JobGrade> jobGradeCrud,
        IEmployeeRepository repository,
        IMapper mapper,
        ILocalizer localizer,
        IValidator<EmployeeDto> validator,
        ICacheService cache) : IEmployeeService
    {
        public async Task<ApiResponse<PageList<EmployeeListItemDto>>> GetPagedAsync(
            int pageNumber, int pageSize, string? search, int? departmentId, int? jobGradeId,
            string? sortBy, bool sortDescending)
        {
            var variantKey = $"{pageNumber}:{pageSize}:{search}:{departmentId}:{jobGradeId}:{sortBy}:{sortDescending}";
            var page = await cache.GetOrCreatePagedAsync<Employee, PageList<EmployeeListItemDto>>(variantKey, () =>
                repository.GetPagedAsync(pageNumber, pageSize, search, departmentId, jobGradeId, sortBy, sortDescending));
            return new SuccessResponse<PageList<EmployeeListItemDto>>(localizer.Get("SuccessRetrieving"), page);
        }

        public async Task<ApiResponse<EmployeeDto>> GetByIdAsync(int id)
        {
            var employee = await crud.GetById(id);
            if (employee is null)
                return new FailResponse<EmployeeDto>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound);

            return new SuccessResponse<EmployeeDto>(localizer.Get("SuccessRetrieving"), mapper.Map<EmployeeDto>(employee));
        }

        public async Task<ApiResponse<EmployeeDto>> CreateAsync(EmployeeDto dto)
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return validation.ToFailResponse<EmployeeDto>();

            var referenceError = await ValidateReferencesAsync(dto);
            if (referenceError is not null)
                return referenceError;

            if (await EmailExistsAsync(dto.Email, excludeId: null))
                return new FailResponse<EmployeeDto>(localizer.Get("EmployeeEmailDuplicate"), HttpStatusCode.Conflict);

            var entity = mapper.Map<Employee>(dto);
            await crud.Create(entity);
            await crud.SaveChangesAsync();
            await cache.InvalidatePagedAsync<Employee>();
            return new SuccessResponse<EmployeeDto>(localizer.Get("AddingSuccess"), mapper.Map<EmployeeDto>(entity));
        }

        public async Task<ApiResponse<EmployeeDto>> UpdateAsync(int id, EmployeeDto dto)
        {
            if (id != dto.Id)
                return new FailResponse<EmployeeDto>(localizer.Get("IdMismatch"), HttpStatusCode.BadRequest);

            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return validation.ToFailResponse<EmployeeDto>();

            var entity = await crud.GetById(id);
            if (entity is null)
                return new FailResponse<EmployeeDto>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound);

            var referenceError = await ValidateReferencesAsync(dto);
            if (referenceError is not null)
                return referenceError;

            if (await EmailExistsAsync(dto.Email, excludeId: id))
                return new FailResponse<EmployeeDto>(localizer.Get("EmployeeEmailDuplicate"), HttpStatusCode.Conflict);

            mapper.Map(dto, entity);
            await crud.Update(entity);
            await crud.SaveChangesAsync();
            await cache.InvalidatePagedAsync<Employee>();
            return new SuccessResponse<EmployeeDto>(localizer.Get("SuccessUpdating"), mapper.Map<EmployeeDto>(entity));
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var entity = await crud.GetById(id);
            if (entity is null)
                return new FailResponse<bool>(localizer.Get("Record_NotFound"), HttpStatusCode.NotFound);

            entity.IsDeleted = true;
            await crud.Update(entity);
            await crud.SaveChangesAsync();
            await cache.InvalidatePagedAsync<Employee>();
            return new SuccessResponse<bool>(localizer.Get("DELETESuccess"));
        }

        private async Task<FailResponse<EmployeeDto>?> ValidateReferencesAsync(EmployeeDto dto)
        {
            var department = await departmentCrud.GetById(dto.DepartmentId);
            if (department is null)
                return new FailResponse<EmployeeDto>(localizer.Get("InvalidDepartmentId"), HttpStatusCode.BadRequest);

            var jobGrade = await jobGradeCrud.GetById(dto.JobGradeId);
            if (jobGrade is null)
                return new FailResponse<EmployeeDto>(localizer.Get("InvalidJobGradeId"), HttpStatusCode.BadRequest);

            return null;
        }

        private async Task<bool> EmailExistsAsync(string email, int? excludeId)
        {
            var employees = await crud.GetAll();
            return employees.Any(e => e.Id != excludeId && string.Equals(e.Email, email, StringComparison.OrdinalIgnoreCase));
        }
    }
}
