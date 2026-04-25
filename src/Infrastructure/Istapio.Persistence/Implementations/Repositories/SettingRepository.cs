using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces;
using Istapio.Domain.Interfaces.Repositories.Generics;
using Istapio.Persistence.Contexts;
using Istapio.Persistence.Implementations.Generics;

namespace Istapio.Persistence.Implementations.Repositories;

public class SettingRepository : AuditableRepository<Setting>, ISettingRepository
{
    public SettingRepository(AppDbContext context, ICurrentUserService currentUserService) : base(context, currentUserService)
    {
    }
}
