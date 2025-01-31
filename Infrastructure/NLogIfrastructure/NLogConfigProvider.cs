using NLog;
using NLog.Targets;

namespace Astronomic_Catalogs.Infrastructure.NLogIfrastructure;



public class NLogConfigProvider : INLogConfigProvider
{
    private readonly INLogConfiguration _configurationStrategy;
    private readonly ConnectionStringProvider _connectionStringProvider;
    private readonly string _nlogConfigFile;

    public NLogConfigProvider(INLogConfiguration configurationStrategy,
                       ConnectionStringProvider connectionStringProvider,
                       IWebHostEnvironment environment)
    {
        _configurationStrategy = configurationStrategy;
        _connectionStringProvider = connectionStringProvider;

        _nlogConfigFile = environment.IsDevelopment()
            ? "NLog.config.Debug.xml"
            : "NLog.config.Release.xml";
    }

    public void ConfigureLogger()
    {
        _configurationStrategy.ConfigureNLog(_nlogConfigFile);
        UpdateNLogDatabaseConnectionString(_connectionStringProvider.ConnectionString);
    }

    public void UpdateNLogDatabaseConnectionString(string connectionString)
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

