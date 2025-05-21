using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.DTO;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System;

namespace Astronomic_Catalogs.Services;

public class PlanetarySystemFilterService : IPlanetarySystemFilterService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cache;

    public PlanetarySystemFilterService (ApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<PlanetarySystem>?> GetFilteredDataAsync(Dictionary<string, object> parameters)
    {
        var planetType = parameters.TryGetValue("PlanetType", out var obj)
            ? JsonSerializerOneUnit.SerializeToNormalizedJson(obj)
            : null;
        string? name = parameters.TryGetValue("Name", out var nameObj) ? nameObj?.ToString() : null;
        int? plenetCountFom = parameters.GetInt("DiscoveredPlenetCountFom");
        int? plenetCountTo = parameters.GetInt("DiscoveredPlenetCountTo");
        int? orderBy = parameters.GetInt("OrderBy");
        bool habitableZone = parameters.GetBool("HabitableZonePlanets");
        bool terrestrialHabitableZone = parameters.GetBool("TerrestrialHabitableZonePlanets");

        int? pageNumber = parameters.GetInt("PageNumberVaulue");
        int? rowOnPage = parameters.GetInt("RowOnPageCatalog");

        string cacheKey = parameters.ToCacheKey("PlanetarySystem");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            var result = await _context.PlanetarySystemsCatalog
            .FromSqlInterpolated($@"
                EXEC GetFilteredNGCICData 
                    @Name = {name},
                    @PlanetType = {planetType},
                    @PlenetCountFom = {plenetCountFom},
                    @PlenetCountTo = {plenetCountTo},
                    @OrderBy = {orderBy},
                    @HabitableZone = {habitableZone},
                    @TerrestrialHabitableZone = {terrestrialHabitableZone},
                    @PageNumber = {pageNumber},
                    @RowOnPage = {rowOnPage}
            ")
            .AsNoTracking()
            .ToListAsync();

            return result;
        });
    }
}
