using System.ComponentModel.DataAnnotations;

namespace Astronomic_Catalogs.Models;

public class Constellation
{
    [Display(Name = "Скороченння")] 
    public string ShortName { get; set; } = string.Empty; // For clear code without squiggle.
    [Display(Name = "Лат. називний")] 
    public string LatineNameNominativeCase { get; set; } = string.Empty;
    [Display(Name = "Лат. родовий")] 
    public string LatineNameGenitiveCase { get; set; } = string.Empty;
    [Display(Name = "Сузір'я")] 
    public string UkraineName { get; set; } = string.Empty;
    [Display(Name = "Площа, °")] 
    public int Area { get; set; }
    [Display(Name = "К-сть зір")] 
    public int NumberStars { get; set; }
}
