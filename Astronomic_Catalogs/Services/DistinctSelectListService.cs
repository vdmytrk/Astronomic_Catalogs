using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Astronomic_Catalogs.Services;

public class DistinctSelectListService : IDistinctSelectListService
{
    private readonly ApplicationDbContext _context;

    public DistinctSelectListService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SelectListItem>> GetDistinctSelectListAsync(Expression<Func<NASAExoplanetCatalog, string?>> selector)
    {
        return await _context.PlanetsCatalog
            .Select(selector)
            .Distinct()
            .Where(x => x != null)
            .OrderBy(x => x)
            .Select(x => new SelectListItem
            {
                Value = x!,
                Text = x!
            })
            .ToListAsync();
    }
}
