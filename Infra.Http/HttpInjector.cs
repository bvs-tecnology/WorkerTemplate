using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.Http;

public static class HttpInjector
{
    public static IServiceCollection InjectHttp(this IServiceCollection services, IConfiguration configuration)
    {
        services.InjectCrossCutting(configuration);
        return services;
    }
    
    private static IServiceCollection InjectCrossCutting(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}