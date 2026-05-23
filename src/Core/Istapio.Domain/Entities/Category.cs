using Istapio.Domain.Entities.Common;

namespace Istapio.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public Category? Parent { get; set; }
    public Guid? ParentId { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();
}


