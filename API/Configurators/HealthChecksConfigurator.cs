using Infra.Utils.Configuration;

namespace API.Configurators;

public static class HealthChecksConfigurator
{
    public static IServiceCollection AddLocalHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(Builders.BuildPostgresConnectionString(configuration))
            .AddRedis(Builders.BuildRedisConnectionString(configuration));
        return services;
    }
}