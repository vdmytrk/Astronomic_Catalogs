using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Astronomic_Catalogs.Utils;

static class DictionaryExtensions 
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

    public static string ToCacheKey(this Dictionary<string, object> parameters, string prefix = "CacheKey")
    {
        if (parameters == null || parameters.Count == 0)
            return $"{prefix}_empty";

        var sorted = parameters
            .OrderBy(kv => kv.Key)
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        string json = JsonSerializer.Serialize(sorted);

        using var sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(json));
        string hash = Convert.ToHexString(hashBytes); 

        return $"{prefix}_{hash}";
    }
} 
