using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayRollApi.Application.Interfaces;
using PayRollApi.Domain.Entities.SecurityModule;
using PayRollApi.Infrastructure.Persistence.Contexts;

namespace PayRollApi.Infrastructure.Persistence.Seed
{
    // No self-registration — the admin account comes from config, never a migration.
    public static class AdminUserSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PayRollDbContext>();

            if (await context.AdminUsers.AnyAsync())
                return;

            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var username = configuration["AdminSeed:Username"];
            var password = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException(
                    "No admin user exists and AdminSeed:Username / AdminSeed:Password are not configured. " +
                    "Set them with 'dotnet user-secrets set' or environment variables before starting the app.");

            var passwordHashing = scope.ServiceProvider.GetRequiredService<IPasswordHashing>();

            context.AdminUsers.Add(new AdminUser
            {
                Username = username,
                PasswordHash = passwordHashing.HashPassword(password),
                IsActive = true
            });

            await context.SaveChangesAsync();
        }
    }
}
