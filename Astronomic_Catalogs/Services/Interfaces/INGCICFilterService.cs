using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.ViewModels;

namespace Astronomic_Catalogs.Services.Interfaces;

public interface INGCICFilterService
{
    Task<List<NGCICOpendatasoft>?> GetFilteredDataAsync(Dictionary<string, object> parameters);
    Task<(int countNGCTask, int countNGCE_Task, List<ConstellationViewModel> constellations, List<NGCICOpendatasoft>? catalogItems)> GetNGCICOpendatasoftDataAsync();
}
