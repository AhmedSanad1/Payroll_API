using FluentValidation;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces;

namespace PayRollApi.Application.Validators.Payroll
{
    public class DepartmentDtoValidator : AbstractValidator<DepartmentDto>
    {
        public DepartmentDtoValidator(ILocalizer localizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(string.Format(localizer.Get("Val_Required"), "Name"))
                .MaximumLength(100).WithMessage(string.Format(localizer.Get("Val_MaxLength"), "Name", 100));

            RuleFor(x => x.IncentivePercent)
                .GreaterThanOrEqualTo(0).WithMessage(string.Format(localizer.Get("Val_MinValue"), "IncentivePercent", 0))
                .LessThanOrEqualTo(100).WithMessage(string.Format(localizer.Get("Val_MaxValue"), "IncentivePercent", 100));
        }
    }
}
