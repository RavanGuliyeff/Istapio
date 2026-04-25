using Istapio.Domain.Entities.Common;

namespace Istapio.Domain.Entities;

public sealed class RefreshToken : BaseEntity
{
    // Scalar properties
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedByIp { get; set; } = string.Empty;
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }  // Token rotate edəndə
    public string? ReasonRevoked { get; set; }

    // Navigation
    public string UserId { get; set; }
    public AppUser User { get; set; } = null!;

    // Helper properties
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt != null;
    public bool IsActive => !IsRevoked && !IsExpired;
}
