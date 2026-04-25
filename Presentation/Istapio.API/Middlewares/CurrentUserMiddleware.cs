using Istapio.Domain.Interfaces;
using System.Security.Claims;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            string? userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            string? userName = context.User.FindFirstValue(ClaimTypes.Name);

            IReadOnlyList<string> roles = context.User.FindAll(ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList()
                .AsReadOnly();

            currentUserService.SetUser(userId, userName, roles, true);
        }
        else
        {
            currentUserService.SetUser("System", null, Array.Empty<string>(), false);
        }

        await _next(context);
    }
}
