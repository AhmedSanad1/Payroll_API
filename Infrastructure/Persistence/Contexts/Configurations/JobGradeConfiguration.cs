using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayRollApi.Domain.Entities.Payroll;

namespace Persistence.Configurations
{
    public class JobGradeConfiguration : IEntityTypeConfiguration<JobGrade>
    {
        // Fixed, deterministic seed timestamp — HasData values must not change between migration builds.
        private static readonly DateTime SeedDate = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Configure(EntityTypeBuilder<JobGrade> builder)
        {
            builder.ToTable("JobGrades", "payroll");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.NameAr).IsRequired().HasMaxLength(50);
            builder.Property(x => x.NameEn).IsRequired().HasMaxLength(50);
            builder.Property(x => x.BaseSalary).HasPrecision(18, 2);

            builder.HasData(
                new JobGrade { Id = 1, NameAr = "أولى", NameEn = "First", BaseSalary = 12000m, CreatedAt = SeedDate },
                new JobGrade { Id = 2, NameAr = "ثانية", NameEn = "Second", BaseSalary = 9000m, CreatedAt = SeedDate },
                new JobGrade { Id = 3, NameAr = "ثالثة", NameEn = "Third", BaseSalary = 6500m, CreatedAt = SeedDate });
        }
    }
}
