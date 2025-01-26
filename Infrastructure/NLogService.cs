using NLog.Targets;
using NLog;

namespace Astronomic_Catalogs.Infrastructure;

public class NLogService
{
    // Set the actual connection string in the NLog.config.json file
    public void UpdateNLogDatabaseConnectionString(string connectionString)
    {
        var config = LogManager.Configuration;
        var databaseTarget = config.FindTargetByName<DatabaseTarget>("database");
        if (databaseTarget != null)
        {
            databaseTarget.ConnectionString = connectionString;
            LogManager.ReconfigExistingLoggers();
        }
    }
}
