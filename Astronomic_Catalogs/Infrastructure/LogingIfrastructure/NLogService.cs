using NLog;
using NLog.Targets;

namespace Astronomic_Catalogs.Infrastructure.NLogIfrastructure;

public class NLogService
{
    public void UpdateNLogDatabaseConnectionString(string connectionString)
    {
        var config = LogManager.Configuration;
        if (config == null)
        {
            throw new InvalidOperationException("NLog configuration is not initialized.");
        }
        var databaseTarget = config.FindTargetByName<DatabaseTarget>("Database");
        if (databaseTarget != null)
        {
            databaseTarget.ConnectionString = connectionString;
            LogManager.ReconfigExistingLoggers();
        }
    }
}
