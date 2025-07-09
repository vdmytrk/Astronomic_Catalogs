using System.ComponentModel.DataAnnotations;

namespace Astronomic_Catalogs.Models;

public class Constellation
{
    public string ShortName { get; set; } = string.Empty; // For clear code without squiggle.
    public string LatineNameNominativeCase { get; set; } = string.Empty;
    public string LatineNameGenitiveCase { get; set; } = string.Empty;
    public string EnglishName { get; set; } = string.Empty;
    public string UkraineName { get; set; } = string.Empty;
    public int Area { get; set; }
    public int NumberStars { get; set; }
}
