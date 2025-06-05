using Astronomic_Catalogs.Entities;

namespace Astronomic_Catalogs.Results;

public class PlanetarySystemResult
{
    public List<PlanetarySystem> Systems { get; set; } = new();
    public int TotalCount { get; set; }
}
