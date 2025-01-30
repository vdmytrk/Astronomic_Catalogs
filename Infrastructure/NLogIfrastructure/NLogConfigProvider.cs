using NLog;
using NLog.Targets;

namespace Astronomic_Catalogs.Infrastructure.NLogIfrastructure;



public class NLogConfigProvider : INLogConfigProvider
{
    private readonly INLogConfiguration _nlogConfiguration;
    private readonly ConnectionStringProvider _connectionStringProvider;
    private readonly string _nlogConfigFile;

    public NLogConfigProvider(INLogConfiguration nlogConfiguration,
                       ConnectionStringProvider connectionStringProvider,
                       IWebHostEnvironment environment)
    {
        _nlogConfiguration = nlogConfiguration;
        _connectionStringProvider = connectionStringProvider;

        _nlogConfigFile = environment.IsDevelopment()
            ? "NLog.config.Debug.xml"
            : "NLog.config.Release.xml";
    }

    public void ConfigureLogger()
    {
        _nlogConfiguration.ConfigureNLog(_nlogConfigFile);
        UpdateNLogDatabaseConnectionString(_connectionStringProvider.ConnectionString);
    }

    private void UpdateNLogDatabaseConnectionString(string connectionString)
    {
        var config = LogManager.Configuration ?? throw new InvalidOperationException("NLog configuration not loaded.");
        var databaseTarget = config.FindTargetByName<DatabaseTarget>("database");
        if (databaseTarget != null)
        {
            databaseTarget.ConnectionString = connectionString;
        }
        Console.WriteLine($"\n\nNLogService connection string in {_nlogConfigFile}:\n{connectionString}\n\n");
        LogManager.ReconfigExistingLoggers();
    }
}

