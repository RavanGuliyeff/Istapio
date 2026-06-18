using Istapio.Domain.Entities;
using Istapio.Domain.Interfaces.Repositories.Generics;

namespace Istapio.Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetActiveTokenAsync(string token, CancellationToken ct = default);
    Task RevokeAllUserTokensAsync(
        string userId, string reason, string ip, CancellationToken ct = default);
}