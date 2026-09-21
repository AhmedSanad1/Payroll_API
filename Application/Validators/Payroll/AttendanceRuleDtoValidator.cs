using FluentValidation;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces;

namespace PayRollApi.Application.Validators.Payroll
{
    public class AttendanceRuleDtoValidator : AbstractValidator<AttendanceRuleDto>
    {
        public AttendanceRuleDtoValidator(ILocalizer localizer)
        {
            RuleFor(x => x.Percent)
                .GreaterThanOrEqualTo(0).WithMessage(string.Format(localizer.Get("Val_MinValue"), "Percent", 0))
                .LessThanOrEqualTo(100).WithMessage(string.Format(localizer.Get("Val_MaxValue"), "Percent", 100));

            RuleFor(x => x)
                .Must(x => x.ToDays is null || x.ToDays >= x.FromDays)
                .WithMessage(string.Format(localizer.Get("Val_MinValue"), "ToDays", "FromDays"));

            RuleFor(x => x.AdjustmentType)
                .IsInEnum().WithMessage(string.Format(localizer.Get("Val_Required"), "AdjustmentType"));
        }
    }
}
