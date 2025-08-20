using Infra.Data.Context;
using Infra.Utils.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Data;

public static class DataInjector
{
    public static IServiceCollection InjectData(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .InjectUnitOfWork(configuration)
            .InjectCache(configuration)
            .InjectRepositories();
        return services;
    }
    
    private static IServiceCollection InjectUnitOfWork(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<Context.Context>(options => 
            options.UseLazyLoadingProxies().UseNpgsql(Builders.BuildPostgresConnectionString(configuration)));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
    
    private static IServiceCollection InjectCache(this IServiceCollection services, IConfiguration configuration) {
        services.AddStackExchangeRedisCache(options => options.Configuration = Builders.BuildRedisConnectionString(configuration));
        return services;
    }
    
    private static IServiceCollection InjectRepositories(this IServiceCollection services)
    {
        return services;
    }
}