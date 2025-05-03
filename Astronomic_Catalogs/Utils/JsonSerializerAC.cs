using System.Text.Json;

namespace Astronomic_Catalogs.Utils;

internal static class JsonSerializerAC
{
    internal static string? SerializeToNormalizedJson(object obj)
    {
        if (obj is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.String)
            {
                var str = jsonElement.GetString();
                return JsonSerializer.Serialize(new[] { str });
            }
            else if (jsonElement.ValueKind == JsonValueKind.Array)
            {
                var listJson = jsonElement.EnumerateArray()
                    .Select(e => e.GetString())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();

                return listJson.Any() ? JsonSerializer.Serialize(listJson) : null;
            }
        }

        if (obj is string strValue && !string.IsNullOrWhiteSpace(strValue))
        {
            return JsonSerializer.Serialize(new[] { strValue });
        }

        if (obj is IEnumerable<string> list && list.Any())
        {
            return JsonSerializer.Serialize(list);
        }

        return null;
    }
}
