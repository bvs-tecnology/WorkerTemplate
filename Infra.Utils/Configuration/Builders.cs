using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Npgsql;
using StackExchange.Redis;

namespace Infra.Utils.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class Builders
    {
        public static string BuildPostgresConnectionString(IConfiguration configuration)
        {
            var connBuilder = new NpgsqlConnectionStringBuilder(configuration.GetConnectionString("Postgres"))
            {
                PersistSecurityInfo = true,
                Pooling = true,
                CommandTimeout = 15
            };
            return connBuilder.ConnectionString;
        }

        public static string BuildRedisConnectionString(IConfiguration configuration)
        {
            var redisOptions = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis")!);
            redisOptions.AbortOnConnectFail = false;
            redisOptions.ConnectRetry = 5;
            redisOptions.ConnectTimeout = 5000;
            redisOptions.SyncTimeout = 5000;
            redisOptions.AllowAdmin = true;
            redisOptions.DefaultDatabase = 0;

            return redisOptions.ToString();
        }
    }
}
