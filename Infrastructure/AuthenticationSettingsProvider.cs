using Astronomic_Catalogs.Models.Services;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.Extensions.Options;

namespace Astronomic_Catalogs.Infrastructure;

public class AuthenticationSettingsProvider
{
    public AuthMessageSenderOptions AuthMessageSenderOptions { get; }
    public string GoogleClientId { get; }
    public string GoogleClientSecret { get; }
    public string MicrosoftClientId { get; }
    public string MicrosoftClientSecret { get; }
    public string JwtKey { get; }
    public string JwtIssuer { get; }
    public string JwtAudience { get; }
    public int JwtExpireMinutes { get; }
    public int MinAgeRestriction { get; }

    public AuthenticationSettingsProvider(IConfiguration configuration)
    {
        AuthMessageSenderOptions = GetConfigSection<AuthMessageSenderOptions>(configuration, "AuthMessageSenderOptions");

        GoogleClientId = GetConfigValue(configuration, "Authentication:Google:ClientId");
        GoogleClientSecret = GetConfigValue(configuration, "Authentication:Google:ClientSecret");

        MicrosoftClientId = GetConfigValue(configuration, "Authentication:Microsoft:ClientId");
        MicrosoftClientSecret = GetConfigValue(configuration, "Authentication:Microsoft:ClientSecret");

        JwtKey = GetConfigValue(configuration, "JwtSettings:Key");
        JwtIssuer = GetConfigValue(configuration, "JwtSettings:Issuer");
        JwtAudience = GetConfigValue(configuration, "JwtSettings:Audience");
        JwtExpireMinutes = GetConfigValue<int>(configuration, "JwtSettings:ExpireMinutes");

        MinAgeRestriction = GetConfigValue<int>(configuration, "AgeRestriction:MinAge");
    }

    private string GetConfigValue(IConfiguration configuration, string key)
    {
        return configuration[key] ?? 
            throw new InvalidOperationException($"Configuration key '{key}' not found.");
    }

    private static T GetConfigValue<T>(IConfiguration configuration, string key)
    {
        var value = configuration[key] ?? 
            throw new InvalidOperationException($"Configuration key '{key}' not found.");

        return (T)Convert.ChangeType(value, typeof(T));
    }

    private T GetConfigSection<T>(IConfiguration configuration, string sectionName)
    {
        return configuration.GetSection(sectionName).Get<T>()
            ?? throw new InvalidOperationException($"Configuration section '{sectionName}' is missing.");
    }
}

