namespace Astronomic_Catalogs.Models;

public class CollinderCatalog
{
    public int Id { get; set; }
    public required string NamberName { get; set; }
    public string? NameOtherCat { get; set; }
    public string? Constellation { get; set; }


    public string? RightAscension { get; set; }
    public int RightAscensionH { get; set; }
    public double RightAscensionM { get; set; }
    public double RightAscensionS { get; set; }


    public string? Declination { get; set; }
    public string? NS { get; set; }
    public int DeclinationD { get; set; }
    public int DeclinationM { get; set; }
    public double DeclinationS { get; set; }


    public double? AppMag { get; set; }
    public string? AppMagFlag { get; set; }
    public string? CountStars { get; set; }
    public int? CountStarsToFinding { get; set; }
    public string? AngDiameter { get; set; }
    public double? AngDiameterNew { get; set; }
    public string? Class { get; set; }
    public string? Comment { get; set; }


    public int? RowOnPage { get; set; }
}
