using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Astronomic_Catalogs.Services.Interfaces
{
    public interface IPlanetFilterService
    {
        Task<List<NASAExoplanetCatalog>?> GetFilteredDataAsync(Dictionary<string, object> parameters);
        Task<(List<SelectListItem> plLetters, List<SelectListItem> telescopes, List<SelectListItem> discoveryMethods)> GetCatalogStatsAsync();
    }
}
