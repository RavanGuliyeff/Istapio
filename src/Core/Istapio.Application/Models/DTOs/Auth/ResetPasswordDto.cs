namespace Istapio.Application.Models.DTOs.Auth;

public sealed record ResetPasswordDto(
    string Email,
    string Token,
    string NewPassword
);

