using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayRollApi.Domain.Entities.Payroll;

namespace Persistence.Configurations
{
    public class PayrollRunConfiguration : IEntityTypeConfiguration<PayrollRun>
    {
        public void Configure(EntityTypeBuilder<PayrollRun> builder)
        {
            builder.ToTable("PayrollRuns", "payroll");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.PeriodYear, x.PeriodMonth }).IsUnique();
            builder.Property(x => x.Status).HasConversion<byte>();
            builder.Property(x => x.CalculationMode).HasConversion<byte>();
        }
    }
}
