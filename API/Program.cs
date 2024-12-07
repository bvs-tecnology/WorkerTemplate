using System.Text.Json.Serialization;
using API.Middlewares;
using Domain.SeedWork.Notification;
using HealthChecks.UI.Client;
using Infra.IoC;
using Infra.Security;
using Infra.Utils.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var apiName = "Template API";

builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddOpenApi();

#region Local Injections
builder.Services.AddLocalServices(builder.Configuration);
builder.Services.AddLocalHttpClients(builder.Configuration);
builder.Services.AddLocalUnitOfWork(builder.Configuration);
builder.Services.AddLocalCache(builder.Configuration);
builder.Services.AddLocalHealthChecks(builder.Configuration);
#endregion

#region Security Injections
builder.Services.AddLocalSecurity(builder.Configuration);
builder.Services.AddLocalCors();
#endregion

builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

builder.Services.AddHttpContextAccessor();
builder.Services.AddOptions();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

ServiceLocator.Initialize(app.Services.GetRequiredService<IContainer>());
app.MapHealthChecks("health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
app.MapControllers();
app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthorization();

#region Middlewares
app.UseMiddleware<ControllerMiddleware>();
app.UseMiddleware<RedisCacheMiddleware>();
#endregion

try
{
    Log.Information($"[{apiName}] Starting the application...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, $"[{apiName}] Application failed to start");
}
finally
{
    Log.Information($"[{apiName}] Finishing the application...");
    Log.CloseAndFlush();
}

