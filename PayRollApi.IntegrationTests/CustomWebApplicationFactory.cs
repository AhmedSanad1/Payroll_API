using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PayRollApi.Infrastructure.Persistence.Contexts;

namespace PayRollApi.IntegrationTests;

// Boots the real app against an in-memory SQLite DB, so tests go through the actual HTTP pipeline.
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string AdminUsername = "integration.admin";
    public const string AdminPassword = "Test@Passw0rd!";

    private readonly SqliteConnection connection = new("DataSource=:memory:");
    private readonly DbContextOptions<PayRollDbContext> options;

    public CustomWebApplicationFactory()
    {
        connection.Open();
        options = new DbContextOptionsBuilder<PayRollDbContext>().UseSqlite(connection).Options;

        // Program.cs seeds the admin while the host starts, so the schema has to exist before that.
        using var context = new PayRollDbContext(options);
        context.Database.EnsureCreated();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "integration-test-signing-key-at-least-32-chars-long",
                ["Jwt:Issuer"] = "payroll-integration-tests",
                ["Jwt:Audience"] = "payroll-integration-tests",
                ["Jwt:AccessTokenMinutes"] = "15",
                ["Jwt:RefreshTokenDays"] = "7",
                ["AdminSeed:Username"] = AdminUsername,
                ["AdminSeed:Password"] = AdminPassword
            });
        });

        builder.ConfigureServices(services =>
        {
            // Can't just call AddDbContext again — EF would merge both providers. Swap the options object instead.
            services.RemoveAll<DbContextOptions<PayRollDbContext>>();
            services.AddSingleton(options);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            connection.Dispose();
    }
}
