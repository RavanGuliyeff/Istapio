using Istapio.Application.Models.DTOs.Common;

namespace Istapio.Application.Models.DTOs.Setting;

public record GetSettingDto(
    Guid Id,
    string Key,
    string Value,
    DateTime CreatedAt,
    string? CreatedBy,
    DateTime? UpdatedAt,
    string? UpdatedBy
) : BaseAuditableDto(Id, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy);
