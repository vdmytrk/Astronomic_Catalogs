namespace Astronomic_Catalogs.Models;

public class NGCICOpendatasoftExtension
{
    public int Id { get; set; }
    public string? NGC_IC { get; set; }
    public int? Name { get; set; }
    public string? SubObject { get; set; }
    public string? Messier { get; set; }
    public string? Name_UK { get; set; }
    public string? Comment { get; set; }
    public string? OtherNames { get; set; }
    public string? NGC { get; set; }
    public string? IC { get; set; }
    public string? LimitAngDiameter { get; set; }
    public string? AngDiameter { get; set; }
    public string? ObjectTypeAbrev { get; set; }
    public string? ObjectType { get; set; }
    public string? ObjectTypeFull { get; set; }
    public string? SourceType { get; set; }


    public string? RA { get; set; }
    public string? RightAscension { get; set; }
    public int? RightAscensionH { get; set; }
    public int? RightAscensionM { get; set; }
    public double? RightAscensionS { get; set; }


    public string? DEC { get; set; }
    public string? Declination { get; set; }
    public string? NS { get; set; }
    public int? DeclinationD { get; set; }
    public int? DeclinationM { get; set; }
    public double? DeclinationS { get; set; }


    public string? Constellation { get; set; }
    public double? MajorAxis { get; set; }
    public double? MinorAxis { get; set; }
    public int? PositionAngle { get; set; }


    public double? AppMag { get; set; }
    public string? AppMagFlag { get; set; }
    public double? BMag { get; set; }
    public double? VMag { get; set; }
    public double? JMag { get; set; }
    public double? HMag { get; set; }
    public double? KMag { get; set; }


    public double? SurfaceBrightness { get; set; }
    public string? HubbleOnlyGalaxies { get; set; }
    public double? CstarUMag { get; set; }
    public double? CstarBMag { get; set; }
    public double? CstarVMag { get; set; }
    public string? CstarNames { get; set; }
    public string? CommonNames { get; set; }
    public string? NedNotes { get; set; }
    public string? OpenngcNotes { get; set; }
    public string? Image { get; set; }


    public int? PageNumber { get; set; }
    public int? PageCount { get; set; }
}
