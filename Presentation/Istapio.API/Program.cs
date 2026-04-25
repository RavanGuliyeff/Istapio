
using Istapio.API.Configurations;
using Istapio.API.Extensions;
using Istapio.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAllApplicationServices(builder.Configuration);

var app = builder.Build();

app.UseAllApplicationMiddlewares(app.Environment);

app.MapControllers();

app.UseMiddleware<CurrentUserMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

await app.SeedDatabaseAsync();

app.Run();
