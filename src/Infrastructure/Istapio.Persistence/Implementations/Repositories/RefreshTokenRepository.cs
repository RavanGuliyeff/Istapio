using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories;
using Istapio.Persistence.Contexts;
using Istapio.Persistence.Implementations.Generics;
using Microsoft.EntityFrameworkCore;

namespace Istapio.Persistence.Implementations.Repositories;

public sealed class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context) { }

    public async Task<RefreshToken?> GetActiveTokenAsync(string token, CancellationToken ct = default)
        => await _dbSet
            .Include(t => t.User)
            .FirstOrDefaultAsync(
                t => t.Token == token && t.RevokedAt == null && t.ExpiresAt > DateTime.UtcNow,
                ct);

    public async Task RevokeAllUserTokensAsync(
        string userId, string reason, string ip, CancellationToken ct = default)
    {
        var activeTokens = await _dbSet
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevokedByIp = ip;
            token.ReasonRevoked = reason;
        }

        _dbSet.UpdateRange(activeTokens);
    }
}
