using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.DTO;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.Json;

namespace Astronomic_Catalogs.Services;

public class NGCICFilterService : INGCICFilterService
{
    private readonly ApplicationDbContext _context;
    private readonly ICacheService _cache;

    public NGCICFilterService(ApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<NGCICOpendatasoft>?> GetFilteredDataAsync(Dictionary<string, object> parameters)
    {
        string? name = parameters.TryGetValue("Name", out var nameObj) ? nameObj?.ToString() : null;
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

        var excludedKeys = new[] { "NGC_Catalog", "IC_Catalog", "Messier_Catalog" };
        var objectTypes = parameters
            .Where(p =>
                !excludedKeys.Contains(p.Key) &&
                p.Value is not null &&
                bool.TryParse(p.Value.ToString(), out var b) && b)
            .Select(p => p.Key)
            .ToList();
        string? objectTypesJson = objectTypes.Any() ? JsonSerializer.Serialize(objectTypes) : null;

        bool includeNGC = parameters.GetBool("NGC_Catalog");
        bool includeIC = parameters.GetBool("IC_Catalog");
        bool includeMessier = parameters.GetBool("Messier_Catalog");

        int? pageNumber = parameters.GetInt("PageNumberVaulue");
        int? rowOnPage = parameters.GetInt("RowOnPageCatalog");


        string cacheKey = parameters.ToCacheKey("CollinderData");

        return await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            var result = await _context.NGCIC_Catalog
            .FromSqlInterpolated($@"
                EXEC GetFilteredNGCICData 
                    @Name = {name},
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
                    @IncludeNGC = {includeNGC},
                    @IncludeIC = {includeIC},
                    @IncludeMessier = {includeMessier},
                    @PageNumber = {pageNumber},
                    @RowOnPage = {rowOnPage}
            ")
            .AsNoTracking()
            .ToListAsync();

            return result;
        });
    }

    public async Task<(int countNGCTask, int countNGCE_Task, List<ConstellationDto> constellations, List<NGCICOpendatasoft>? catalogItems)> GetNGCICOpendatasoftDataAsync()
    {
        using var conn = _context.Database.GetDbConnection();

        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync();

        var sql = @"
                SELECT COUNT(*) FROM NGCICOpendatasoft;
                SELECT COUNT(*) FROM NGCICOpendatasoft_Extension;
                SELECT 
                    Short_name AS ShortName, 
                    Latine_name_Nominative_case AS LatineNameNominativeCase, 
                    English_name AS EnglishName, 
                    Ukraine_name AS UkraineName
                FROM Constellation;
                SELECT TOP 50 
                    Id,
                    NGC_IC,
                    [Name],
                    SubObject,
                    Messier,
                    Name_UK,
                    Comment,
                    Other_names AS OtherNames,
                    NGC,
                    IC,
                    Limit_Ang_Diameter AS LimitAngDiameter,
                    Ang_Diameter AS AngDiameter,
                    ObjectTypeAbrev,
                    ObjectType,
                    Object_type AS ObjectTypeFull,
                    Source_Type AS SourceType,

                    RA,
                    Right_ascension AS RightAscension,
                    Right_ascension_H AS RightAscensionH,
                    Right_ascension_M AS RightAscensionM,
                    Right_ascension_S AS RightAscensionS,

                    DEC,
                    Declination,
                    NS,
                    Declination_D AS DeclinationD,
                    Declination_M AS DeclinationM,
                    Declination_S AS DeclinationS,

                    Constellation,
                    MajorAxis,
                    MinorAxis,
                    PositionAngle,

                    App_Mag AS AppMag,
                    App_Mag_Flag AS AppMagFlag,
                    b_mag AS BMag,
                    v_mag AS VMag,
                    j_mag AS JMag,
                    h_mag AS HMag,
                    k_mag AS KMag,

                    Surface_Brigthness AS SurfaceBrightness,
                    Hubble_OnlyGalaxies AS HubbleOnlyGalaxies,
                    Cstar_UMag AS CstarUMag,
                    Cstar_BMag AS CstarBMag,
                    Cstar_VMag AS CstarVMag,
                    Cstar_Names AS CstarNames,
                    CommonNames,
                    NedNotes,
                    OpenngcNotes,
                    Image,

                    RowOnPage,
                    SourceTable
                FROM NGCICOpendatasoft
                ORDER BY NGC_IC DESC, Name ASC;
            ";

        using var multi = await conn.QueryMultipleAsync(sql);

        int countNGCTask = await multi.ReadFirstAsync<int>();
        var countNGCE_Task = await multi.ReadFirstAsync<int>();
        var constellations = (await multi.ReadAsync<ConstellationDto>()).ToList();
        var catalogItems = (await multi.ReadAsync<NGCICOpendatasoft>()).ToList();

        return (countNGCTask, countNGCE_Task, constellations, catalogItems);
    }

}

