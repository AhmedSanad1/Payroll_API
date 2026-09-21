using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayRollApi.Domain.Entities.Payroll;

namespace Persistence.Configurations
{
    public class PayrollItemConfiguration : IEntityTypeConfiguration<PayrollItem>
    {
        public void Configure(EntityTypeBuilder<PayrollItem> builder)
        {
            builder.ToTable("PayrollItems", "payroll");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => new { x.PayrollRunId, x.EmployeeId }).IsUnique();
            builder.HasIndex(x => new { x.PayrollRunId, x.DepartmentId });

            builder.Property(x => x.EmployeeName).IsRequired().HasMaxLength(150);
            builder.Property(x => x.DepartmentName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.GradeName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.BaseSalary).HasPrecision(18, 2);
            builder.Property(x => x.DeptIncentivePercent).HasPrecision(5, 2);
            builder.Property(x => x.DeptIncentiveAmount).HasPrecision(18, 2);
            builder.Property(x => x.ServiceIncentivePercent).HasPrecision(5, 2);
            builder.Property(x => x.ServiceIncentiveAmount).HasPrecision(18, 2);
            builder.Property(x => x.AttendancePercent).HasPrecision(5, 2);
            builder.Property(x => x.AttendanceAmount).HasPrecision(18, 2);
            builder.Property(x => x.NetSalary).HasPrecision(18, 2);
            builder.Property(x => x.AttendanceAdjustmentType).HasConversion<byte?>();

            builder.HasOne<PayrollRun>().WithMany().HasForeignKey(x => x.PayrollRunId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne<Employee>().WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne<Department>().WithMany().HasForeignKey(x => x.DepartmentId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne<JobGrade>().WithMany().HasForeignKey(x => x.JobGradeId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
