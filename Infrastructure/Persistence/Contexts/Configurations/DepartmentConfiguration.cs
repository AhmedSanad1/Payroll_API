using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayRollApi.Domain.Entities.Payroll;

namespace Persistence.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments", "payroll");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.IncentivePercent).HasPrecision(5, 2);
            builder.HasIndex(x => x.Name).IsUnique().HasFilter("[IsDeleted] = 0");
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
