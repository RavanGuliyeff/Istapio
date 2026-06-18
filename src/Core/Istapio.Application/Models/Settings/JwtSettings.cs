namespace Istapio.Application.Models.Settings
{
    public sealed class JwtSettings
    {
        public string SecurityKey { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public int AccessTokenExpiresInMinutes { get; init; } = 15;
        public int RefreshTokenExpiresInDays { get; init; } = 7;
    }
}
