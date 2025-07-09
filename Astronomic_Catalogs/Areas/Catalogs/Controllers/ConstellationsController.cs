using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Astronomic_Catalogs.Areas.Catalogs.Controllers
{
    [Area("Catalogs")]
    public class ConstellationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ConstellationsController> _logger;

        public ConstellationsController(ApplicationDbContext context, ILogger<ConstellationsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Catalogs/Constellations
        public async Task<IActionResult> Index()
        {
            List<Constellation> constellations = new();
            try
            {
                constellations = await _context.Constellations.ToListAsync();
            }
            catch (Exception ex)
            {
                var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                _logger.LogError(
                    ex,
                    "An unexpected error occurred during data retrieval in ConstellationsController. RequestId: {RequestId}",
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

            return View(constellations);
        }

        // GET: Catalogs/Constellations/Details/5
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var constellation = await _context.Constellations
                .FirstOrDefaultAsync(m => m.ShortName == id);
                if (constellation == null)
                {
                    return NotFound();
                }

                return View(constellation);
            }
            catch (Exception ex)
            {
                var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
                _logger.LogError(ex, "Error retrieving details for Constellations by ID {Id}. RequestId: {RequestId}", id, requestId);

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

        // GET: Catalogs/Constellations/Create
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Catalogs/Constellations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShortName,LatineNameNominativeCase,LatineNameGenitiveCase,UkraineName,Area,NumberStars")] Constellation constellation)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(constellation);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database update error during creation of Constellations: {@Constellations}", constellation);
                    ModelState.AddModelError("", "Failed to save changes. Please try again later.");
                }
                catch (Exception ex)
                {
                    var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                    _logger.LogError(
                        ex,
                        "Unexpected error during creation of Constellations: {@Constellations}. RequestId: {RequestId}",
                        constellation,
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

            return View(constellation);
        }

        // GET: Catalogs/Constellations/Edit/5
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var constellation = await _context.Constellations.FindAsync(id);
            if (constellation == null)
            {
                return NotFound();
            }

            return View(constellation);
        }

        // POST: Catalogs/Constellations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ShortName,LatineNameNominativeCase,LatineNameGenitiveCase,UkraineName,Area,NumberStars")] Constellation constellation)
        {
            if (id != constellation.ShortName)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(constellation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException dbUpConcEx)
                {
                    if (!ConstellationExists(constellation.ShortName))
                    {
                        return NotFound();
                    }
                    var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
                    TempData["RequestId"] = requestId;
                    TempData["ErrorMessage"] = dbUpConcEx.Message;
                    TempData["StackTrace"] = dbUpConcEx.ToString();
                    TempData["Path"] = HttpContext.Request.Path.ToString();

                    _logger.LogError(
                        dbUpConcEx,
                        "Concurrency error occurred during editing Constellation: {@Constellation}. RequestId: {RequestId}",
                        constellation,
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
                    _logger.LogError(ex, "Database update error during editing Constellation: {@Constellation}", constellation);
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
                        "Unexpected error during editing Constellation: {@Constellation}. RequestId: {RequestId}",
                        constellation,
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

            return View(constellation);
        }

        // GET: Catalogs/Constellations/Delete/5
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var constellation = await _context.Constellations
               .FirstOrDefaultAsync(m => m.ShortName == id);
            if (constellation == null)
            {
                return NotFound();
            }

            return View(constellation);
        }

        // POST: Catalogs/Constellations/Delete/5
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var constellation = await _context.Constellations.FindAsync(id);
                if (constellation != null)
                {
                    _context.Constellations.Remove(constellation);
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
                    "Database update error during deletion of Constellation {Id}. RequestId: {RequestId}",
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
                _logger.LogError(ex, "Unexpected error during deletion of Constellation ID by {Id}", id);
                var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                TempData["RequestId"] = requestId;
                TempData["ErrorMessage"] = ex.Message;
                TempData["StackTrace"] = ex.ToString();
                TempData["Path"] = HttpContext.Request.Path.ToString();

                _logger.LogError(
                    ex,
                    "Unexpected error during deletion of Constellation ID by {Id}. RequestId: {RequestId}",
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

        private bool ConstellationExists(string id)
        {
            return _context.Constellations.Any(e => e.ShortName == id);
        }
    }
}
