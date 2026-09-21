using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayRollApi.Domain.Entities.Payroll;

namespace Persistence.Configurations
{
    public class EmployeeAbsenceConfiguration : IEntityTypeConfiguration<EmployeeAbsence>
    {
        public void Configure(EntityTypeBuilder<EmployeeAbsence> builder)
        {
            builder.ToTable("EmployeeAbsences", "payroll");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Notes).HasMaxLength(250);
            builder.HasIndex(x => new { x.EmployeeId, x.AbsenceDate }).IsUnique();
            builder.HasIndex(x => x.AbsenceDate).IncludeProperties(x => x.EmployeeId);

            builder.HasOne<Employee>().WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
