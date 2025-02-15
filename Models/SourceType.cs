using System.ComponentModel.DataAnnotations;

namespace Astronomic_Catalogs.Models;

public class SourceType
{
    public int Id { get; set; }
    public int? Count { get; set; }
    [Display(Name = "Скорочення")] 
    public string? Code { get; set; }
    [Display(Name = "Пояснення")] 
    public string? Meaning { get; set; }
}
