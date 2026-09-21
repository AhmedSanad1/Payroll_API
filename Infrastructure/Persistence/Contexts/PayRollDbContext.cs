using Microsoft.EntityFrameworkCore;
using PayRollApi.Domain.Common;
using PayRollApi.Domain.Entities.Payroll;
using PayRollApi.Domain.Entities.SecurityModule;

namespace PayRollApi.Infrastructure.Persistence.Contexts;

public class PayRollDbContext : DbContext
{
    public PayRollDbContext(DbContextOptions<PayRollDbContext> options) : base(options)
    {
    }

    public DbSet<AdminUser> AdminUsers { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<JobGrade> JobGrades { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<ServiceIncentiveTier> ServiceIncentiveTiers { get; set; }
    public DbSet<AttendanceRule> AttendanceRules { get; set; }
    public DbSet<PayrollSettings> PayrollSettings { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmployeeAbsence> EmployeeAbsences { get; set; }
    public DbSet<PayrollRun> PayrollRuns { get; set; }
    public DbSet<PayrollItem> PayrollItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PayRollDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        StampAuditableTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampAuditableTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    // CreatedBy/LastModifiedBy are set by the caller, not stamped here.
    private void StampAuditableTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<IAuditable>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;
            else if (entry.State == EntityState.Modified)
                entry.Entity.LastModifiedAt = DateTime.UtcNow;
        }
    }
}
