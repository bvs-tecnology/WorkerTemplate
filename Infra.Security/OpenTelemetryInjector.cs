using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Infra.Security;

public static class OpenTelemetryInjector
{
    public static IServiceCollection AddOpenTemeletryConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var apiName = configuration["ApiName"]!;
        var otelUri = configuration["OpenTelemetryUrl"]!;
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(apiName);
        services.AddOpenTelemetry()
            .WithTracing(traceBuilder =>
            {
                traceBuilder
                    .AddSource(apiName)
                    .SetResourceBuilder(resourceBuilder)
                    .AddAspNetCoreInstrumentation()
                    .AddNpgsql()
                    .AddOtlpExporter(opt => opt.Endpoint = new Uri(otelUri));
            })
            .WithMetrics(metricsBuilder =>
            {
                metricsBuilder
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .SetResourceBuilder(resourceBuilder)
                    .AddOtlpExporter(opt => opt.Endpoint = new Uri(otelUri));
            });
        
        return services;
    }

    public static ILoggingBuilder AddOpenTelemetryConfiguration(this ILoggingBuilder logging, IConfiguration configuration)
    {
        var apiName = configuration["ApiName"]!;
        var otelUri = configuration["OpenTelemetryUrl"]!;
        var resourceBuilder = ResourceBuilder.CreateDefault().AddService(apiName);
        logging.AddOpenTelemetry(loggingBuilder =>
        {
            loggingBuilder.IncludeFormattedMessage = true;
            loggingBuilder.SetResourceBuilder(resourceBuilder)
                .AttachLogsToActivityEvent()
                .AddOtlpExporter(opt => opt.Endpoint = new Uri(otelUri));
        });
        return logging;
    }
}