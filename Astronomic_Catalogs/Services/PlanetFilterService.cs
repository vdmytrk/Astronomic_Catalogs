using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Services
{
    public class PlanetFilterService : IPlanetFilterService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cache;

        public PlanetFilterService(ApplicationDbContext context, ICacheService cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<NASAExoplanetCatalog>?> GetFilteredDataAsync(Dictionary<string, object> parameters)
        {
            bool planetWithSize = parameters.GetBool("PlanetWithSize");
            var planetType = parameters.TryGetValue("PlanetType", out var objPT)
                ? JsonSerializerOneUnit.SerializeToNormalizedJson(objPT)
                : null;
            var telescope = parameters.TryGetValue("Telescope", out var objT)
                ? JsonSerializerOneUnit.SerializeToNormalizedJson(objT)
                : null;
            var planetName = parameters.TryGetValue("PlanetName", out var objPN)
                ? JsonSerializerOneUnit.SerializeToNormalizedJson(objPN)
                : null;
            string? name = parameters.TryGetValue("Name", out var nameObj) ? nameObj?.ToString() : null;
            var discoveryMethod = parameters.TryGetValue("DiscoveryMethod", out var objDM)
                ? JsonSerializerOneUnit.SerializeToNormalizedJson(objDM)
                : null;
            int? plenetCountFom = parameters.GetInt("DiscoveredPlenetCountFom");
            int? plenetCountTo = parameters.GetInt("DiscoveredPlenetCountTo");
            DateTime? dateFrom = parameters.GetDateTime("DateFrom");
            DateTime? dateTo = parameters.GetDateTime("DateTo");
            bool habitableZone = parameters.GetBool("HabitableZonePlanets");
            bool terrestrialHabitableZone = parameters.GetBool("TerrestrialHabitableZonePlanets");

            int? pageNumber = parameters.GetInt("PageNumberVaulue");
            int? rowOnPage = parameters.GetInt("RowOnPageCatalog");

            string cacheKey = parameters.ToCacheKey("Planet");

            return await _cache.GetOrAddAsync(cacheKey, async () =>
            {
                var result = await _context.PlanetsCatalog
                .FromSqlInterpolated($@"
                EXEC GetFilteredNGCICData 
                    @PlanetWithSize = {planetWithSize},
                    @PlanetType = {planetType},
                    @Telescope = {telescope},
                    @PlanetName = {planetName},
                    @Name = {name},
                    @DiscoveryMethod = {discoveryMethod},
                    @PlenetCountFom = {plenetCountFom},
                    @PlenetCountTo = {plenetCountTo},
                    @DateFrom = {dateFrom},
                    @DateTo = {dateTo},
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
}
