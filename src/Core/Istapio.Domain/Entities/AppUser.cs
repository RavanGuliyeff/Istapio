using Microsoft.AspNetCore.Identity;

namespace Istapio.Domain.Entities;

public sealed class AppUser : IdentityUser
{
    //Scalar Properties
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public byte OtpCountToday { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public string? LastLoginIp { get; set; }


    //Navigation Properties
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

