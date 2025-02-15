using NLog;
using NLog.Targets;

namespace Astronomic_Catalogs.Infrastructure.NLogIfrastructure;

public class NLogService
{
    public void UpdateNLogDatabaseConnectionString(string connectionString)
    {
        var config = LogManager.Configuration;
        var databaseTarget = config.FindTargetByName<DatabaseTarget>("Database");
        if (databaseTarget != null)
        {
            databaseTarget.ConnectionString = connectionString;
            LogManager.ReconfigExistingLoggers();
        }
    }
}
