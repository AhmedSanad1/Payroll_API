using FluentValidation;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces;

namespace PayRollApi.Application.Validators.Payroll
{
    public class ServiceIncentiveTierDtoValidator : AbstractValidator<ServiceIncentiveTierDto>
    {
        public ServiceIncentiveTierDtoValidator(ILocalizer localizer)
        {
            RuleFor(x => x.Percent)
                .GreaterThanOrEqualTo(0).WithMessage(string.Format(localizer.Get("Val_MinValue"), "Percent", 0))
                .LessThanOrEqualTo(100).WithMessage(string.Format(localizer.Get("Val_MaxValue"), "Percent", 100));
        }
    }
}
