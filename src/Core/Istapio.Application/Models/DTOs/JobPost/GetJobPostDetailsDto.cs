using Istapio.Application.Models.DTOs.Common;

namespace Istapio.Application.Models.DTOs.JobPost;

public record GetJobPostDetailsDto(
    Guid Id,
    string Title,
    string Description,
    string Requirements,
    bool IsActive,
    long ViewCount,
    DateTime? LastDate,

    Guid CompanyId,
    string CompanyName,

    Guid CategoryId,
    string CategoryName,

    DateTime CreatedAt,
    string? CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy
) : BaseAuditableDto(Id, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy);