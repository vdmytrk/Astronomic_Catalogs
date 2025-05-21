using Astronomic_Catalogs.DTO;

namespace Astronomic_Catalogs.Services.Interfaces;

public interface IPlanetarySystemFilterService
{
    Task<List<PlanetarySystem>?> GetFilteredDataAsync(Dictionary<string, object> parameters);
}
