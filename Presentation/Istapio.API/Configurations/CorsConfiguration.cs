namespace Istapio.API.Configurations
{
    public static class CorsConfiguration
    {
        public const string AllowAllOrigins = "_IstapioAllowAllOrigins";

        public static IServiceCollection AddAppCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowAllOrigins,
                    policy =>
                    {
                        
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader() 
                              .AllowAnyMethod(); 

                    });
            });
            return services;
        }

        public static IApplicationBuilder UseAppCors(this IApplicationBuilder app)
        {
            app.UseCors(AllowAllOrigins);
            return app;
        }
    }

}
