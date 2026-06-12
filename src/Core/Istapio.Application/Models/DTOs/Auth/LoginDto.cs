namespace Istapio.Application.Models.DTOs.Auth;

public sealed record LoginDto(
    string Email,
    string Password
);

