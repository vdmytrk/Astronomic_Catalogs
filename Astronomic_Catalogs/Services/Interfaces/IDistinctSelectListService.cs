using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq.Expressions;

namespace Astronomic_Catalogs.Services.Interfaces;

public interface IDistinctSelectListService
{
    Task<List<SelectListItem>> GetDistinctSelectListAsync(Expression<Func<NASAExoplanetCatalog, string?>> selector);
}
