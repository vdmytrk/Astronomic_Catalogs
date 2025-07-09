using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.DTO;
using Astronomic_Catalogs.Entities;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Astronomic_Catalogs.Services;

public class PlanetarySystemFilterService : IPlanetarySystemFilterService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cache;
    private readonly IMapper _mapper;
    private readonly ILogger<PlanetarySystemFilterService> _logger;

    public PlanetarySystemFilterService (
        ApplicationDbContext context, 
        ICacheService cache, 
        IMapper mapper,
        ILogger<PlanetarySystemFilterService> logger)
    {
        _context = context;
        _cache = cache;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<PlanetarySystem>?> GetFilteredDataAsync(Dictionary<string, object> parameters)
    {
        string? planetType = parameters.TryGetValue("PlanetType", out var obj)
            ? JsonSerializerOneUnit.SerializeToNormalizedJson(obj)
            : null;
        string? name = parameters.TryGetValue("Name", out var nameObj) ? nameObj?.ToString() : null;
        name = String.IsNullOrEmpty(name) ? null : name;
        int? plenetCountFom = parameters.GetInt("DiscoveredPlenetCountFom");
        int? plenetCountTo = parameters.GetInt("DiscoveredPlenetCountTo");
        int? orderBy = parameters.GetInt("OrderBy");
        int? distanceFrom = parameters.GetInt("DistanceFrom");
        int? distanceTo = parameters.GetInt("DistanceTo");
        bool habitableZone = parameters.GetBool("HabitableZonePlanets");
        bool terrestrialHabitableZone = parameters.GetBool("TerrestrialHabitableZonePlanets");

        int? pageNumber = parameters.GetInt("PageNumberVaulue") ?? 1;
        int? rowOnPage = parameters.GetInt("RowOnPageCatalog") ?? 10;

        string cacheKey = parameters.ToCacheKey("PlanetarySystem");

        try
        {
            return await _cache.GetOrAddAsync(cacheKey, async () =>
            {
                List<PlanetarySystemFlatRow> result = await _context.PlanetarySystemsCatalog
                .FromSqlInterpolated($@"
                    EXEC GetFilteredPlanetarySystemsData  
                        @PlanetType = {planetType},
                        @Name = {name},
                        @PlenetsCountFrom = {plenetCountFom},
                        @PlenetsCountTo = {plenetCountTo},
                        @OrderBy = {orderBy},
                        @HabitableZone = {habitableZone},
                        @TerrestrialHabitableZone = {terrestrialHabitableZone},
                        @SyDistFrom = {distanceFrom},
                        @SyDistTo = {distanceTo},
                        @PageNumber = {pageNumber},
                        @RowOnPage = {rowOnPage}
                ")
                .AsNoTracking()
                .ToListAsync();

                return MapToSystems(result);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in List<PlanetarySystem> GetFilteredDataAsync.");
            throw;
        }

    }

    public List<PlanetarySystem> MapToSystems(List<PlanetarySystemFlatRow> flatRows)
    {
        return flatRows
            .GroupBy(row => row.Hostname)
            .Select(group =>
            {
                var system = _mapper.Map<PlanetarySystem>(group.First());

                system.Exoplanets = group
                    .Select((row, index) =>
                    {
                        var planet = _mapper.Map<Exoplanet>(row);
                        planet.Id = index + 1;
                        return planet;
                    }).ToList();

                return system;
            }).ToList();
    }

}