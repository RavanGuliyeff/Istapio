namespace Istapio.Domain.Interfaces;

public interface ICurrentUserService
{
    string UserId { get; }
    string? UserName { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsAuthenticated { get; }

    void SetUser(string userId, string? userName, IReadOnlyList<string> roles, bool isAuthenticated);

    // Helper methods
    bool IsInRole(string role);
    bool IsInAnyRole(params string[] roles);
}