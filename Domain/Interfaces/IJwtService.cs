using PayRollApi.Domain.Entities.SecurityModule;

namespace PayRollApi.Domain.Interfaces
{
    public interface ITokenService
    {
        // Returns the signed token and the expiry it was actually stamped with,
        // so callers never have to re-derive it from configuration.
        (string Token, DateTime ExpiresAtUtc) GenerateAccessToken(AdminUser user);

        // Returns the raw token (sent to the client) and its entity to persist (hash only).
        (string RawToken, RefreshToken Entity) GenerateRefreshToken(int adminUserId);

        string HashRefreshToken(string rawToken);
    }
}
