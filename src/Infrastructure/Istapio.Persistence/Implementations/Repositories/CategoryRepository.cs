using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;
using Istapio.Persistence.Contexts;
using Istapio.Persistence.Implementations.Generics;

namespace Istapio.Persistence.Implementations.Repositories;

public sealed class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }
}