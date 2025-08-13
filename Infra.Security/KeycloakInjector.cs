using System.Security.Claims;
using Infra.Utils.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infra.Security;

public static class KeycloakInjector
{
    public static IServiceCollection AddLocalAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Keycloak>(configuration.GetSection("Keycloak"));
        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.Authority = configuration["Keycloak:Issuer"];
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = configuration["Keycloak:Audience"],
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Keycloak:Issuer"],
                    NameClaimType = "preferred_username",
                    RoleClaimType = ClaimTypes.Role
                };
            });
        services.AddAuthorization();
        return services;
    }
}