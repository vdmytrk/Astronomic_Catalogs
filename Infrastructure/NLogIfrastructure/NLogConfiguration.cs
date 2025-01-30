using NLog;
using NLog.Web;


namespace Astronomic_Catalogs.Infrastructure.NLogIfrastructure;

public class NLogConfiguration : INLogConfiguration
{
    public void ConfigureLogger(string configFilePath)
    {
        LogManager.Setup()
                  .SetupExtensions(ext => ext.RegisterNLogWeb())
                  .LoadConfigurationFromFile(configFilePath, optional: false);
    }
}
