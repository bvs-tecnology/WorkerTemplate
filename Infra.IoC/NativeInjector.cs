using System.Diagnostics.CodeAnalysis;
using Application;
using Infra.Data;
using Infra.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.IoC
{
    [ExcludeFromCodeCoverage]
    public static class NativeInjector
    {
        public static IServiceCollection InjectDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .InjectData(configuration)
                .InjectHttp(configuration)
                .InjectApplication();
            
            return services;
        }
    }
}
