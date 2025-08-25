namespace ITC.Authorization.Options;

public class KeycloakClientOptions
{
    [ConfigurationKeyName("auth-server-url")]
    public string Url { get; set; } = "";

    [ConfigurationKeyName("ssl-required")]
    public string SslRequired { get; set; } = "";

    [ConfigurationKeyName("verify-token-audience")]
    public bool VerifyTokenAudience { get; set; }

    [ConfigurationKeyName("confidential-port")]
    public int ConfidentialPort { get; set; }

    [ConfigurationKeyName("realm")]
    public string Realm { get; set; } = "";

    [ConfigurationKeyName("resource")]
    public string ClientName { get; set; } = "";

    [ConfigurationKeyName("credentials:secret")]
    public string ClientSecret { get; set; } = "";

    [ConfigurationKeyName("client-id")]
    public Guid ClientId { get; set; }
}