using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Istapio.API.Configurations
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Istapio API", Version = "v1" });

                c.TagActionsBy(api =>
                {
                    var controller = api.ActionDescriptor.RouteValues["controller"];
                    return new[] { controller ?? "Default" };
                });

                c.OrderActionsBy(api =>
                {
                    var controller = api.ActionDescriptor.RouteValues["controller"] ?? string.Empty;
                    var method = api.HttpMethod?.ToUpperInvariant() ?? string.Empty;
                    var path = api.RelativePath ?? string.Empty;

                    var methodOrder = method switch
                    {
                        "GET" => 1,
                        "POST" => 2,
                        "PUT" => 3,
                        "PATCH" => 4,
                        "DELETE" => 5,
                        _ => 9
                    };

                    return $"{controller}_{methodOrder:D2}_{path}";
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT token-i daxil edin. Format: Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "Bearer",
                            Name = "Authorization",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
        {           
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Istapio API V1");

                c.ConfigObject.AdditionalItems["tagsSorter"] = "alpha";
                c.ConfigObject.AdditionalItems["operationsSorter"] = "method";
            });

            return app;
        }
    }
}