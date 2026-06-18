namespace Istapio.Application.Models.DTOs.Auth;

public sealed record GetUserCompanyDto(
    Guid Id,
    string Name,
    string? LogoUrl
);