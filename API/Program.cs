using API.Configurators;
using API.Jobs;
using Hangfire;
using HealthChecks.UI.Client;
using Infra.IoC;
using Infra.Security;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

#region Injections
builder.Services
    .AddOpenTelemetryConfiguration(builder.Configuration)
    .AddLocalServices(builder.Configuration)
    .AddLocalHttpClients(builder.Configuration)
    .AddLocalUnitOfWork(builder.Configuration)
    .AddLocalCache(builder.Configuration)
    .AddLocalMassTransit(builder.Configuration)
    .AddLocalHangfire(builder.Configuration)
    .AddLocalHealthChecks(builder.Configuration)
    .AddLocalAuthentication(builder.Configuration)
    .AddLocalCors()
    .AddOptions();
builder.Logging
    .AddOpenTelemetryConfiguration(builder.Configuration);
#endregion

var app = builder.Build();

#region Middlewares
app.UseLocalCors(builder.Environment);
app.UseHangfireDashboard();
MapJobs.MapTestJobs();
app.MapHealthChecks("health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
#endregion

app.Run();

