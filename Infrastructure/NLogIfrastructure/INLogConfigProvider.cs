namespace Astronomic_Catalogs.Infrastructure.NLogIfrastructure;

public interface INLogConfigProvider
{
    void ConfigureLogger();
    void UpdateNLogDatabaseConnectionString(string connectionString);
}
