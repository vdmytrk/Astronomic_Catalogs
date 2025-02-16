using System.ComponentModel.DataAnnotations;

namespace Astronomic_Catalogs.Models;

public class CollinderCatalog
{
    public int Id { get; set; }
    [Display(Name = "Об'єкт №")] 
    public required string NamberName { get; set; }
    [Display(Name = "Ім'я в інших каталогах")] 
    public string? NameOtherCat { get; set; }
    [Display(Name = "Сузір'я")]
    public string? Constellation { get; set; }


    [Display(Name = "RA")] 
    public string? RightAscension { get; set; }
    public int RightAscensionH { get; set; }
    public float RightAscensionM { get; set; }
    public float RightAscensionS { get; set; }


    [Display(Name = "Схилення")] 
    public string? Declination { get; set; }
    public string? NS { get; set; }
    public int DeclinationD { get; set; }
    public int DeclinationM { get; set; }
    public float DeclinationS { get; set; }


    [Display(Name = "Видима зоряна величина")] 
    public float? AppMag { get; set; }
    public string? AppMagFlag { get; set; }
    [Display(Name = "Кількісь зір...")] 
    public string? CountStars { get; set; }
    public int? CountStarsToFinding { get; set; }
    public string? AngDiameterOld { get; set; }
    [Display(Name = "Розмір в '")] 
    public float? AngDiameterNew { get; set; }
    [Display(Name = "Класифікація")] 
    public string? Class { get; set; }
    [Display(Name = "Коментар")] 
    public string? Comment { get; set; }


    public int? PageNumber { get; set; }
    public int? PageCount { get; set; }
}
