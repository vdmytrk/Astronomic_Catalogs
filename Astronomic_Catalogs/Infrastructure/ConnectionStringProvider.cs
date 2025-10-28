using Astronomic_Catalogs.Infrastructure.Interfaces;
using Astronomic_Catalogs.Infrastructure.LogingIfrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using System.Text.Json;

namespace Astronomic_Catalogs.Infrastructure;


public class ConnectionStringProvider : IConnectionStringProvider
{
    public string ConnectionString { get; }
    private readonly IConfiguration _configuration;
    private readonly bool _isUsingKeyVault;
    private readonly string _environment;
    private readonly string _baseConnectionString;
    private static string? _targetDatabase;

    public ConnectionStringProvider(IConfiguration configuration)
    {
        string? keyVaultUrl = Environment.GetEnvironmentVariable("AZURE_KEYVAULT_URL");
        _isUsingKeyVault = !string.IsNullOrEmpty(keyVaultUrl);
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "AzureDevelopment";
        _baseConnectionString = configuration["DefaultConnection"]
            ?? _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        ConnectionString = ResolveConnectionString();
    }

    private string ResolveConnectionString()
    {
        string resolvedConnectionString = _environment switch
        {
            "AzureDevelopment" => BuildConnectionString(_baseConnectionString, _configuration, _isUsingKeyVault),
            "Testing" => _baseConnectionString,
            _ => _baseConnectionString,
        };

        LogResolvedConnectionString(resolvedConnectionString, _isUsingKeyVault);
        return resolvedConnectionString;
    }

    private static string BuildConnectionString(string baseConnectionString, IConfiguration configuration, bool isUsingKeyVault)
    {
        _targetDatabase = GetDatabaseName(configuration, isUsingKeyVault);
        return UpdateInitialCatalog(baseConnectionString);
    }

    private static string GetDatabaseName(IConfiguration configuration, bool isUsingKeyVault)
    {
        int day = DateTime.UtcNow.Day;

        if (!isUsingKeyVault) // JSON configuration
        {
            var section = configuration.GetSection("ConnectionDateRanges");
            if (!section.Exists())
                throw new InvalidOperationException("ConnectionDateRanges configuration section not found.");

            foreach (var item in section.GetChildren())
            {
                int[] range = item.Get<int[]>()!;
                if (range.Length == 2 && day >= range[0] && day <= range[1])
                    return item.Key;
            }

            return "AstroCatalogsDB";
        }
        else // KeyVault configuration
        {
            var ranges = configuration.AsEnumerable()
                .Where(kvp =>
                    !string.IsNullOrWhiteSpace(kvp.Value) &&
                    kvp.Key.StartsWith("AstroCatalogsDB-", StringComparison.OrdinalIgnoreCase))
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            if (ranges.Count == 0)
                throw new InvalidOperationException("No 'AstroCatalogsDB-XX' or ranged connections found in KeyVault configuration.");

            foreach (var kvp in ranges)
            {
                try
                {
                    FileLogService.WriteLogInFile("DEBUG", "ConnectionString", $"Day={DateTime.UtcNow.Day} → kvp = {kvp}\n");

                    var range = JsonSerializer.Deserialize<int[]>(kvp.Value!);
                    if (range != null && range.Length == 2 && day >= range[0] && day <= range[1])
                    {
                        string dbName = kvp.Key.Replace("-", "_", StringComparison.Ordinal);
                        return dbName;
                    }
                }
                catch (JsonException)
                {
                    continue; // Skip if any of the secrets is not an array
                }
            }

            return "AstroCatalogsDB";
        }

        throw new InvalidOperationException("No matching database found and 'AstroCatalogsDB' is missing in KeyVault configuration.");
    }

    private static string UpdateInitialCatalog(string baseConnectionString)
    {
        var builder = new SqlConnectionStringBuilder(baseConnectionString)
        {
            InitialCatalog = _targetDatabase
        };

        return builder.ConnectionString;
    }

    private static void LogResolvedConnectionString(string resolvedConnectionString, bool isUsingKeyVault)
    {
        if (!isUsingKeyVault)
        {
            FileLogService.WriteLogInFile(
                "DEBUG",
                "ConnectionString",
                $"Day={DateTime.UtcNow.Day} → Database={_targetDatabase}\n{resolvedConnectionString}\n\n\n");
        }
    }

}
