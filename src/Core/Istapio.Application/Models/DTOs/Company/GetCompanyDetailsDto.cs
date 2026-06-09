using Istapio.Application.Models.DTOs.Common;

namespace Istapio.Application.Models.DTOs.Company;

public record GetCompanyDetailsDto(
    Guid Id,
    string Name,
    string Description,
    string? LogoUrl,
    string UserId,
    string UserName,
    ICollection<GetCompanyJobPostDto> JobPosts,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy
) : BaseAuditableDto(Id, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy);
