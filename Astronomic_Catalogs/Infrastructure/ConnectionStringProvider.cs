using Astronomic_Catalogs.Infrastructure.Interfaces;
using Astronomic_Catalogs.Infrastructure.LogingIfrastructure;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Infrastructure;

public class ConnectionStringProvider : IConnectionStringProvider
{
    public string ConnectionString { get; }

    public ConnectionStringProvider(IConfiguration configuration)
    {
        ConnectionString = configuration["DefaultConnection"]
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        FileLogService.WriteLogInFile("\n\n\nDEBUG", "ConnectionString", $"{ConnectionString}\n\n");
    }

}