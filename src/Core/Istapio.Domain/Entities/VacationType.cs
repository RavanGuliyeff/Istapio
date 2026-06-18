using Istapio.Domain.Entities.Common;

namespace Istapio.Domain.Entities;

public class VacationType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
}


