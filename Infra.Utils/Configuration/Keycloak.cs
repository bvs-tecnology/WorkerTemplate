namespace Infra.Utils.Configuration;

public class Keycloak
{
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public string? ClientId { get; set; }
    public string? RealmId { get; set; }
}