using Istapio.Domain.Entities;
using Istapio.Persistence.Contexts;
using Istapio.Persistence.Seeders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Istapio.API.Extensions;

public static class SeederExtension
{
    public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        services.GetRequiredService<AppDbContext>().Database.Migrate();

        try
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();

            var dbContext = services.GetRequiredService<AppDbContext>();


            await RoleSeeder.SeedAsync(roleManager);
            await AdminSeeder.SeedAsync(userManager);

            await CategorySeeder.SeedAsync(dbContext);
            await SettingSeeder.SeedAsync(dbContext);
            await SkillSeeder.SeedAsync(dbContext);
            await VacationTypeSeeder.SeedAsync(dbContext);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }

        return app;
    }
}