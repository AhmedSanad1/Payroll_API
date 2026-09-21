using PayRollApi.Domain.Common;

namespace PayRollApi.Domain.Entities.SecurityModule
{
    public class RefreshToken : IAuditable
    {
        public long Id { get; set; }
        public int AdminUserId { get; set; }

        // SHA-256 hex hash of the raw token — the raw value is never stored.
        public required string TokenHash { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByTokenHash { get; set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => RevokedAt is null && !IsExpired;

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}
