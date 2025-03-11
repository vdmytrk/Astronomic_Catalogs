using Astronomic_Catalogs.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Infrastructure;

public class ConnectionStringProvider : IConnectionStringProvider
{
    public string ConnectionString { get; }

    public ConnectionStringProvider(IConfiguration configuration)
    {
        ConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

}