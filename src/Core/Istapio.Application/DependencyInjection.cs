using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Istapio.Domain.Interfaces;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Istapio.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

        //services.AddAutoMapper(assembly);

        var licenseKey = configuration["AutoMapper:LicenseKey"];

        services.AddAutoMapper(cfg => cfg.LicenseKey = licenseKey,
            Assembly.GetExecutingAssembly());

        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<ValidationFilter>();

        RegisterServices(services, assembly);

        return services;
    }

    private static void RegisterServices(IServiceCollection services, Assembly assembly)
    {
        services.AddSingleton<ICurrentUserService, CurrentUserService>();


        List<Type> serviceTypes = assembly.GetTypes()
            .Where(t => t.IsClass
                     && !t.IsAbstract
                     && t.Name.EndsWith("Service")
                     && t.Namespace?.Contains("Services") == true
                     && t != typeof(CurrentUserService))
            .ToList();

        foreach (Type serviceType in serviceTypes)
        {
            Type? interfaceType = serviceType.GetInterfaces()
                .FirstOrDefault(i => i.Name == $"I{serviceType.Name}");

            if (interfaceType != null)
            {
                services.AddScoped(interfaceType, serviceType);
            }
        }
    }
}
