namespace Astronomic_Catalogs.Models;

public class PlanetarySystem
{
    public string Hostname { get; set; } = string.Empty;
    public string StSpectype { get; set; } = string.Empty; // st_spectype
    public string StTeff { get; set; } = string.Empty; // st_teff - Temperature of the star
    public string StRad { get; set; } = string.Empty; // st_rad - Stellar Radius in [Solar Radius]
    public string StMass { get; set; } = string.Empty; // st_mass - Stellar Mass in [Solar mass]
    public string StMet { get; set; } = string.Empty; // st_met - Measurement of the metal content of the photosphere of the star as compared to the hydrogen content > Stellar Metallicity [dex]
    public string StMetratio { get; set; } = string.Empty; // st_metratio - Ratio for the Metallicity Value ([Fe/H] denotes iron abundance, [M/H] refers to a general metal content) > Stellar Metallicity Ratio
    public string StLum { get; set; } = string.Empty; // st_lum - Temperature of the star
    public string StAge { get; set; } = string.Empty; // st_age [Gyr]
    public string StDist { get; set; } = string.Empty; // sy_dist - Distance to the planetary system in units of parsecs">Distance [pc]
    public List<Exoplanet> exoplanets { get; set; } = new ();


    public int? PageNumber { get; set; }
    public int? PageCount { get; set; }

}
