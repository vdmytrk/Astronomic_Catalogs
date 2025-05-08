using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Constants;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using Astronomic_Catalogs.Utils;
using Astronomic_Catalogs.ViewModels;
using Astronomic_Catalogs.Services.Interfaces;
using AutoMapper;

namespace Astronomic_Catalogs.Areas.Catalogs.Controllers
{
    [Area("Catalogs")]
    public class CollinderCatalogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICollinderFilterService _filterService;

        public CollinderCatalogsController(ApplicationDbContext context, ICollinderFilterService filterService)
        {
            _context = context;
            _filterService = filterService;
        }

        // GET: Catalogs/CollinderCatalogs
        public async Task<IActionResult> Index()
        {
            var data = await _context.CollinderCatalog.ToListAsync();
            var count = data.Count;

            var sorted = data
                .OrderBy(x => ExtractLeadingNumber(x.NamberName))
                .ThenBy(x => x.NamberName)
                .Take(50)
                .ToList();

            var constellations = await _context.Constellations
            .Select(c => new
            {
                c.ShortName,
                c.LatineNameNominativeCase,
                c.EnglishName,
                c.UkraineName
            })
            .ToListAsync();


            ViewBag.RowsCount = count;
            ViewBag.Constellations = constellations;

            return View(sorted);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromBody] Dictionary<string, object> parameters)
        {            
            string rowOnPageCatalog = parameters.GetString("RowOnPageCatalog") ?? "50";
            ViewBag.RowOnPageCatalog = rowOnPageCatalog == "All" ? 500 : int.Parse(rowOnPageCatalog);

            List<CollinderCatalog> selectedList;

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
        public async Task<IActionResult> Create([Bind("Id,NamberName,NameOtherCat,Constellation,RightAscension,RightAscensionH,RightAscensionM,RightAscensionS,Declination,NS,DeclinationD,DeclinationM,DeclinationS,AppMag,AppMagFlag,CountStars,CountStarsToFinding,AngDiameter,AngDiameterNew,Class,Comment,PageNumber,PageCount")] CollinderCatalog collinderCatalog)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,NamberName,NameOtherCat,Constellation,RightAscension,RightAscensionH,RightAscensionM,RightAscensionS,Declination,NS,DeclinationD,DeclinationM,DeclinationS,AppMag,AppMagFlag,CountStars,CountStarsToFinding,AngDiameter,AngDiameterNew,Class,Comment,PageNumber,PageCount")] CollinderCatalog collinderCatalog)
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
