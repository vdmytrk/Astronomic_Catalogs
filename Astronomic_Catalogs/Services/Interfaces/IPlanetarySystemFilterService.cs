using Astronomic_Catalogs.Entities;
using Astronomic_Catalogs.Results;

namespace Astronomic_Catalogs.Services.Interfaces;

public interface IPlanetarySystemFilterService
{
    Task<List<PlanetarySystem>?> GetFilteredDataAsync(Dictionary<string, object> parameters);
}
