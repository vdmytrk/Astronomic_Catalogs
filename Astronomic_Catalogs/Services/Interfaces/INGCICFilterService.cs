using Astronomic_Catalogs.DTO;
using Astronomic_Catalogs.Models;

namespace Astronomic_Catalogs.Services.Interfaces;

public interface INGCICFilterService
{
    Task<List<NGCICOpendatasoft>?> GetFilteredDataAsync(Dictionary<string, object> parameters);
    Task<(int countNGCTask, int countNGCE_Task, List<ConstellationDto> constellations, List<NGCICOpendatasoft>? catalogItems)> GetNGCICOpendatasoftDataAsync();
}
