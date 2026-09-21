using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayRollApi.Domain.Entities.Payroll;
using PayRollApi.Domain.Enums;

namespace Persistence.Configurations
{
    public class AttendanceRuleConfiguration : IEntityTypeConfiguration<AttendanceRule>
    {
        private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<AttendanceRule> builder)
        {
            builder.ToTable("AttendanceRules", "payroll");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Percent).HasPrecision(5, 2);
            builder.Property(x => x.AdjustmentType).HasConversion<byte>();

            builder.HasData(
                new AttendanceRule { Id = 1, FromDays = 0, ToDays = 0, AdjustmentType = AttendanceAdjustmentType.Bonus, Percent = 5m, CreatedAt = SeedDate },
                new AttendanceRule { Id = 2, FromDays = 3, ToDays = 5, AdjustmentType = AttendanceAdjustmentType.Deduction, Percent = 5m, CreatedAt = SeedDate },
                new AttendanceRule { Id = 3, FromDays = 6, ToDays = 10, AdjustmentType = AttendanceAdjustmentType.Deduction, Percent = 10m, CreatedAt = SeedDate },
                new AttendanceRule { Id = 4, FromDays = 11, ToDays = 15, AdjustmentType = AttendanceAdjustmentType.Deduction, Percent = 20m, CreatedAt = SeedDate },
                new AttendanceRule { Id = 5, FromDays = 16, ToDays = null, AdjustmentType = AttendanceAdjustmentType.Deduction, Percent = 30m, CreatedAt = SeedDate });
        }
    }
}
