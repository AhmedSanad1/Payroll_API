using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace PayRollApi.Infrastructure.Persistence.Contexts
{
    // Only for `dotnet ef` — keeps the tooling from booting Program.cs and its seeders.
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PayRollDbContext>
    {
        // Fresh clones have no appsettings.json; `migrations add` doesn't need a real server anyway.
        private const string NoConfigFallback =
            "Server=.;Database=PayRollDB_DesignTimeOnly;Trusted_Connection=True;TrustServerCertificate=True";

        public PayRollDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PayRollDbContext>();
            optionsBuilder.UseSqlServer(ResolveConnectionString());
            return new PayRollDbContext(optionsBuilder.Options);
        }

        private static string ResolveConnectionString()
        {
            // dotnet ef may run from the solution root or Infrastructure, so walk up to find the API config.
            for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory()); dir is not null; dir = dir.Parent)
            {
                foreach (var candidate in new[]
                         {
                             Path.Combine(dir.FullName, "appsettings.json"),
                             Path.Combine(dir.FullName, "PayRollApi", "appsettings.json")
                         })
                {
                    if (!File.Exists(candidate))
                        continue;

                    var connectionString = new ConfigurationBuilder()
                        .AddJsonFile(candidate)
                        .Build()
                        .GetConnectionString("DefaultConnection");

                    if (!string.IsNullOrWhiteSpace(connectionString))
                        return connectionString;
                }
            }

            return NoConfigFallback;
        }
    }
}
