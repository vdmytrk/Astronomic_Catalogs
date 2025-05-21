using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Astronomic_Catalogs.DTO;

[Keyless]
public class PlanetarySystem 
{
    [Key]
    public string Hostname { get; set; } = string.Empty;
    public string HdName { get; set; } = string.Empty;
    public string HipName { get; set; } = string.Empty;
    public string TicId { get; set; } = string.Empty;
    public string GaiaId { get; set; } = string.Empty;
    public string StSpectype { get; set; } = string.Empty; // st_spectype
    public float? StTeff { get; set; } // st_teff - Temperature of the star
    public float? StRad { get; set; } // st_rad - Stellar Radius in [Solar Radius]
    public float? StMass { get; set; } // st_mass - Stellar Mass in [Solar mass]
    public float? StMet { get; set; } // st_met - Measurement of the metal content of the photosphere of the star as compared to the hydrogen content > Stellar Metallicity [dex]
    public string StMetratio { get; set; } = string.Empty; // st_metratio - Ratio for the Metallicity Value ([Fe/H] denotes iron abundance, [M/H] refers to a general metal content) > Stellar Metallicity Ratio
    public float? StLum { get; set; } // st_lum - Temperature of the star
    public float? StAge { get; set; } // st_age [Gyr]
    public float? SyDist { get; set; } // sy_dist - Distance to the planetary system in units of parsecs">Distance [pc]
    public List<Exoplanet> Exoplanets { get; set; } = new ();


    public int? PageNumber { get; set; }
    public int? PageCount { get; set; }

}
