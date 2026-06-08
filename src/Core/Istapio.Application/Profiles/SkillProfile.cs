using AutoMapper;
using Istapio.Application.Models.DTOs.Skill;
using Istapio.Domain.Entities;

namespace Istapio.Application.Profiles;

public class SkillProfile : Profile
{
    public SkillProfile()
    {
        CreateMap<Skill, GetSkillDto>();
        CreateMap<Skill, CreateSkillDto>();
        CreateMap<Skill, UpdateSkillDto>();
    }
}