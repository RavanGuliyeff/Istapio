using Istapio.Domain.Entities.Common;

namespace Istapio.Domain.Entities;

public class JobPost: BaseAuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public long ViewCount { get; set; }

    public DateTime? LastDate { get; set; }
    public Company Company { get; set; } = null!;
    public Guid CompanyId { get; set; }
    public Category Category { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public VacationType VacationType { get; set; } = null!;
    public Guid VacationTypeId { get; set; }
    public ICollection<JobPostSkill> RequiredSkills { get; set; } = new List<JobPostSkill>();
}


