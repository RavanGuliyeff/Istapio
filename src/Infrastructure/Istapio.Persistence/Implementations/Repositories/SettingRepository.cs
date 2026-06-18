using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces;
using Istapio.Domain.Interfaces.Repositories;
using Istapio.Persistence.Contexts;
using Istapio.Persistence.Implementations.Generics;

namespace Istapio.Persistence.Implementations.Repositories;

public sealed class SettingRepository : AuditableRepository<Setting>, ISettingRepository
{
    public SettingRepository(AppDbContext context)
        : base(context)
    {
    }
}
