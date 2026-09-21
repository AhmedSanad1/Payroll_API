using Microsoft.EntityFrameworkCore;
using PayRollApi.Application.Interfaces;
using PayRollApi.Domain.Entities.SecurityModule;
using PayRollApi.Infrastructure.Persistence.Contexts;

namespace PayRollApi.Infrastructure.Persistence.Repositories
{
    public class RefreshTokenCRUD(PayRollDbContext context) : IRefreshTokenCRUD
    {
        public Task<RefreshToken?> GetByTokenHash(string tokenHash)
        {
            return context.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        }

        public async Task<RefreshToken> Create(RefreshToken refreshToken)
        {
            await context.RefreshTokens.AddAsync(refreshToken);
            return refreshToken;
        }

        public async Task RevokeAllActiveForUser(int adminUserId)
        {
            var now = DateTime.UtcNow;
            var activeTokens = await context.RefreshTokens
                .Where(t => t.AdminUserId == adminUserId && t.RevokedAt == null && t.ExpiresAt > now)
                .ToListAsync();

            foreach (var token in activeTokens)
                token.RevokedAt = now;
        }
    }
}
