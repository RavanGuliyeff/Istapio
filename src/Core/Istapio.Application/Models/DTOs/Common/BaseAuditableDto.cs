namespace Istapio.Application.Models.DTOs.Common;

public abstract record BaseAuditableDto(
    Guid Id,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy
) : BaseDto(Id);