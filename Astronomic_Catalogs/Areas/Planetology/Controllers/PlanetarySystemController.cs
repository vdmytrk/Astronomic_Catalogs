using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Entities;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Areas.Planetology.Controllers;

[Area("Planetology")]
public class PlanetarySystemController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IPlanetarySystemFilterService _filterService;
    private readonly IPlanetFilterService _planetFilterService;
    private readonly IMapper _mapper;

    public PlanetarySystemController(
        ApplicationDbContext context,
        IPlanetarySystemFilterService filterService,
        IPlanetFilterService planetFilterService,
        IMapper mapper
        )
    {
        _context = context;
        _filterService = filterService;
        _planetFilterService = planetFilterService;
        _mapper = mapper;
    }

    // GET: PlanetarySystem/PlanetsCatalog
    public async Task<IActionResult> Index()
    {
        var (plLetters, telescopes, discoveryMethods) = await _planetFilterService.GetCatalogStatsAsync(); 
        ViewBag.RowOnPageCatalog = "10";
        ViewBag.PlanetNames = plLetters;
        ViewBag.TelescopNames = telescopes;
        ViewBag.DiscoveryMethod = discoveryMethods;

        // Since the stored procedure GetFilteredPlanetsData returns a result from multiple tables and not all fields
        var result = await _filterService.GetFilteredDataAsync(new() { ["pageNumber"] = 1, ["rowOnPage"] = 10 });
        ViewBag.AmountRowsResult = result?.FirstOrDefault()?.RowOnPage ?? 1;

        return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromBody] Dictionary<string, object> parameters)
    {
        ViewBag.RowOnPageCatalog = parameters.GetString("RowOnPageCatalog") ?? "10";
        int? pageNumber = parameters.GetInt("PageNumberVaulue");
        ViewBag.PageNumber = pageNumber == 0 || pageNumber == null ? 1 : pageNumber;

        List<PlanetarySystem>? selectedList = new ();

        try
        {
            selectedList = await _filterService.GetFilteredDataAsync(parameters);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error fetching filtered data: {ex.Message}");
        }

        if (selectedList == null)
            return NotFound();

        var firstItem = selectedList.FirstOrDefault();

        // TODO: Use Dapper to return an output parameter
        ViewBag.AmountRowsResult = firstItem?.RowOnPage ?? 0; // Using the RowOnPage field of the database table to pass a value.
        ViewBag.Contorller = "PlanetarySystem";

        string partialViewName = parameters.GetString("PartialViewName")!;

        try
        {
            var tableHtml = await this.RenderViewAsync(partialViewName, selectedList, true);
            var paginationHtml = await this.RenderViewAsync("_PaginationLine", null, true);

            return Json(new { tableHtml, paginationHtml });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"RenderViewAsync error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetGroupedTable([FromBody] Dictionary<string, object> parameters)
    {
        var systems = await _filterService.GetFilteredDataAsync(parameters);
        return PartialView("_PlanetarySystemTableInGroups", systems);
    }

    [HttpPost]
    public async Task<IActionResult> GetFlatTable([FromBody] Dictionary<string, object> parameters)
    {
        var systems = await _filterService.GetFilteredDataAsync(parameters);
        return PartialView("_PlanetarySystemTable", systems);
    }

}
