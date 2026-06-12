namespace Istapio.Application.Models.DTOs.Auth;


public sealed record RegisterDto(
    string FirstName,
    string LastName,
    string Email,
    string Password
);

