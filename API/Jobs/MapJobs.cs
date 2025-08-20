using System.Diagnostics.CodeAnalysis;
using Domain.Interfaces.Services;
using Hangfire;

namespace API.Jobs;
[ExcludeFromCodeCoverage]
public static class MapJobs
{
    private static RecurringJobOptions _options => new()
    {
        TimeZone = TimeZoneInfo.Utc,
        MisfireHandling = MisfireHandlingMode.Relaxed
    };
    public static void MapTestJobs()
    {
        RecurringJob.AddOrUpdate<ITestService>(
            recurringJobId: "test",
            methodCall: job => job.Test(),
            cronExpression: Cron.Daily,
            options: _options
        );
    }
}
