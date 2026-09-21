using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PayRollApi.Domain.Entities.SecurityModule;

namespace Persistence.Configurations
{
    public class RefreshTokensConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> entity)
        {
            entity.ToTable("RefreshTokens", "security");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.TokenHash)
                .IsRequired()
                .HasMaxLength(64);
            entity.HasIndex(e => e.TokenHash).IsUnique();

            entity.HasOne<AdminUser>()
                .WithMany()
                .HasForeignKey(e => e.AdminUserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.Ignore(e => e.IsExpired);
            entity.Ignore(e => e.IsActive);
        }
    }
}
