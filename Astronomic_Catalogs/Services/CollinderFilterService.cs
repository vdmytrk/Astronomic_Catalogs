using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Astronomic_Catalogs.Services;

public class CollinderFilterService : ICollinderFilterService
{
    private readonly ApplicationDbContext _context;

    public CollinderFilterService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CollinderCatalog>> GetFilteredDataAsync(Dictionary<string, object> parameters)
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

        int? pageNumber = parameters.GetInt("PageNumberVaulue");
        string rowOnPageCatalog = parameters.GetString("RowOnPageCatalog") ?? "50";
        int? rowOnPage = rowOnPageCatalog == "All" ? 500 : int.Parse(rowOnPageCatalog);


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
    }

}

