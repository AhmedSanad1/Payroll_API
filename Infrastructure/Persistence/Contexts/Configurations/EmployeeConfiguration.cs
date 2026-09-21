using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayRollApi.Domain.Entities.Payroll;

namespace Persistence.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees", "payroll");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FullName).IsRequired().HasMaxLength(150);
            builder.HasIndex(x => x.FullName);
            builder.Property(x => x.Address).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Phone).IsRequired().HasMaxLength(20);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
            builder.HasIndex(x => x.Email).IsUnique().HasFilter("[IsDeleted] = 0");
            builder.HasIndex(x => x.DepartmentId).HasFilter("[IsDeleted] = 0");
            // Every payroll run filters on HireDate, so it gets its own index.
            builder.HasIndex(x => x.HireDate).HasFilter("[IsDeleted] = 0");

            builder.HasOne<Department>().WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne<JobGrade>().WithMany().HasForeignKey(x => x.JobGradeId).OnDelete(DeleteBehavior.NoAction);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
