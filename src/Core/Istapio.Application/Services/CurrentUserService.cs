using Istapio.Domain.Interfaces;

namespace Istapio.Application;

public class CurrentUserService : ICurrentUserService
{
    private static readonly AsyncLocal<UserContext?> _userContext = new();

    public string UserId => _userContext.Value?.UserId ?? "System";
    public string? UserName => _userContext.Value?.UserName;
    public IReadOnlyList<string> Roles => _userContext.Value?.Roles ?? Array.Empty<string>();
    public bool IsAuthenticated => _userContext.Value?.IsAuthenticated ?? false;

    public void SetUser(string userId, string? userName, IReadOnlyList<string> roles, bool isAuthenticated)
    {
        _userContext.Value = new UserContext(userId, userName, roles, isAuthenticated);
    }

    public bool IsInRole(string role)
    {
        return Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    public bool IsInAnyRole(params string[] roles)
    {
        return roles.Any(role => IsInRole(role));
    }

    private sealed record UserContext(
        string UserId,
        string? UserName,
        IReadOnlyList<string> Roles,
        bool IsAuthenticated
    );
}
