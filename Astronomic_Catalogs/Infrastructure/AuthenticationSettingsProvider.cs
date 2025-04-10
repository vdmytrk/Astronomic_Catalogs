using Astronomic_Catalogs.Infrastructure.Configuration;
using Astronomic_Catalogs.Infrastructure.LogingIfrastructure;
using Astronomic_Catalogs.Models.Services;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Astronomic_Catalogs.Infrastructure;


/// <summary>
/// Because JSON uses ':', but Key Vault uses '-'.
/// </summary>
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

    private readonly IConfiguration _configuration;
    private readonly bool _isUsingKeyVault;

    public AuthenticationSettingsProvider(IConfiguration configuration)
    {
        string? keyVaultUrl = Environment.GetEnvironmentVariable("AZURE_KEYVAULT_URL");
        FileLogService.WriteLogInFile("DEBUG", $"{ new string(' ', 10)}{new string('=', 20)} {FileLogService.GetKyivTime()} {new string('=', 20)}");

        _configuration = configuration;
        _isUsingKeyVault = !string.IsNullOrEmpty(keyVaultUrl);

        AuthMessageSenderOptions = GetConfigSection<AuthMessageSenderOptions>("AuthMessageSenderOptions");

        GoogleClientId = GetConfigValue(ConfigKeys.GoogleClientId);
        GoogleClientSecret = GetConfigValue(ConfigKeys.GoogleClientSecret);

        MicrosoftClientId = GetConfigValue(ConfigKeys.MicrosoftClientId);
        MicrosoftClientSecret = GetConfigValue(ConfigKeys.MicrosoftClientSecret);

        JwtKey = GetConfigValue(ConfigKeys.JwtKey);
        JwtIssuer = GetConfigValue(ConfigKeys.JwtIssuer);
        JwtAudience = GetConfigValue(ConfigKeys.JwtAudience);
        JwtExpireMinutes = GetConfigValue<int>(ConfigKeys.JwtExpireMinutes);

        MinAgeRestriction = GetConfigValue<int>(ConfigKeys.MinAge);
    }

    private string GetConfigValue(string key)
    {
        string resolvedKey = ResolveKey(key);
        var value = _configuration[resolvedKey];
        FileLogService.WriteLogInFile("DEBUG", key, value);

        return value ?? throw new InvalidOperationException($"Configuration key '{key}' not found.");
    }

    private T GetConfigValue<T>(string key)
    {
        string resolvedKey = ResolveKey(key);
        var value = _configuration[resolvedKey];
        FileLogService.WriteLogInFile("DEBUG", key, value);
        value = value ?? throw new InvalidOperationException($"Configuration key '{key}' not found.");

        return (T)Convert.ChangeType(value, typeof(T));
    }

    private T GetConfigSection<T>(string sectionName) where T : new ()
    {
        T? section = new T();

        if (_isUsingKeyVault)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                string fullKey = $"{sectionName}-{property.Name}";
                string? rawValue = _configuration[fullKey];

                FileLogService.WriteLogInFile("DEBUG SECTION", fullKey, rawValue);

                if (rawValue != null)
                {
                    try
                    {
                        object? convertedValue = Convert.ChangeType(rawValue, property.PropertyType);
                        property.SetValue(section, convertedValue);
                    }
                    catch (Exception ex)
                    {
                        FileLogService.WriteLogInFile("ERROR", fullKey, ex.Message);
                        throw new InvalidOperationException($"Failed to set property '{property.Name}' from key '{fullKey}'.", ex);
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Key '{fullKey}' not found in configuration.");
                }
            }
        }
        else
        {
            section = _configuration.GetSection(sectionName).Get<T>();
            FileLogService.WriteLogInFile("DEBUG SECTION", sectionName, section!.ToString());
        }  

        return section ?? throw new InvalidOperationException($"Configuration section '{sectionName}' is missing.");
    }

    private string ResolveKey(string flatKey)
    {
        return _isUsingKeyVault ? flatKey : flatKey.Replace('-', ':');
    }

}

