namespace Istapio.Application.Models.DTOs.Auth;

public sealed record VerifyEmailDto(
    string Email,
    string OtpCode
);

