using Astronomic_Catalogs.DTO;
using Astronomic_Catalogs.Models;

namespace Astronomic_Catalogs.Results;

public class PlanetsResult
{
    public List<NASAExoplanetCatalog> Systems { get; set; } = new();
    public int TotalCount { get; set; }
}
