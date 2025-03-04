namespace Astronomic_Catalogs.Infrastructure;

public class AuthenticationSettingsProvider
{
    public string GoogleClientId { get; }
    public string GoogleClientSecret { get; }
    public string MicrosoftClientId { get; }
    public string MicrosoftClientSecret { get; }
    public string JwtKey { get; }
    public string JwtIssuer { get; }
    public string JwtAudience { get; }
    public int JwtExpireMinutes { get; }

    public AuthenticationSettingsProvider(IConfiguration configuration)
    {

        GoogleClientId = GetConfigValue(configuration, "Authentication:Google:ClientId");
        GoogleClientSecret = GetConfigValue(configuration, "Authentication:Google:ClientSecret");
        MicrosoftClientId = GetConfigValue(configuration, "Authentication:Microsoft:ClientId");
        MicrosoftClientSecret = GetConfigValue(configuration, "Authentication:Microsoft:ClientSecret");
        JwtKey = GetConfigValue(configuration, "JwtSettings:Key");
        JwtIssuer = GetConfigValue(configuration, "JwtSettings:Issuer");
        JwtAudience = GetConfigValue(configuration, "JwtSettings:Audience");
        JwtExpireMinutes = int.Parse(GetConfigValue(configuration, "JwtSettings:ExpireMinutes"));
    }

    private string GetConfigValue(IConfiguration configuration, string key)
    {
        var azureKey = key + "Azure";
        var azureValue = Environment.GetEnvironmentVariable(azureKey) ?? configuration[azureKey];
        return !string.IsNullOrEmpty(azureValue) ? azureValue : configuration[key]
            ?? throw new InvalidOperationException($"Configuration key '{key}' not found.");
    }
}
