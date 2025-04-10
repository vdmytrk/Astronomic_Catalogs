using System.Runtime.InteropServices;

namespace Astronomic_Catalogs.Infrastructure.LogingIfrastructure;

public static class FileLogService
{
    public const string logFilePath = "C:\\home\\LogFiles\\log.txt";

    public static string GetKyivTime()
    {
        string timeZoneId = string.Empty;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // "Ukraine Standard Time" - for Windows 7 - 11
            // "FLE Standard Time" - for latest versions of Windows
            bool hasUkraineStandardTime = TimeZoneInfo.GetSystemTimeZones().Any(tz => tz.Id == "Ukraine Standard Time");
            timeZoneId = hasUkraineStandardTime ? "Ukraine Standard Time" : "FLE Standard Time";
        }
        else
        {
            timeZoneId = "Europe/Kyiv"; // For Linux/macOS 
        }

        TimeZoneInfo kyivTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        DateTime kyivTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, kyivTimeZone);
        return kyivTime.ToString();
    }


    public static void LogAllConfiguration(IConfiguration configuration)
    {
        var allValues = new List<string>();
        LogConfigurationRecursive(configuration, allValues, parentPath: "");

        File.AppendAllText(logFilePath, $"\n\n\n[DEBUG] {new string('=', 30)} FULL CONFIG DUMP at {GetKyivTime()} {new string('=', 30)}\n");
        foreach (var entry in allValues)
        {
            File.AppendAllText(logFilePath, entry + "\n");
        }
    }

    private static void LogConfigurationRecursive(IConfiguration configSection, List<string> output, string parentPath)
    {
        foreach (var child in configSection.GetChildren())
        {
            string currentPath = string.IsNullOrEmpty(parentPath) ? child.Key : $"{parentPath}:{child.Key}";

            if (child.GetChildren().Any())
            {
                LogConfigurationRecursive(child, output, currentPath);
            }
            else
            {
                output.Add($"[DEBUG] {currentPath} = {child.Value}");
            }
        }
    }

    public static void WriteLogInFile (string? type = null, string? parameterName = null, string? parameterValue = null)
    {
#if DEBUG 
        type ??= "UNKNOWN";
        parameterName ??= string.Empty;
        parameterValue ??= string.Empty;
        File.AppendAllText(logFilePath, $"[{type}] {parameterName} = {parameterValue}\n");
#endif
    }
}
