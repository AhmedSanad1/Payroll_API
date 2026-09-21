using FluentValidation;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces;

namespace PayRollApi.Application.Validators.Payroll
{
    public class JobGradeDtoValidator : AbstractValidator<JobGradeDto>
    {
        public JobGradeDtoValidator(ILocalizer localizer)
        {
            RuleFor(x => x.NameAr)
                .NotEmpty().WithMessage(string.Format(localizer.Get("Val_Required"), "NameAr"))
                .MaximumLength(50).WithMessage(string.Format(localizer.Get("Val_MaxLength"), "NameAr", 50));

            RuleFor(x => x.NameEn)
                .NotEmpty().WithMessage(string.Format(localizer.Get("Val_Required"), "NameEn"))
                .MaximumLength(50).WithMessage(string.Format(localizer.Get("Val_MaxLength"), "NameEn", 50));

            RuleFor(x => x.BaseSalary)
                .GreaterThan(0).WithMessage(string.Format(localizer.Get("Val_MinValue"), "BaseSalary", 0));
        }
    }
}
