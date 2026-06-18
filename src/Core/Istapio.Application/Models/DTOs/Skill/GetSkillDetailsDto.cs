namespace Istapio.Application.Models.DTOs.Skill;

public record GetSkillDetailsDto(
    Guid Id,
    string Name,
    List<GetSkillJobPostDto> JobPosts,
    List<GetSkillUserDto> Users
);
