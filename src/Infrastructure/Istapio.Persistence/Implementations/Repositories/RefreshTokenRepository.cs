using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;
using Istapio.Persistence.Contexts;
using Istapio.Persistence.Implementations.Generics;

namespace Istapio.Persistence.Implementations.Repositories;

public sealed class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }
}