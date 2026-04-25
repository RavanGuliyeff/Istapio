using System.Text.Json.Serialization;

namespace Istapio.API.Configurations;

public static class WebApiConfiguration
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
    
        services.AddControllers(options =>
        {
            options.Filters.Add<ValidationFilter>();
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        })
            .AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddEndpointsApiExplorer();

        return services;
    }
}
