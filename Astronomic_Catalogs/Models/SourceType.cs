using System.ComponentModel.DataAnnotations;

namespace Astronomic_Catalogs.Models;

public class SourceType
{
    public int Id { get; set; }
    public int? Count { get; set; }
    public string? Code { get; set; }
    public string? Meaning { get; set; }
}
