using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Istapio.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

                RegisterRepositories(services, assembly);

        return services;
    }

    private static void RegisterRepositories(IServiceCollection services, Assembly assembly)
    {
        var repositoryTypes = assembly.GetTypes()
            .Where(t => t.IsClass
                     && !t.IsAbstract
                     && t.Name.EndsWith("Service")
                     && t.Namespace?.Contains("Services") == true)
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