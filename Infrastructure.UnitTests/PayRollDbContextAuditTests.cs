using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PayRollApi.Domain.Entities.SecurityModule;
using PayRollApi.Infrastructure.Persistence.Contexts;

public class PayRollDbContextAuditTests
{
    private static PayRollDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<PayRollDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task SaveChangesAsync_stamps_CreatedAt_on_a_newly_added_IAuditable_entity()
    {
        await using var context = CreateContext();
        var token = new RefreshToken { TokenHash = "hash-1", ExpiresAt = DateTime.UtcNow.AddDays(7), AdminUserId = 1 };
        context.RefreshTokens.Add(token);

        await context.SaveChangesAsync();

        token.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        token.LastModifiedAt.Should().BeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_stamps_LastModifiedAt_on_an_updated_IAuditable_entity()
    {
        await using var context = CreateContext();
        var token = new RefreshToken { TokenHash = "hash-2", ExpiresAt = DateTime.UtcNow.AddDays(7), AdminUserId = 1 };
        context.RefreshTokens.Add(token);
        await context.SaveChangesAsync();

        token.RevokedAt = DateTime.UtcNow;
        context.RefreshTokens.Update(token);
        await context.SaveChangesAsync();

        token.LastModifiedAt.Should().NotBeNull();
        token.LastModifiedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}
