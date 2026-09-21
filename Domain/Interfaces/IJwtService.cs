using PayRollApi.Domain.Entities.SecurityModule;

namespace PayRollApi.Domain.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(AdminUser user);

        // Returns the raw token (sent to the client) and its entity to persist (hash only).
        (string RawToken, RefreshToken Entity) GenerateRefreshToken(int adminUserId);

        string HashRefreshToken(string rawToken);
    }
}
