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
    public class NameObjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NameObjectsController> _logger;

        public NameObjectsController(ApplicationDbContext context, ILogger<NameObjectsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Catalogs/NameObjects
        public async Task<IActionResult> Index()
        {
            List<NameObject> nameObjects = new();
            try
            {
                nameObjects = await _context.NameObjects.ToListAsync();
            }
            catch (Exception ex)
            {
                var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                _logger.LogError(
                    ex,
                    "An unexpected error occurred during data retrieval in NameObjectsController. RequestId: {RequestId}",
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

            return View(nameObjects);
        }

        // GET: Catalogs/NameObjects/Details/5
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
                var nameObject = await _context.NameObjects
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (nameObject == null)
                {
                    return NotFound();
                }

                return View(nameObject);
            }
            catch (Exception ex)
            {
                var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
                _logger.LogError(ex, "Error retrieving details for NameObjects by ID {Id}. RequestId: {RequestId}", id, requestId);

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

        // GET: Catalogs/NameObjects/Create
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Catalogs/NameObjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Object,Name,Comment")] NameObject nameObject)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(nameObject);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database update error during creation of NameObject: {@NameObject}", nameObject);
                    ModelState.AddModelError("", "Failed to save changes. Please try again later.");
                }
                catch (Exception ex)
                {
                    var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                    _logger.LogError(
                        ex,
                        "Unexpected error during creation of NameObject: {@NameObject}. RequestId: {RequestId}",
                        nameObject,
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
            return View(nameObject);
        }

        // GET: Catalogs/NameObjects/Edit/5
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nameObject = await _context.NameObjects.FindAsync(id);
            if (nameObject == null)
            {
                return NotFound();
            }
            return View(nameObject);
        }

        // POST: Catalogs/NameObjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Object,Name,Comment")] NameObject nameObject)
        {
            if (id != nameObject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nameObject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException dbUpConcEx)
                {
                    if (!NameObjectExists(nameObject.Id))
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
                        "Concurrency error occurred during editing NameObject: {@NameObject}. RequestId: {RequestId}",
                        nameObject,
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
                    _logger.LogError(ex, "Database update error during editing NameObject: {@NameObject}", nameObject);
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
                        "Unexpected error during editing NameObject: {@NameObject}. RequestId: {RequestId}",
                        nameObject,
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

            return View(nameObject);
        }

        // GET: Catalogs/NameObjects/Delete/5
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nameObject = await _context.NameObjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nameObject == null)
            {
                return NotFound();
            }

            return View(nameObject);
        }

        // POST: Catalogs/NameObjects/Delete/5
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var nameObject = await _context.NameObjects.FindAsync(id);
                if (nameObject != null)
                {
                    _context.NameObjects.Remove(nameObject);
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
                    "Database update error during deletion of NameObjects {Id}. RequestId: {RequestId}",
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
                _logger.LogError(ex, "Unexpected error during deletion of NameObjects ID by {Id}", id);
                var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

                TempData["RequestId"] = requestId;
                TempData["ErrorMessage"] = ex.Message;
                TempData["StackTrace"] = ex.ToString();
                TempData["Path"] = HttpContext.Request.Path.ToString();

                _logger.LogError(
                    ex,
                    "Unexpected error during deletion of NameObjects ID by {Id}. RequestId: {RequestId}",
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

        private bool NameObjectExists(int id)
        {
            return _context.NameObjects.Any(e => e.Id == id);
        }
    }
}
