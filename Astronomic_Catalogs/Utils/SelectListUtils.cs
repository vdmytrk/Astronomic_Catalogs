using Microsoft.AspNetCore.Mvc.Rendering;

namespace Astronomic_Catalogs.Utils;

public static class SelectListUtils
{
    public static List<SelectListItem> FromStrings(IEnumerable<string> values)
    {
        return values
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .OrderBy(x => x)
            .Select(x => new SelectListItem { Value = x, Text = x })
            .ToList();
    }
}
