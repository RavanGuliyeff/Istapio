using Microsoft.AspNetCore.Identity;
using Istapio.Domain.Constants;

namespace Istapio.Persistence.Seeders;

public class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[]
        {
            Roles.SuperAdmin,
            Roles.Admin,
            Roles.Moderator,
            Roles.Member
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
