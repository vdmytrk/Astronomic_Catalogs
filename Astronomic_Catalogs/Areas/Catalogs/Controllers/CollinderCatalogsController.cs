using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.DTO;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Constants;
using Astronomic_Catalogs.Services.Interfaces;
using Astronomic_Catalogs.Utils;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text.RegularExpressions;

namespace Astronomic_Catalogs.Areas.Catalogs.Controllers
{
    [Area("Catalogs")]
    public class CollinderCatalogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICollinderFilterService _filterService;
        private readonly ICacheService _cache;

        public CollinderCatalogsController(ApplicationDbContext context, ICollinderFilterService filterService, ICacheService cache)
        {
            _context = context;
            _filterService = filterService;
            _cache = cache;
        }

        // GET: Catalogs/CollinderCatalogs
        public async Task<IActionResult> Index()
        {
            var (count, rawData, constellations) = await _filterService.GetCollinderCatalogDataAsync();

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
            int? pageNumber = parameters.GetInt("PageNumberVaulue");
            ViewBag.PageNumber = pageNumber == 0 || pageNumber == null ? 1 : pageNumber;

            List<CollinderCatalog>? selectedList;

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
            ViewBag.Contorller = "CollinderCatalogs";

            try
            {
                var tableHtml = await this.RenderViewAsync("_CollinderTable", selectedList, true);
                var paginationHtml = await this.RenderViewAsync("_PaginationLine", null, true);

                return Json(new { tableHtml, paginationHtml });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"RenderViewAsync error: {ex.Message}");
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

            var collinderCatalog = await _context.CollinderCatalog
                .FirstOrDefaultAsync(m => m.Id == id);
            if (collinderCatalog == null)
            {
                return NotFound();
            }

            return View(collinderCatalog);
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
                _context.Add(collinderCatalog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
                    // TODO: Clear cache
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CollinderCatalogExists(collinderCatalog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
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
            var collinderCatalog = await _context.CollinderCatalog.FindAsync(id);
            if (collinderCatalog != null)
            {
                _context.CollinderCatalog.Remove(collinderCatalog);
            }

            await _context.SaveChangesAsync();
            // TODO: Clear cache
            return RedirectToAction(nameof(Index));
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
}
