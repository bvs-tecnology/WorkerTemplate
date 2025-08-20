using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Hangfire;
using Hangfire.Client;
using Hangfire.Console;
using Hangfire.Redis.StackExchange;
using Hangfire.Server;
using Infra.Utils.Configuration;

namespace API.Configurators;
[ExcludeFromCodeCoverage]
public static class HangfireConfigurator
{
    public static IServiceCollection AddLocalHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        var connection = Builders.BuildRedisConnectionString(configuration);
        services.AddHangfire(options => {
            options.UseRedisStorage(connection, new RedisStorageOptions { Prefix = "HANGFIRE" });
            options.UseConsole();
        });
        GlobalConfiguration.Configuration.UseFilter(new HangfireTraceIdFilter());
        services.AddHangfireServer();

        return services;
    }
}
[ExcludeFromCodeCoverage]
public class HangfireTraceIdFilter : IClientFilter, IServerFilter
{
    private static readonly ActivitySource HangfireActivitySource = new("Hangfire");
    private const string TraceParentIdKey = "TraceParentId";
    private const string ActivityKey = "TraceActivity";
    public void OnCreating(CreatingContext context)
    {
        if (Activity.Current?.Id is not null)
        {
            context.SetJobParameter(TraceParentIdKey, Activity.Current.Id);
        }
    }

    public void OnCreated(CreatedContext context) { }

    public void OnPerforming(PerformingContext context)
    {var parentId = context.GetJobParameter<string>(TraceParentIdKey);
        if (parentId is not null)
        {
            var activity = HangfireActivitySource.StartActivity(
                $"{context.BackgroundJob.Job.Type.Name}.{context.BackgroundJob.Job.Method.Name}",
                ActivityKind.Server,
                parentId
            );

            if (activity is not null)
            {
                context.Items[ActivityKey] = activity;
            }
        }
    }

    public void OnPerformed(PerformedContext context)
    {
        if (context.Items.TryGetValue(ActivityKey, out var activityItem) && activityItem is Activity activity)
        {
            activity.Dispose();
        }
    }
}
