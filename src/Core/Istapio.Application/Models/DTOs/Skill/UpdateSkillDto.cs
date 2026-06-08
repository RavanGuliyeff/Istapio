using Istapio.Application.Models.DTOs.Common;

namespace Istapio.Application.Models.DTOs.Skill;

public record UpdateSkillDto(
    Guid Id,
    string Name
) : BaseDto(Id);