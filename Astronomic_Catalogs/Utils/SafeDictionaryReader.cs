using System.Text.Json;

namespace Astronomic_Catalogs.Utils;

public static class SafeDictionaryReader 
{
    public static int? GetInt(this Dictionary<string, object> parameters, string key) =>
        parameters.TryGetValue(key, out var val) && int.TryParse(val?.ToString(), out var result) ? result : null;

    public static double? GetDouble(this Dictionary<string, object> parameters, string key) =>
        parameters.TryGetValue(key, out var val) && double.TryParse(val?.ToString(), out var result) ? result : null;

    public static bool GetBool(this Dictionary<string, object> parameters, string key) =>
        parameters.TryGetValue(key, out var val) && bool.TryParse(val?.ToString(), out var result) ? result : false;

    public static string? GetString(this Dictionary<string, object> parameters, string key) =>
        parameters.TryGetValue(key, out var val) && val is JsonElement el && el.ValueKind == JsonValueKind.String ? el.GetString() : val?.ToString();

    public static DateTime? GetDateTime(this Dictionary<string, object> parameters, string key) =>
        parameters.TryGetValue(key, out var val) && DateTime.TryParse(val?.ToString(), out var result) ? result : null;

}
