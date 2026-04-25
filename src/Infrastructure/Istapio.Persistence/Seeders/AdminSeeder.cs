using Microsoft.AspNetCore.Identity;
using Istapio.Domain.Constants;
using Istapio.Domain.Entities;

namespace Istapio.Persistence.Seeders;

public class AdminSeeder
{
    public static async Task SeedAsync(UserManager<AppUser> userManager)
    {
        var superAdminEmail = "guliyevtr-ab107@code.edu.az";
        var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);

        if (superAdmin == null)
        {
            superAdmin = new AppUser
            {
                UserName = "superadmin",
                Email = superAdminEmail,
                EmailConfirmed = true,
                FirstName = "Super",
                LastName = "Admin"
            };

            var result = await userManager.CreateAsync(superAdmin, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, Roles.SuperAdmin);
            }
        }

        var adminEmail = "revan.quliyev211@mail.ru";
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new AppUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User"
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, Roles.Admin);
            }
        }
    }
}