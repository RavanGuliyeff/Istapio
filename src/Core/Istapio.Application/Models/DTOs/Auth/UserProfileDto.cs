namespace Istapio.Application.Models.DTOs.Auth;

public sealed record UserProfileDto(
    string Id,
    string Email,
    string? FirstName,
    string? LastName,
    DateTime Created,
    IReadOnlyList<string> Roles,
    IReadOnlyList<GetUserCompanyDto> Companies,
    IReadOnlyList<GetUserSkillDto> Skills
);