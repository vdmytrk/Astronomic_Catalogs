using NLog;
using NLog.Web;


namespace Astronomic_Catalogs.Infrastructure.NLogIfrastructure;

public class NLogConfiguration : INLogConfiguration
{
    public void ConfigureNLog(string configFilePath)
    {
        if (!File.Exists(configFilePath))
        {
            throw new FileNotFoundException($"NLog configuration file not found: {configFilePath}");
        }

        LogManager.Setup()
                  .SetupExtensions(ext => ext.RegisterNLogWeb())
                  .LoadConfigurationFromFile(configFilePath, optional: false);
    }
}
