namespace Istapio.Application.Models.DTOs.Cache;

public sealed record OtpEntry(
    string Code,
    DateTime ExpiresAt,
    DateTime CreatedAt
)
{
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
