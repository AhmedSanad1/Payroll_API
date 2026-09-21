using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayRollApi.Domain.Entities.Payroll;

namespace Persistence.Configurations
{
    public class ServiceIncentiveTierConfiguration : IEntityTypeConfiguration<ServiceIncentiveTier>
    {
        private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<ServiceIncentiveTier> builder)
        {
            builder.ToTable("ServiceIncentiveTiers", "payroll");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Percent).HasPrecision(5, 2);
            builder.HasIndex(x => x.MinYearsExceeded).IsUnique();

            // Placeholder percentages — not specified by the spec beyond the year thresholds (5, 7).
            builder.HasData(
                new ServiceIncentiveTier { Id = 1, MinYearsExceeded = 5, Percent = 5m, CreatedAt = SeedDate },
                new ServiceIncentiveTier { Id = 2, MinYearsExceeded = 7, Percent = 10m, CreatedAt = SeedDate });
        }
    }
}
