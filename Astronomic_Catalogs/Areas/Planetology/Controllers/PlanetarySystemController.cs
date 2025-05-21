using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.DTO;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using Astronomic_Catalogs.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Areas.Planetology.Controllers;

[Area("Planetology")]
public class PlanetarySystemController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IPlanetarySystemFilterService _filterService;
    private readonly IDistinctSelectListService _distinctSelectListService;
    private readonly IMapper _mapper;

    public PlanetarySystemController(
        ApplicationDbContext context,
        IPlanetarySystemFilterService filterService,
        IDistinctSelectListService distinctSelectListService, 
        IMapper mapper
        )
    {
        _context = context;
        _filterService = filterService;
        _distinctSelectListService = distinctSelectListService;
        _mapper = mapper;
    }

    // GET: PlanetarySystem/PlanetsCatalog
    public async Task<IActionResult> Index()
    {
        var allSystems = await Get30PlanetSystems(_context, 1000000000);
        ViewBag.RowsCount = allSystems.Count;

        ViewBag.PlanetNames = await _distinctSelectListService.GetDistinctSelectListAsync(c => c.PlLetter);
        ViewBag.TelescopNames = await _distinctSelectListService.GetDistinctSelectListAsync(c => c.DiscTelescope);
        ViewBag.DiscoveryMethod = await _distinctSelectListService.GetDistinctSelectListAsync(c => c.DiscoveryMethod);

        var systems = await Get30PlanetSystems(_context, 10);

        return View(systems);
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromBody] Dictionary<string, object> parameters)
    {
        ViewBag.RowOnPageCatalog = parameters.GetString("RowOnPageCatalog") ?? "30";

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

        ViewBag.RowsCount = firstItem?.PageCount ?? 1;
        ViewBag.AmountRowsResult = firstItem?.PageNumber ?? 0; // Using the PageNumber field of the database table to pass a value.
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
        var systems = await Get30PlanetSystems(_context, 10);
        return PartialView("_PlanetarySystemTableInGroups", systems);
    }

    [HttpPost]
    public async Task<IActionResult> GetFlatTable([FromBody] Dictionary<string, object> parameters)
    {
        var systems = await Get30PlanetSystems(_context, 10);
        return PartialView("_PlanetarySystemTable", systems);
    }

    /// <summary>
    /// Delete this method after the stored procedure has been implemented.!!!
    /// </summary>
    public async Task<List<PlanetarySystem>> Get30PlanetSystems (ApplicationDbContext _context, int count)
    {
        var results = await _context.PlanetsCatalog.Take(100).ToListAsync(); 

        List<PlanetarySystem> systems = results
            .GroupBy(p => p.Hostname)
            .Take(count)
            .Select(g =>
            {
                var first = g.First() ?? new NASAExoplanetCatalog();

                return new PlanetarySystem
                {
                    Hostname = first.Hostname,
                    HdName = first.HdName, 
                    HipName = first.HipName,
                    TicId = first.TicId,
                    GaiaId = first.GaiaId,
                    StSpectype = first.StSpectype ?? "",
                    StTeff = first.StTeff,
                    StRad = first.StRad,
                    StMass = first.StMass,
                    StMet = first.StMet,
                    StMetratio = first.StMetratio,
                    StLum = first.StLum,
                    StAge = first.StAge,
                    SyDist = first.SyDist,
                    Exoplanets = g.Select((x, i) => new Exoplanet
                    {
                        Id = i + 1,
                        PlLetter = x.PlLetter,
                        PlRade = x.PlRade,
                        PlRadJ = x.PlRadJ,
                        PlMasse = x.PlMasse,
                        PlMassJ = x.PlMassJ,
                        PlOrbsmax = x.PlOrbSmax 
                    }).ToList(),
                    PageNumber = first.PageNumber,
                    PageCount = first.PageCount
                };
            })
            .ToList();

        return systems;
    }
}
