using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using Astronomic_Catalogs.ViewModels;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;

namespace Astronomic_Catalogs.Services;

public class CollinderFilterService : ICollinderFilterService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cache;
    private readonly ILogger<CollinderFilterService> _logger;

    public CollinderFilterService(ApplicationDbContext context, ICacheService cache, ILogger<CollinderFilterService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<CollinderCatalog>?> GetFilteredDataAsync(Dictionary<string, object> parameters)
    {
        string? nameOtherCat = parameters.TryGetValue("Name", out var nameObj) ? nameObj?.ToString() : null;
        var constellationsJson = parameters.TryGetValue("constellations", out var obj)
            ? JsonSerializerOneUnit.SerializeToNormalizedJson(obj)
            : null;

        double? angDiameterMin = parameters.GetInt("Ang_Diameter_min");
        double? angDiameterMax = parameters.GetInt("Ang_Diameter_max");

        int? raFromH = parameters.GetInt("RA_From_Hours");
        int? raFromM = parameters.GetInt("RA_From_Minutes");
        double? raFromS = parameters.GetDouble("RA_From_Seconds");
        int? raToH = parameters.GetInt("RA_To_Hours");
        int? raToM = parameters.GetInt("RA_To_Minutes");
        double? raToS = parameters.GetDouble("RA_To_Seconds");

        string? decFromPole = parameters.GetString("Dec_From_Pole");
        int? decFromD = parameters.GetInt("Dec_From_Degrees");
        int? decFromM = parameters.GetInt("Dec_From_Minutes");
        double? decFromS = parameters.GetDouble("Dec_From_Seconds");
        string? decToPole = parameters.GetString("Dec_To_Pole");
        int? decToD = parameters.GetInt("Dec_To_Degrees");
        int? decToM = parameters.GetInt("Dec_To_Minutes");
        double? decToS = parameters.GetDouble("Dec_To_Seconds");

        var objectTypes = parameters
            .Where(p =>
                p.Value is not null &&
                bool.TryParse(p.Value.ToString(), out var b) && b)
            .Select(p => p.Key)
            .ToList();
        string? objectTypesJson = objectTypes.Any() ? JsonSerializer.Serialize(objectTypes) : null;

        int? pageNumber = parameters.GetInt("PageNumberValue");
        string rowOnPageCatalog = parameters.GetString("RowOnPageCatalog") ?? "50";
        int? rowOnPage = rowOnPageCatalog == "All" ? 500 : int.Parse(rowOnPageCatalog);


        string cacheKey = parameters.ToCacheKey("CollinderData");

        try
        {
            return await _cache.GetOrAddAsync(cacheKey, async () =>
            {
                var result = await _context.CollinderCatalog
                    .FromSqlInterpolated($@"
                EXEC GetFilteredCollinderData 
                    @NameOtherCat = {nameOtherCat},
                    @Constellations = {constellationsJson},
                    @Ang_Diameter_min = {angDiameterMin},
                    @Ang_Diameter_max = {angDiameterMax},
                    @RA_From_Hours = {raFromH},
                    @RA_From_Minutes = {raFromM},
                    @RA_From_Seconds = {raFromS},
                    @RA_To_Hours = {raToH},
                    @RA_To_Minutes = {raToM},
                    @RA_To_Seconds = {raToS},
                    @Dec_From_Pole = {decFromPole},
                    @Dec_From_Degrees = {decFromD},
                    @Dec_From_Minutes = {decFromM},
                    @Dec_From_Seconds = {decFromS},
                    @Dec_To_Pole = {decToPole},
                    @Dec_To_Degrees = {decToD},
                    @Dec_To_Minutes = {decToM},
                    @Dec_To_Seconds = {decToS},
                    @ObjectTypes = {objectTypesJson},
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
            _logger.LogError(ex, "Exception occurred in List<CollinderCatalog> GetFilteredDataAsync.");
            throw;
        }
    }

    public async Task<(int count, List<CollinderCatalog> rawData, List<ConstellationViewModel> constellations)> GetCollinderCatalogDataAsync()
    {
        try
        {
            using var conn = _context.Database.GetDbConnection();

            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            var sql = @"
                SELECT COUNT(*) FROM CollinderCatalog;
                SELECT TOP 50 
                    Id, 
	                [Namber_name] AS NamberName, 
	                NameOtherCat, 
	                Constellation,
	                Right_ascension AS RightAscension, 
	                Right_ascension_H AS RightAscensionH, 
	                Right_ascension_M AS RightAscensionM, 
	                Right_ascension_S AS RightAscensionS,
	                Declination, 
	                NS, 
	                Declination_D AS DeclinationD, 
	                Declination_M AS DeclinationM, 
	                Declination_S AS DeclinationS,
	                App_Mag AS AppMag, 
	                App_Mag_Flag AS AppMagFlag,
	                CountStars, 
	                CountStars_ToFinding AS CountStarsToFinding,
	                [Ang_Diameter] AS AngDiameter, 
	                Ang_Diameter_Max AS AngDiameterNew,
	                Class, 
	                Comment
                FROM CollinderCatalog;
                SELECT 
                    Short_name AS ShortName, 
                    Latine_name_Nominative_case AS LatineNameNominativeCase, 
                    English_name AS EnglishName, 
                    Ukraine_name AS UkraineName
                FROM Constellation;
            ";

            using var multi = await conn.QueryMultipleAsync(sql);

            int count = await multi.ReadFirstAsync<int>();
            var rawData = (await multi.ReadAsync<CollinderCatalog>()).ToList();
            var constellations = (await multi.ReadAsync<ConstellationViewModel>()).ToList();

            return (count, rawData, constellations);
        }
        catch (Exception sqlEx) when (sqlEx is SqlException || sqlEx is InvalidOperationException)
        {
            string message = sqlEx switch
            {
                SqlException => "SQL exception in GetCollinderCatalogDataAsync.",
                InvalidOperationException => "Invalid operation in GetCollinderCatalogDataAsync. Possibly wrong DB state or context misuse.",
                _ => "An unexpected rendering error occurred."
            };
            _logger.LogError(sqlEx, message);
            sqlEx.Data["ErrorMessage"] = message;
            sqlEx.Data["IsLogged"] = true;

            throw;
        }
        catch (Exception ex)
        {
            string message = "Unexpected exception in GetCollinderCatalogDataAsync";
            _logger.LogError(ex, message);
            ex.Data["ErrorMessage"] = message;
            ex.Data["IsLogged"] = true;

            throw;
        }
    }

} 

