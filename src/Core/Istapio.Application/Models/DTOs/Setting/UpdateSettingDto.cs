using Istapio.Application.Models.DTOs.Common;

namespace Istapio.Application.Models.DTOs.Setting;

public record UpdateSettingDto(
    Guid Id,
    string Key,
    string Value
) : BaseDto(Id);