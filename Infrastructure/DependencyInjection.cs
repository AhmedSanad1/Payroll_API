using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayRollApi.Application.Interfaces;
using PayRollApi.Application.Interfaces.Caching;
using PayRollApi.Application.Interfaces.Payroll;
using PayRollApi.Application.Interfaces.UOWInterfaces;
using PayRollApi.Domain.Interfaces;
using PayRollApi.Infrastructure.Persistence.Contexts;
using PayRollApi.Infrastructure.Persistence.Repositories;
using PayRollApi.Infrastructure.Persistence.UOWs;
using PayRollApi.Infrastructure.Services;

namespace PayRollApi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<PayRollDbContext>(options =>
                     options.UseSqlServer(connectionString));

            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddSingleton<ILangManager, LangManager>();
            services.AddScoped(typeof(ICRUDinterface<>), typeof(CRUDRepository<>));
            services.AddScoped<IRefreshTokenCRUD, RefreshTokenCRUD>();
            services.AddScoped<IGetUser, GetUserRepository>();
            services.AddScoped<ITokenUOW, TokenUOW>();
            services.AddScoped<IPasswordHashing, PasswordHashing>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IAbsenceRepository, AbsenceRepository>();
            services.AddScoped<IPayrollRunRepository, PayrollRunRepository>();
            services.AddScoped<IRdlcReportRenderer, RdlcReportRenderer>();

            return services;
        }
    }
}
