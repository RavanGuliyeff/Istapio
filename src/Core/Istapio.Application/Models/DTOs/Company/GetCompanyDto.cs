using Istapio.Application.Models.DTOs.Common;

namespace Istapio.Application.Models.DTOs.Company;

public record GetCompanyDto(
    Guid Id,
    string Name,
    string Description,
    string? LogoUrl,
    string UserId,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy
) : BaseAuditableDto(Id, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy);