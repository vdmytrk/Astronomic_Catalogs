using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.DTO;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Results;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Astronomic_Catalogs.Services;

public class PlanetFilterService : IPlanetFilterService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<PlanetFilterService> _logger;

    public PlanetFilterService(ApplicationDbContext context, ICacheService cache, ILogger<PlanetFilterService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
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
        name = String.IsNullOrEmpty(name) ? null : name;
        var discoveryMethod = parameters.TryGetValue("DiscoveryMethod", out var objDM)
            ? JsonSerializerOneUnit.SerializeToNormalizedJson(objDM)
            : null;
        int? plenetsCountFom = parameters.GetInt("DiscoveredPlenetCountFom");
        int? plenetsCountTo = parameters.GetInt("DiscoveredPlenetCountTo");
        DateTime? dateFrom = parameters.GetDateTime("DateFrom");
        DateTime? dateTo = parameters.GetDateTime("DateTo");
        int? distanceToStarFrom = parameters.GetInt("DistanceToStarFrom");
        int? distanceToStarTo = parameters.GetInt("DistanceToStarTo");
        bool habitableZone = parameters.GetBool("HabitableZonePlanets");
        bool terrestrialHabitableZone = parameters.GetBool("TerrestrialHabitableZonePlanets");

        int? pageNumber = parameters.GetInt("PageNumberValue") ?? 1;
        int? rowOnPage = parameters.GetInt("RowOnPageCatalog") ?? 30;

        string cacheKey = parameters.ToCacheKey("Planet");

        try
        {
            return await _cache.GetOrAddAsync(cacheKey, async () =>
            {
                var result = await _context.PlanetsCatalog
                .FromSqlInterpolated($@"
                EXEC GetFilteredPlanetsData
                    @PlanetWithSize = {planetWithSize},
                    @PlanetType = {planetType},
                    @Telescope = {telescope},
                    @PlanetName = {planetName},
                    @Name = {name},
                    @DiscoveryMethod = {discoveryMethod},
                    @DateFrom = {dateFrom},
                    @DateTo = {dateTo},
                    @HabitableZone = {habitableZone},
                    @TerrestrialHabitableZone = {terrestrialHabitableZone},
                    @DistanceToStarFrom = {distanceToStarFrom},
                    @DistanceToStarTo = {distanceToStarTo},
                    @PageNumber = {pageNumber},
                    @RowOnPage = {rowOnPage}
                ")
                .AsNoTracking()
                .ToListAsync();

                return result;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred in List<NASAExoplanetCatalog> GetFilteredDataAsync.");
            throw;
        }
    }

    public async Task<(List<SelectListItem> plLetters, List<SelectListItem> telescopes, List<SelectListItem> discoveryMethods)> GetCatalogStatsAsync()
    {
        try
        {
            using var conn = new SqlConnection(_context.Database.GetConnectionString());

            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var sql = @"
                SELECT DISTINCT Pl_letter AS PlLetter  FROM NASAExoplanetCatalog WHERE Pl_letter IS NOT NULL;
                SELECT DISTINCT Disc_telescope AS DiscTelescope FROM NASAExoplanetCatalog WHERE Disc_telescope IS NOT NULL;
                SELECT DISTINCT DiscoveryMethod FROM NASAExoplanetCatalog WHERE DiscoveryMethod IS NOT NULL;
            ";

            using var multi = await conn.QueryMultipleAsync(sql);

            var plLetterRaw = (await multi.ReadAsync<string>()).ToList();
            var telescopesRaw = (await multi.ReadAsync<string>()).ToList();
            var discoveryMethodsRaw = (await multi.ReadAsync<string>()).ToList();

            List<SelectListItem> plLetters = SelectListUtils.FromStrings(plLetterRaw);
            List<SelectListItem> telescopes = SelectListUtils.FromStrings(telescopesRaw);
            List<SelectListItem> discoveryMethods = SelectListUtils.FromStrings(discoveryMethodsRaw);

            return (plLetters, telescopes, discoveryMethods);
        }
        catch (Exception sqlEx) when (sqlEx is SqlException || sqlEx is InvalidOperationException)
        {
            string message = sqlEx switch
            {
                SqlException => "SQL exception in GetCatalogStatsAsync.",
                InvalidOperationException => "Invalid operation in GetCatalogStatsAsync. Possibly wrong DB state or context misuse.",
                _ => "An unexpected rendering error occurred."
            };
            _logger.LogError(sqlEx, message);
            sqlEx.Data["ErrorMessage"] = message;
            sqlEx.Data["IsLogged"] = true;

            throw;
        }
        catch (Exception ex)
        {
            string message = "Unexpected exception in GetCatalogStatsAsync";
            _logger.LogError(ex, message);
            ex.Data["ErrorMessage"] = message;
            ex.Data["IsLogged"] = true;

            throw;
        }
    }


}
