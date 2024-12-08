using API.Jobs;
using Hangfire;
using Hangfire.Console;
using Hangfire.Redis.StackExchange;
using Infra.Utils.Configuration;

namespace API.Configurations
{
    public static class HangfireConfigurator
    {
        public static void AddLocalHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = Builders.BuildRedisConnectionString(configuration);
            services.AddHangfire(options => {
                options.UseRedisStorage(connection, new RedisStorageOptions { Prefix = "HANGFIRE" });
                options.UseConsole();
            });
            services.AddHangfireServer();
        }
    }
}
