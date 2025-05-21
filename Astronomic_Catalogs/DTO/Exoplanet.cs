
namespace Astronomic_Catalogs.DTO;

public class Exoplanet
{
    public int Id { get; set; } // Sequential number in planetary system.
    public string Hostname { get; set; } = string.Empty;
    public string PlLetter { get; set; } = string.Empty;
    public float? PlRade { get; set; } // pl_rade
    public float? PlRadJ { get; set; } // pl_radj
    public float? PlMasse { get; set; } // pl_masse
    public float? PlMassJ { get; set; } // pl_massj
    public float? PlOrbsmax { get; set; } // pl_orbsmax - The longest radius of an elliptic orbit, or, for exoplanets detected via gravitational microlensing or direct imaging, the projected separation in the plane of the sky">Orbit Semi-Major Axis [au]

}
