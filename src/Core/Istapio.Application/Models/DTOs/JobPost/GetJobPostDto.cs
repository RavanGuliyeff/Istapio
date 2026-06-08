using Istapio.Application.Models.DTOs.Common;

namespace Istapio.Application.Models.DTOs.JobPost;

public record GetJobPostDto(
    Guid Id,
    string Title,
    string Description,
    string Requirements,
    bool IsActive,
    long ViewCount,
    DateTime? LastDate,
    Guid CompanyId,
    Guid CategoryId,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy
) : BaseAuditableDto(Id, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy);