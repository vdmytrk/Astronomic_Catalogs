using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Exceptions;
using Astronomic_Catalogs.Mappers;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Constants;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using Astronomic_Catalogs.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Astronomic_Catalogs.Areas.Catalogs.Controllers;

[Area("Catalogs")]
public class NGCICOpendatasoftsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly INGCICFilterService _filterService;
    private readonly ILogger<NGCICOpendatasoftsController> _logger;
    private readonly IExceptionRedirectUrlService _exRedirectService;

    public NGCICOpendatasoftsController(
        ApplicationDbContext context,
        INGCICFilterService filterService,
        ILogger<NGCICOpendatasoftsController> logger,
        IExceptionRedirectUrlService exceptionRedirectService)
    {
        _context = context;
        _filterService = filterService;
        _logger = logger;
        _exRedirectService = exceptionRedirectService;
    }

    // GET: Catalogs/NGCICOpendatasofts
    public async Task<IActionResult> Index()
    {
        int countNGC;
        int countNGCE;
        List<ConstellationViewModel>? constellations;
        List<NGCICOpendatasoft>? catalogItems;
        List<NGCICViewModel> catalogViewModels;

        try
        {
            (countNGC, countNGCE, constellations, catalogItems) = await _filterService.GetNGCICOpendatasoftDataAsync();
            catalogViewModels = catalogItems.ToViewModelList();
        }
        catch (Exception ex)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            string messageTemplate = $"An unexpected error occurred during data retrieval AmountRowsResult in NGCICOpendatasoftsController.";
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

        ViewBag.RowOnPageCatalog = "50";
        ViewBag.AmountRowsResult = countNGC + countNGCE;
        ViewBag.Constellations = constellations;

        return View(catalogViewModels);
    }

    [HttpPost]
    public async Task<IActionResult> Index([FromBody] Dictionary<string, object> parameters)
    {
        ViewBag.RowOnPageCatalog = parameters.GetString("RowOnPageCatalog") ?? "50";
        int? pageNumber = parameters.GetInt("PageNumberValue");
        ViewBag.PageNumber = pageNumber == 0 || pageNumber == null ? 1 : pageNumber;
        List<NGCICOpendatasoft>? selectedList;

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

        var viewModelList = selectedList.ToViewModelList();
        var firstItem = viewModelList.FirstOrDefault();

        // TODO: Use Dapper to return an output parameter
        ViewBag.AmountRowsResult = firstItem?.RowOnPage ?? 0; // Using the RowOnPage field of the database table to pass a value.
        ViewBag.Contorller = "NGCICOpendatasofts";

        try
        {
            var tableHtml = await this.RenderViewAsync("_NGCICTableHeaders", viewModelList, true);
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
    // GET: Catalogs/NGCICOpendatasofts/Details/5
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
            var entity = await _context.NGCIC_Catalog.FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null) return NotFound();

            var viewModel = entity.ToViewModel();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            _logger.LogError(ex, "Error retrieving details for NGCICOpendatasofts by ID {Id}. RequestId: {RequestId}", id, requestId);

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

    // GET: Catalogs/NGCICOpendatasofts/Create
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    public IActionResult Create()
    {
        var model = new NGCICViewModel();
        return View(model);
    }

    // POST: Catalogs/NGCICOpendatasofts/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NGCICViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var entity = viewModel.ToEntity();
                _context.Add(entity);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error during creation of NGCICOpendatasoft: {@NGCICOpendatasoft}", viewModel);
                ModelState.AddModelError("", "Failed to save changes. Please try again later.");
            }
            catch (Exception ex)
            {
                var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                _logger.LogError(
                    ex,
                    "Unexpected error during creation of NGCICOpendatasoft: {@NGCICOpendatasoft}. RequestId: {RequestId}",
                    viewModel,
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

        return View(viewModel);
    }

    // GET: Catalogs/NGCICOpendatasofts/Edit/5
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var entity = await _context.NGCIC_Catalog.FirstOrDefaultAsync(m => m.Id == id);
        if (entity == null) return NotFound();

        var viewModel = entity.ToViewModel();
        return View(viewModel);
    }

    // POST: Catalogs/NGCICOpendatasofts/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NGCICViewModel viewModel)
    {
        if (id != viewModel.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var entity = viewModel.ToEntity();
                _context.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException dbUpConcEx)
            {
                if (!NGCICOpendatasoftExists(viewModel.Id))
                    return NotFound();

                var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                TempData["RequestId"] = requestId;
                TempData["ErrorMessage"] = dbUpConcEx.Message;
                TempData["StackTrace"] = dbUpConcEx.ToString();
                TempData["Path"] = HttpContext.Request.Path.ToString();

                _logger.LogError(
                    dbUpConcEx,
                    "Concurrency error occurred during editing NGCICOpendatasoft: {@NGCICOpendatasoft}. RequestId: {RequestId}",
                    viewModel,
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
                _logger.LogError(ex, "Database update error during editing NGCICOpendatasoft: {@NGCICOpendatasoft}", viewModel);
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
                    "Unexpected error during editing NGCICOpendatasoft: {@NGCICOpendatasoft}. RequestId: {RequestId}",
                    viewModel,
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
        return View(viewModel);
    }

    // GET: Catalogs/NGCICOpendatasofts/Delete/5
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var entity = await _context.NGCIC_Catalog.FirstOrDefaultAsync(m => m.Id == id);
        if (entity == null) return NotFound();

        var viewModel = entity.ToViewModel();
        return View(viewModel);
    }

    // POST: Catalogs/NGCICOpendatasofts/Delete/5
    [Authorize(Roles = RoleNames.Admin)]
    [Authorize(Policy = "AdminPolicy")]
    [Authorize(Policy = "UsersAccessClaim")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var nGCICOpendatasoft = await _context.NGCIC_Catalog.FindAsync(id);
            if (nGCICOpendatasoft != null)
            {
                _context.NGCIC_Catalog.Remove(nGCICOpendatasoft);
            }

            await _context.SaveChangesAsync();
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
                "Database update error during deletion of NGCICOpendatasofts by ID {Id}. RequestId: {RequestId}",
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
                "Unexpected error during deletion of NGCICOpendatasofts ID by {Id}. RequestId: {RequestId}", 
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

    private bool NGCICOpendatasoftExists(int id)
    {
        return _context.NGCIC_Catalog.Any(e => e.Id == id);
    }

}
