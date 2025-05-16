using Astronomic_Catalogs.Models;

namespace Astronomic_Catalogs.Services.Interfaces
{
    public interface IPlanetFilterService
    {
        Task<List<NASAExoplanetCatalog>?> GetFilteredDataAsync(Dictionary<string, object> parameters);
    }
}
