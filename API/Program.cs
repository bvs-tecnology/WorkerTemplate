using API.Middlewares;
using Hangfire;
using HealthChecks.UI.Client;
using Infra.IoC;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var apiName = "Template API";

#region Local Injections
builder.Services.AddLocalServices(builder.Configuration);
builder.Services.AddLocalHttpClients(builder.Configuration);
builder.Services.AddLocalUnitOfWork(builder.Configuration);
builder.Services.AddLocalCache(builder.Configuration);
builder.Services.AddLocalHangfire(builder.Configuration);
builder.Services.AddLocalHealthChecks(builder.Configuration);
#endregion

builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

var app = builder.Build();

app.UseHangfireDashboard();

app.MapHealthChecks("health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });

#region Middlewares
app.UseMiddleware<RedisCacheMiddleware>();
#endregion

try
{
    Log.Information($"[{apiName}] Starting the worker...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, $"[{apiName}] Worker failed to start");
}
finally
{
    Log.Information($"[{apiName}] Finishing the worker...");
    Log.CloseAndFlush();
}

