using FluentValidation;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces;
using PayRollApi.Domain.Common;

namespace PayRollApi.Application.Validators.Payroll
{
    public class EmployeeDtoValidator : AbstractValidator<EmployeeDto>
    {
        public EmployeeDtoValidator(ILocalizer localizer)
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage(string.Format(localizer.Get("Val_Required"), "FullName"))
                .MaximumLength(150).WithMessage(string.Format(localizer.Get("Val_MaxLength"), "FullName", 150));

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage(string.Format(localizer.Get("Val_Required"), "Address"))
                .MaximumLength(250).WithMessage(string.Format(localizer.Get("Val_MaxLength"), "Address", 250));

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage(string.Format(localizer.Get("Val_Required"), "Phone"))
                .MaximumLength(20).WithMessage(string.Format(localizer.Get("Val_MaxLength"), "Phone", 20));

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(string.Format(localizer.Get("Val_Required"), "Email"))
                .EmailAddress().WithMessage(localizer.Get("EmailFormatError"))
                .MaximumLength(150).WithMessage(string.Format(localizer.Get("Val_MaxLength"), "Email", 150));

            RuleFor(x => x.JobGradeId)
                .GreaterThan(0).WithMessage(string.Format(localizer.Get("Val_Required"), "JobGradeId"));

            RuleFor(x => x.DepartmentId)
                .GreaterThan(0).WithMessage(string.Format(localizer.Get("Val_Required"), "DepartmentId"));

            RuleFor(x => x.HireDate)
                .Must(hireDate => hireDate <= DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage(localizer.Get("HireDateInFuture"));

            RuleFor(x => x)
                .Must(x => x.HireDate > x.BirthDate)
                .WithMessage(localizer.Get("HireDateBeforeBirthDate"))
                .DependentRules(() =>
                {
                    RuleFor(x => x)
                        .Must(x => DateCalculations.CompletedYearsBetween(x.BirthDate, x.HireDate) >= 18)
                        .WithMessage(localizer.Get("EmployeeUnderageAtHire"));
                });
        }
    }
}
