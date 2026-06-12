namespace Istapio.Application.Models.Responses;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    string UserId,
    string Email,
    string? FirstName,
    string? LastName,
    IReadOnlyCollection<string> Roles
);