using AutoMapper;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Domain.Entities.Payroll;

namespace PayRollApi.Application.Mapping
{
    public class PayrollProfile : Profile
    {
        public PayrollProfile()
        {
            CreateMap<JobGrade, JobGradeDto>().ReverseMap();

            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<Department, DepartmentLookupDto>();

            CreateMap<ServiceIncentiveTier, ServiceIncentiveTierDto>().ReverseMap();

            CreateMap<AttendanceRule, AttendanceRuleDto>().ReverseMap();

            CreateMap<PayrollSettings, PayrollSettingsDto>().ReverseMap();

            CreateMap<Employee, EmployeeDto>().ReverseMap();
        }
    }
}
