using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.ViewModels;

namespace Astronomic_Catalogs.Services.Interfaces;

public interface ICollinderFilterService
{
    Task<List<CollinderCatalog>?> GetFilteredDataAsync(Dictionary<string, object> parameters);
    Task<(int count, List<CollinderCatalog> rawData, List<ConstellationViewModel> constellations)> GetCollinderCatalogDataAsync();
}
