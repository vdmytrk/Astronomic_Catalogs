namespace Astronomic_Catalogs.Entities;

public class Exoplanet
{
    public int Id { get; set; } // Sequential number in planetary system.
    public string Hostname { get; set; } = string.Empty;
    public string PlLetter { get; set; } = string.Empty;
    public decimal? PlRade { get; set; } // pl_rade
    public decimal? PlRadJ { get; set; } // pl_radj
    public decimal? PlMasse { get; set; } // pl_masse
    public decimal? PlMassJ { get; set; } // pl_massj
    public decimal? PlOrbsmax { get; set; } // pl_orbsmax - The longest radius of an elliptic orbit, or, for exoplanets detected via gravitational microlensing or direct imaging, the projected separation in the plane of the sky">Orbit Semi-Major Axis [au]

}
