using Istapio.API.Extensions;

namespace Istapio.API.Configurations;

public static class ApplicationServicesConfiguration
{

    public static IServiceCollection AddAllApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddWebApiServices();
        services.AddSwaggerDocumentation(); 

        services.AddAppCors();

        services.AddHttpContextAccessor();

        services.AddLayeredServices(configuration);

        services.AddAppSecurity(configuration);

        return services;
    }


    public static WebApplication UseAllApplicationMiddlewares(this WebApplication app, IHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseRouting();

        app.UseHttpsRedirection();

        app.UseAppCors();

        app.UseAppSecurity();

        app.UseSwaggerDocumentation();

        return app;
    }
}