using System.Globalization;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PayRollApi.Application.Interfaces;
using PayRollApi.Application.Interfaces.Payroll;
using PayRollApi.Application.Logging;
using PayRollApi.Application.Mapping;
using PayRollApi.Application.Services;
using PayRollApi.Application.Services.Payroll;
using PayRollApi.Application.Validators;
using PayRollApi.Domain.Services.Payroll;

namespace PayRollApi.Application
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ILocalizer, Localizer>();
            services.AddScoped<IApplicationLogger, ApplicationLogger>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAbsenceService, AbsenceService>();
            services.AddScoped<IPayrollRunService, PayrollRunService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddSingleton<PayrollCalculator>();
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
            services.AddAutoMapper(cfg =>
            {
                cfg.AllowNullCollections = true;
            }, typeof(LoginRequestProfile).Assembly);

            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}
