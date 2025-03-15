using System.Text.RegularExpressions;

namespace Astronomic_Catalogs.Helpers;

public static class HtmlHelperExtensions
{
    public static string FixReferences(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        string pattern = @"<a\s+refstr=([^ ]+)\s+href=([^ ]+)\s+target=([^>]+)>(.*?)</a>";
        string replacement = @"<a href=""$2"" target=""_blank"">$4</a>";

        return Regex.Replace(input, pattern, replacement);
    }
}
