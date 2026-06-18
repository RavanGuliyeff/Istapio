namespace Istapio.Application.Models.DTOs.Auth;

public sealed record LoginDto(
    string EmailOrUsername,
    string Password
);

