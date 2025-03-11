using Astronomic_Catalogs.Infrastructure.Interfaces;
using NLog;
using NLog.Targets;

namespace Astronomic_Catalogs.Infrastructure.NLogIfrastructure;



public class NLogConfigProvider : INLogConfigProvider
{
    private readonly INLogConfiguration _nlogConfiguration;
    private readonly IConnectionStringProvider _connectionStringProvider;
    private readonly string _nlogConfigFile;

    public NLogConfigProvider(INLogConfiguration nlogConfiguration,
                       IConnectionStringProvider connectionStringProvider,
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

    public void UpdateNLogDatabaseConnectionString(string connectionString)
    {
        var config = LogManager.Configuration ?? throw new InvalidOperationException("NLog configuration not loaded.");
        var databaseTarget = config.FindTargetByName<DatabaseTarget>("database");
        if (databaseTarget != null)
        {
            databaseTarget.ConnectionString = connectionString;
        }
        LogManager.ReconfigExistingLoggers();
    }
}

