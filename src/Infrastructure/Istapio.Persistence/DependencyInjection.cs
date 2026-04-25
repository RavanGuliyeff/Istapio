using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Istapio.Domain.Entities;
using Istapio.Persistence.Contexts;
using System.Reflection;

namespace Istapio.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {

            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions =>
                {
                    //npgsqlOptions.MigrationsAssembly(assembly.FullName);

                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorCodesToAdd: null
                    );

                    npgsqlOptions.CommandTimeout(30);
                }
            );

#if DEBUG
            // Development-də SQL-ləri görmək üçün
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        });

        services.AddIdentity<AppUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;

            options.User.RequireUniqueEmail = true;

            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        RegisterRepositories(services, assembly);

        return services;
    }

    private static void RegisterRepositories(IServiceCollection services, Assembly assembly)
    {
        var repositoryTypes = assembly.GetTypes()
            .Where(t => t.IsClass
                     && !t.IsAbstract
                     && t.Name.EndsWith("Repository")
                     && t.Namespace?.Contains("Repositories") == true)
            .ToList();

        foreach (var repositoryType in repositoryTypes)
        {
            var interfaceType = repositoryType.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{repositoryType.Name}");

            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, repositoryType);
            }
        }
    }
}
