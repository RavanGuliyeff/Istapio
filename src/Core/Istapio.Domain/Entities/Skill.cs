using Istapio.Domain.Entities.Common;

namespace Istapio.Domain.Entities;

public class Skill : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<JobPostSkill> JobPostSkills { get; set; } = new List<JobPostSkill>();
    public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>(); 
}


