using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Domain.Entities.Dtos;
using Infra.Security.Constants;
using Infra.Utils.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infra.Security.Services;

public class JwtService(IOptionsSnapshot<AppSettings> appSettings) : IJwtService
{
    private readonly AppSettings _appSettings = appSettings.Value;
    private static JsonSerializerOptions jsonOptions
        => new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

    public string CreateToken(UserDto user)
    {
        var secret = _appSettings.Jwt?.Secret!;
        var expirationTime = Convert.ToInt32(_appSettings.Jwt?.SessionExpirationHours);

        var signinKey = new SigningCredentials(
            new SymmetricSecurityKey(Convert.FromBase64String(secret)),
            SecurityAlgorithms.HmacSha256
        );

        var tokenConfig = new SecurityTokenDescriptor
        {
            Subject = GetClaims(user),
            Expires = DateTime.UtcNow.AddHours(expirationTime),
            SigningCredentials = signinKey
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenConfig);

        return tokenHandler.WriteToken(token);
    }

    private static ClaimsIdentity GetClaims(UserDto user)
    {
        var identity = new ClaimsIdentity("JWT");
        identity.AddClaim(new Claim(JwtClaims.ClaimUserProfile, JsonSerializer.Serialize(user, jsonOptions)));
        identity.AddClaim(new Claim(JwtClaims.ClaimScopes, user.Role.ToString()));
        return identity;
    }
}