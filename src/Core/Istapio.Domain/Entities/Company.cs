using Istapio.Domain.Entities.Common;

namespace Istapio.Domain.Entities;

public class Company : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }

    public AppUser User { get; set; } = null!;
    public string UserId { get; set; } = string.Empty;
    public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
}


