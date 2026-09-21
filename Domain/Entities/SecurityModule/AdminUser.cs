using PayRollApi.Domain.Common;

namespace PayRollApi.Domain.Entities.SecurityModule
{
    // Single admin account. No self-registration, no roles/permissions — seeded from configuration at startup.
    public class AdminUser : IAuditable
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int? LastModifiedBy { get; set; }
    }
}
