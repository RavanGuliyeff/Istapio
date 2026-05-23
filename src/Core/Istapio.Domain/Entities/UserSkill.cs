using Istapio.Domain.Entities.Common;

namespace Istapio.Domain.Entities;

public class UserSkill : BaseJunctionEntity
{
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;

    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = null!;

    protected override IReadOnlyCollection<object?> GetEqualityComponents()
    {
        return new object?[] { UserId, SkillId };
    }
}


