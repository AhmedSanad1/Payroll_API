using FluentValidation;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces;

namespace PayRollApi.Application.Validators.Payroll
{
    public class PayrollSettingsDtoValidator : AbstractValidator<PayrollSettingsDto>
    {
        public PayrollSettingsDtoValidator(ILocalizer localizer)
        {
            RuleFor(x => x.CalculationMode)
                .IsInEnum().WithMessage(string.Format(localizer.Get("Val_Required"), "CalculationMode"));
        }
    }
}
