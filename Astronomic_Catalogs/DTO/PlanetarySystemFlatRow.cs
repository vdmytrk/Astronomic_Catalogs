using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.DTO;

[Keyless]
public class PlanetarySystemFlatRow
{
    // System data
    public string Hostname { get; set; } = string.Empty;
    public string HdName { get; set; } = string.Empty;
    public string HipName { get; set; } = string.Empty;
    public string TicId { get; set; } = string.Empty;
    public string GaiaId { get; set; } = string.Empty;
    public string StSpectype { get; set; } = string.Empty;
    public decimal? StTeff { get; set; }
    public decimal? StRad { get; set; }
    public decimal? StMass { get; set; }
    public decimal? StMet { get; set; }
    public string StMetratio { get; set; } = string.Empty;
    public decimal? StLum { get; set; }
    public decimal? StAge { get; set; }
    public decimal? SyDist { get; set; }
    public decimal? StLumSunAbsol { get; set; }
    public decimal? HabitablZone { get; set; }

    // Planet data
    public string PlLetter { get; set; } = string.Empty;
    public decimal? PlRade { get; set; }
    public decimal? PlRadJ { get; set; }
    public decimal? PlMasse { get; set; }
    public decimal? PlMassJ { get; set; }
    public decimal? PlOrbsmax { get; set; }

    public int? RowOnPage { get; set; }
}

