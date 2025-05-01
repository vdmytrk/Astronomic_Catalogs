using Astronomic_Catalogs.Models;

namespace Astronomic_Catalogs.Services.Interfaces;

public interface INGCICFilterService
{
    Task<List<NGCICOpendatasoft>> GetFilteredDataAsync(Dictionary<string, object> parameters);
}
