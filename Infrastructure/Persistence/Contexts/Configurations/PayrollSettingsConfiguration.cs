using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayRollApi.Domain.Entities.Payroll;
using PayRollApi.Domain.Enums;

namespace Persistence.Configurations
{
    public class PayrollSettingsConfiguration : IEntityTypeConfiguration<PayrollSettings>
    {
        private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<PayrollSettings> builder)
        {
            builder.ToTable("PayrollSettings", "payroll", t => t.HasCheckConstraint("CK_PayrollSettings_SingleRow", "[Id] = 1"));
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.CalculationMode).HasConversion<byte>();

            builder.HasData(new PayrollSettings
            {
                Id = PayrollSettings.SingletonId,
                CalculationMode = PayrollCalculationMode.Additive,
                CreatedAt = SeedDate
            });
        }
    }
}
