using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Infrastructure;

public class ConnectionStringProvider
{
    public string ConnectionString { get; }

    public ConnectionStringProvider(IConfiguration configuration)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables();
        var azureConnectionString = Environment.GetEnvironmentVariable("DefaultConnectionAzure");

        if (!string.IsNullOrEmpty(azureConnectionString))
        {
            ConnectionString = azureConnectionString;
        }
        else
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }
    }
}