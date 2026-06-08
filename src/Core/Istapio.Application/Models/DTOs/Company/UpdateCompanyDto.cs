using Istapio.Application.Models.DTOs.Common;

namespace Istapio.Application.Models.DTOs.Company;

public record UpdateCompanyDto(
    Guid Id,
    string Name,
    string Description,
    string? LogoUrl = null
) : BaseDto(Id);