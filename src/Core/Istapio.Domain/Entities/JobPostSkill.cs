using Istapio.Domain.Entities.Common;

namespace Istapio.Domain.Entities;

public class JobPostSkill : BaseJunctionEntity
{
    public Guid JobPostId { get; set; }
    public JobPost JobPost { get; set; } = null!;

    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = null!;

    protected override IReadOnlyCollection<object?> GetEqualityComponents()
    {
        return new object?[] { JobPostId, SkillId };
    }
}


