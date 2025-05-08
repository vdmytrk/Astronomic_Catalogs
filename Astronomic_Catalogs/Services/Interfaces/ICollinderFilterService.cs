using Astronomic_Catalogs.Models;

namespace Astronomic_Catalogs.Services.Interfaces;

public interface ICollinderFilterService
{
    Task<List<CollinderCatalog>> GetFilteredDataAsync(Dictionary<string, object> parameters);
}
