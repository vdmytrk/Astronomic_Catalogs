using Astronomic_Catalogs.DTO;
using Astronomic_Catalogs.Models;

namespace Astronomic_Catalogs.Services.Interfaces;

public interface ICollinderFilterService
{
    Task<List<CollinderCatalog>?> GetFilteredDataAsync(Dictionary<string, object> parameters);
    Task<(int count, List<CollinderCatalog> rawData, List<ConstellationDto> constellations)> GetCollinderCatalogDataAsync();
}
