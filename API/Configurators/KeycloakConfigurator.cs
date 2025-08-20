using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Infra.Utils.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Configurators;
[ExcludeFromCodeCoverage]
public static class KeycloakConfigurator
{
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
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