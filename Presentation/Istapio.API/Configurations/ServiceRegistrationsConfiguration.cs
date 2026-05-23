using Istapio.Application;
using Istapio.Persistence;
using Istapio.Infrastructure;

namespace Istapio.API.Configurations
{
    public static class ServiceRegistrationsConfiguration
    {
        public static IServiceCollection AddLayeredServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplication(configuration);
            services.AddPersistence(configuration);
            services.AddInfrastructure(configuration);

            return services;
        }
    }

}
