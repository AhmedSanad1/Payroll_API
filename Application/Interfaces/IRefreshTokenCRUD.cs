using PayRollApi.Domain.Entities.SecurityModule;

namespace PayRollApi.Application.Interfaces
{
    public interface IRefreshTokenCRUD
    {
        Task<RefreshToken?> GetByTokenHash(string tokenHash);
        Task<RefreshToken> Create(RefreshToken refreshToken);

        // Reusing a revoked token is a compromise signal — revokes the whole chain.
        Task RevokeAllActiveForUser(int adminUserId);
    }
}
