using PayRollApi.Application.DTOs;
using PayRollApi.Application.Interfaces;
using FluentValidation;

namespace PayRollApi.Application.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator(ILocalizer localizer)
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage(localizer.Get("NotEmptyEmailOrUserName"));

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(localizer.Get("InvalidCredentials"));
        }
    }
}
