namespace Astronomic_Catalogs.Infrastructure.NLogIfrastructure;

public interface INLogConfigProvider
{
    void ConfigureNLog();
    void UpdateNLogDatabaseConnectionString(string connectionString);
}
