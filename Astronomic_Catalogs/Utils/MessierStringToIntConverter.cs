using AutoMapper;

namespace Astronomic_Catalogs.Utils;

public class MessierStringToIntConverter : IValueConverter<string?, int?>
{
    public int? Convert(string? sourceMember, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(sourceMember))
            return null;

        var digitsOnly = sourceMember.Replace("M", "").Trim();
        return int.TryParse(digitsOnly, out var val) ? val : null;
    }
}
