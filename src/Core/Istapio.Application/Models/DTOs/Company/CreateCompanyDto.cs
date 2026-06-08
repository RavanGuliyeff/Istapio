namespace Istapio.Application.Models.DTOs.Company;

public record CreateCompanyDto(
    string Name,
    string Description,
    string? LogoUrl = null,
    string? UserId = null
);