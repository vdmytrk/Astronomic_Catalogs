using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Entities;
using Astronomic_Catalogs.Exceptions;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace Astronomic_Catalogs.Areas.Planetology.Controllers;

[Area("Planetology")]
public class PlanetarySystemController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IPlanetarySystemFilterService _filterService;
    private readonly IPlanetFilterService _planetFilterService;
    private readonly IMapper _mapper;
    private readonly ILogger<PlanetarySystemController> _logger;
    private readonly IExceptionRedirectUrlService _exRedirectService;

    public PlanetarySystemController(
        ApplicationDbContext context,
        IPlanetarySystemFilterService filterService,
        IPlanetFilterService planetFilterService,
        IMapper mapper,
        ILogger<PlanetarySystemController> logger,
        IExceptionRedirectUrlService exRedirectService
        )
    {
        _context = context;
        _filterService = filterService;
        _planetFilterService = planetFilterService;
        _mapper = mapper;
        _logger = logger;
        _exRedirectService = exRedirectService;
    }

    // GET: PlanetarySystem/PlanetsCatalog
    public async Task<IActionResult> Index()
    {
        List<SelectListItem> plLetters;
        List<SelectListItem> telescopes;
        List<SelectListItem> discoveryMethods;

        try
        {
            (plLetters, telescopes, discoveryMethods) = await _planetFilterService.GetCatalogStatsAsync();
        }
        catch (Exception ex)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            string messageTemplate = "An unexpected error occurred during data retrieval in PlanetarySystemController.";
            _logger.LogError(ex, "{Message} RequestId: {RequestId}", messageTemplate, requestId);

            var message = ex.Data.Contains("ErrorMessage")
                ? ex.Data["ErrorMessage"]?.ToString()
                : string.IsNullOrEmpty(ex.Message)
                    ? $"{messageTemplate} RequestId : {requestId}"
                    : ex.Message;

            TempData["RequestId"] = requestId;
            TempData["IsLogged"] = true;
            TempData["ErrorMessage"] = message;
            TempData["StackTrace"] = ex.ToString();
            TempData["Path"] = HttpContext.Request.Path.ToString();
#if DEBUG
            throw;
#else
            return RedirectToAction("Error", "Error");
#endif
        }

        ViewBag.RowOnPageCatalog = "100";
        ViewBag.PlanetNames = plLetters;
        ViewBag.TelescopNames = telescopes;
        ViewBag.DiscoveryMethod = discoveryMethods;

        List<PlanetarySystem>? result = new();

        try
        {
            // Since the stored procedure GetFilteredPlanetsData returns a result from multiple tables and not all fields
            result = await _filterService.GetFilteredDataAsync(new() { ["PageNumberVaulue"] = 1, ["RowOnPageCatalog"] = 100 });
            ViewBag.AmountRowsResult = result?.FirstOrDefault()?.RowOnPage ?? 1;
        }
        catch (Exception ex)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            string messageTemplate = $"An unexpected error occurred during data retrieval AmountRowsResult in PlanetarySystemController.";
            _logger.LogError(ex, "{Message}. RequestId: {RequestId}", messageTemplate, requestId);

            var message = ex.Data.Contains("ErrorMessage") 
                ? ex.Data["ErrorMessage"]?.ToString() 
                : string.IsNullOrEmpty(ex.Message) 
                    ? $"{messageTemplate} RequestId : {requestId}"
                    : ex.Message;

            TempData["RequestId"] = requestId;
            TempData["IsLogged"] = true;
            TempData["ErrorMessage"] = message;
            TempData["StackTrace"] = ex.ToString();
            TempData["Path"] = HttpContext.Request.Path.ToString();
#if DEBUG
            throw;
#else
            return RedirectToAction("Error", "Error");
#endif
        }

        return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromBody] Dictionary<string, object> parameters)
    {
        ViewBag.RowOnPageCatalog = parameters.GetString("RowOnPageCatalog") ?? "100";
        int? pageNumber = parameters.GetInt("PageNumberVaulue");
        ViewBag.PageNumber = pageNumber == 0 || pageNumber == null ? 1 : pageNumber;
        List<PlanetarySystem>? selectedList = new ();

        try
        {
            selectedList = await _filterService.GetFilteredDataAsync(parameters);
        }
        catch (Exception ex)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var redirectUrl = _exRedirectService.BuildRedirectUrl(ex, requestId, HttpContext.Request.Path);
#if DEBUG
            throw;
#else
            return Json(new { redirectTo = redirectUrl });
#endif
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
        catch (Exception ex) when (ex is FileNotFoundException || ex is ViewRenderingException)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var redirectUrl = _exRedirectService.BuildRedirectUrl(ex, requestId, HttpContext.Request.Path);
            string errorMessage = ex switch
            {
                FileNotFoundException => "Partial view file was not found during rendering.",
                ViewRenderingException => "An error occurred while rendering the partial view.",
                _ => "An unexpected rendering error occurred."
            };

            _logger.LogError(ex, "RequestId: {RequestId}", requestId);
#if DEBUG
            throw;
#else
            return Json(new { redirectTo = redirectUrl });
#endif
        }
        catch (Exception ex)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var redirectUrl = _exRedirectService.BuildRedirectUrl(ex, requestId, HttpContext.Request.Path);

            _logger.LogError(ex, "Unexpected error rendering partial views. RequestId: {RequestId}", requestId);
#if DEBUG
            throw;
#else
            return Json(new { redirectTo = redirectUrl });
#endif
        }
    }

    [HttpPost]
    public async Task<IActionResult> PlanetarySystemVisualization([FromBody] Dictionary<string, object> parameters)
    {
        ViewBag.RowOnPageCatalog = parameters.GetString("RowOnPageCatalog") ?? "100";
        int? pageNumber = parameters.GetInt("PageNumberVaulue");
        ViewBag.PageNumber = pageNumber == 0 || pageNumber == null ? 1 : pageNumber;
        List<PlanetarySystem>? selectedList = new();

        try
        {
            selectedList = await _filterService.GetFilteredDataAsync(parameters);
        }
        catch (Exception ex)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var redirectUrl = _exRedirectService.BuildRedirectUrl(ex, requestId, HttpContext.Request.Path);
#if DEBUG
            throw;
#else
            return Json(new { redirectTo = redirectUrl });
#endif
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
            var paginationHtml = await this.RenderViewAsync("_PaginationLine", null, true);
            var tableHtml = await this.RenderViewAsync(partialViewName, selectedList, true);

            return Json(new { tableHtml, paginationHtml });
        }
        catch (Exception ex) when (ex is FileNotFoundException || ex is ViewRenderingException)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var redirectUrl = _exRedirectService.BuildRedirectUrl(ex, requestId, HttpContext.Request.Path);
            string errorMessage = ex switch
            {
                FileNotFoundException => "Partial view file was not found during rendering.",
                ViewRenderingException => "An error occurred while rendering the partial view.",
                _ => "An unexpected rendering error occurred."
            };

            _logger.LogError(ex, "RequestId: {RequestId}", requestId);
#if DEBUG
            throw;
#else
            return Json(new { redirectTo = redirectUrl });
#endif
        }
        catch (Exception ex)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var redirectUrl = _exRedirectService.BuildRedirectUrl(ex, requestId, HttpContext.Request.Path);

            _logger.LogError(ex, "Unexpected error rendering partial views. RequestId: {RequestId}", requestId);
#if DEBUG
            throw;
#else
            return Json(new { redirectTo = redirectUrl });
#endif
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetGroupedTable([FromBody] Dictionary<string, object> parameters)
    {
        try
        {
            var systems = await _filterService.GetFilteredDataAsync(parameters);
            return PartialView("_PlanetarySystemTableInGroups", systems);
        }
        catch (Exception ex)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var redirectUrl = _exRedirectService.BuildRedirectUrl(ex, requestId, HttpContext.Request.Path);
#if DEBUG
            throw;
#else
            return Json(new { redirectTo = redirectUrl });
#endif
        }

    }

    [HttpPost]
    public async Task<IActionResult> GetFlatTable([FromBody] Dictionary<string, object> parameters)
    {
        try
        {
            var systems = await _filterService.GetFilteredDataAsync(parameters);
            return PartialView("_PlanetarySystemTable", systems);
        }
        catch (Exception ex)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var redirectUrl = _exRedirectService.BuildRedirectUrl(ex, requestId, HttpContext.Request.Path);
#if DEBUG
            throw;
#else
            return Json(new { redirectTo = redirectUrl });
#endif
        }
    }

}
