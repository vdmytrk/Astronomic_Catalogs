using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Exceptions;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Constants;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using Astronomic_Catalogs.ViewModels;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Astronomic_Catalogs.Areas.Catalogs.Controllers;

[Area("Catalogs")]
public class CollinderCatalogsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ICollinderFilterService _filterService;
    private readonly ILogger<CollinderCatalogsController> _logger;
    private readonly IExceptionRedirectUrlService _exRedirectService;

    public CollinderCatalogsController(
        ApplicationDbContext context,
        ICollinderFilterService filterService,
        ILogger<CollinderCatalogsController> logger,
        IExceptionRedirectUrlService exceptionRedirectService)
    {
        _context = context;
        _filterService = filterService;
        _logger = logger;
        _exRedirectService = exceptionRedirectService;
    }

    // GET: Catalogs/CollinderCatalogs
    public async Task<IActionResult> Index()
    {
        int count;
        List<CollinderCatalog> rawData;
        List<ConstellationViewModel> constellations;

        try
        {
            (count, rawData, constellations) = await _filterService.GetCollinderCatalogDataAsync();
        }
        catch (Exception ex)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            string messageTemplate = "An unexpected error occurred during data retrieval in CollinderCatalogsController.";
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

        var data = rawData
            .OrderBy(x => ExtractLeadingNumber(x.NamberName))
            .ThenBy(x => x.NamberName)
            .ToList();

        ViewBag.RowOnPageCatalog = "50";
        ViewBag.AmountRowsResult = count;
        ViewBag.Constellations = constellations;

        return View(data);

    }

    [HttpPost]
    public async Task<IActionResult> Index([FromBody] Dictionary<string, object> parameters)
    {
        string rowOnPageCatalog = parameters.GetString("RowOnPageCatalog") ?? "50";
        ViewBag.RowOnPageCatalog = rowOnPageCatalog == "All" ? 500 : int.Parse(rowOnPageCatalog);
        int? pageNumber = parameters.GetInt("PageNumberValue");
        ViewBag.PageNumber = pageNumber == 0 || pageNumber == null ? 1 : pageNumber;
        List<CollinderCatalog>? selectedList;

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
        ViewBag.Contorller = "CollinderCatalogs";

        try
        {
            var tableHtml = await this.RenderViewAsync("_CollinderTable", selectedList, true);
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

    #region Optional methods
    // GET: Catalogs/CollinderCatalogs/Details/5
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var collinderCatalog = await _context.CollinderCatalog.FirstOrDefaultAsync(m => m.Id == id);
            if (collinderCatalog == null)
            {
                return NotFound();
            }

            return View(collinderCatalog);
        }
        catch (Exception ex)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError(ex, "Error retrieving details for CollinderCatalog by ID {Id}. RequestId: {RequestId}", id, requestId);

            TempData["RequestId"] = requestId;
            TempData["ErrorMessage"] = ex.Message;
            TempData["StackTrace"] = ex.ToString();
            TempData["Path"] = HttpContext.Request.Path.ToString();
#if DEBUG
            throw;
#else
            return StatusCode(500);
#endif
        }
    }

    // GET: Catalogs/CollinderCatalogs/Create
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Catalogs/CollinderCatalogs/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,NamberName,NameOtherCat,Constellation,RightAscension,RightAscensionH,RightAscensionM,RightAscensionS,Declination,NS,DeclinationD,DeclinationM,DeclinationS,AppMag,AppMagFlag,CountStars,CountStarsToFinding,AngDiameter,AngDiameterNew,Class,Comment,RowOnPage")] CollinderCatalog collinderCatalog)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(collinderCatalog);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error during creation of CollinderCatalog: {@Collinder}", collinderCatalog);
                ModelState.AddModelError("", "Failed to save changes. Please try again later.");
            }
            catch (Exception ex)
            {
                var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                _logger.LogError(
                    ex,
                    "Unexpected error during creation of CollinderCatalog: {@Collinder}. RequestId: {RequestId}",
                    collinderCatalog,
                    requestId
                );

                TempData["RequestId"] = requestId;
                TempData["ErrorMessage"] = ex.Message;
                TempData["StackTrace"] = ex.ToString();
                TempData["Path"] = HttpContext.Request.Path.ToString();
#if DEBUG
                throw;
#else
                return StatusCode(500);
#endif
            }
        }
        return View(collinderCatalog);
    }

    // GET: Catalogs/CollinderCatalogs/Edit/5
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var collinderCatalog = await _context.CollinderCatalog.FindAsync(id);
        if (collinderCatalog == null)
        {
            return NotFound();
        }

        return View(collinderCatalog);
    }

    // POST: Catalogs/CollinderCatalogs/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,NamberName,NameOtherCat,Constellation,RightAscension,RightAscensionH,RightAscensionM,RightAscensionS,Declination,NS,DeclinationD,DeclinationM,DeclinationS,AppMag,AppMagFlag,CountStars,CountStarsToFinding,AngDiameter,AngDiameterNew,Class,Comment,RowOnPage")] CollinderCatalog collinderCatalog)
    {
        if (id != collinderCatalog.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(collinderCatalog);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException dbUpConcEx)
            {
                if (!CollinderCatalogExists(collinderCatalog.Id))
                    return NotFound();

                var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                TempData["RequestId"] = requestId;
                TempData["ErrorMessage"] = dbUpConcEx.Message;
                TempData["StackTrace"] = dbUpConcEx.ToString();
                TempData["Path"] = HttpContext.Request.Path.ToString();

                _logger.LogError(
                    dbUpConcEx,
                    "Concurrency error occurred during editing CollinderCatalog: {@Collinder}. RequestId: {RequestId}",
                    collinderCatalog,
                    requestId
                );
                TempData["IsLogged"] = true;
#if DEBUG
                throw;
#else
                return RedirectToAction("Error", "Error");
#endif
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error during editing CollinderCatalog: {@Collinder}", collinderCatalog);
                ModelState.AddModelError("", "Failed to save changes. Please try again later.");
            }
            catch (Exception ex)
            {
                var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                TempData["RequestId"] = requestId;
                TempData["ErrorMessage"] = ex.Message;
                TempData["StackTrace"] = ex.ToString();
                TempData["Path"] = HttpContext.Request.Path.ToString();

                _logger.LogError(
                    ex,
                    "Unexpected error during editing CollinderCatalog: {@Collinder}. RequestId: {RequestId}",
                    collinderCatalog,
                    requestId
                );
#if DEBUG
                throw;
#else
                return StatusCode(500);
#endif
            }

            return RedirectToAction(nameof(Index));
        }

        return View(collinderCatalog);
    }

    // GET: Catalogs/CollinderCatalogs/Delete/5
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var collinderCatalog = await _context.CollinderCatalog
            .FirstOrDefaultAsync(m => m.Id == id);
        if (collinderCatalog == null)
        {
            return NotFound();
        }

        return View(collinderCatalog);
    }

    // POST: Catalogs/CollinderCatalogs/Delete/5
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var collinderCatalog = await _context.CollinderCatalog.FindAsync(id);
            if (collinderCatalog != null)
            {
                _context.CollinderCatalog.Remove(collinderCatalog);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            TempData["RequestId"] = requestId;
            TempData["ErrorMessage"] = ex.Message;
            TempData["StackTrace"] = ex.ToString();
            TempData["Path"] = HttpContext.Request.Path.ToString();

            _logger.LogError(
                ex, 
                "Database update error during deletion of CollinderCatalog by ID {Id}. RequestId: {RequestId}", 
                id, 
                requestId
                
            );
#if DEBUG
            throw;
#else
            return StatusCode(500);
#endif
        }
        catch (Exception ex)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            TempData["RequestId"] = requestId;
            TempData["ErrorMessage"] = ex.Message;
            TempData["StackTrace"] = ex.ToString();
            TempData["Path"] = HttpContext.Request.Path.ToString();

            _logger.LogError(
                ex, 
                "Unexpected error during deletion of CollinderCatalog ID by {Id}. RequestId: {RequestId}", 
                id, 
                requestId
            );
#if DEBUG
            throw;
#else
            return StatusCode(500);
#endif
        }
    }
    #endregion

    private bool CollinderCatalogExists(int id)
    {
        return _context.CollinderCatalog.Any(e => e.Id == id);
    }

    private int ExtractLeadingNumber(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return int.MaxValue;

        var match = Regex.Match(input, @"^\d+");
        return match.Success ? int.Parse(match.Value) : int.MaxValue;
    }

}
