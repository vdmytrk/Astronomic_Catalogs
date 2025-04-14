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

namespace Astronomic_Catalogs.Areas.Catalogs.Controllers
{
    [Area("Catalogs")]
    public class CollinderCatalogsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CollinderCatalogsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Catalogs/CollinderCatalogs
        public async Task<IActionResult> Index()
        {
            return View(await _context.CollinderCatalog.OrderBy(x => x.NamberName).ToListAsync());
        }

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
        public async Task<IActionResult> Create([Bind("Id,NamberName,NameOtherCat,Constellation,RightAscension,RightAscensionH,RightAscensionM,RightAscensionS,Declination,NS,DeclinationD,DeclinationM,DeclinationS,AppMag,AppMagFlag,CountStars,CountStarsToFinding,AngDiameterOld,AngDiameterNew,Class,Comment,PageNumber,PageCount")] CollinderCatalog collinderCatalog)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,NamberName,NameOtherCat,Constellation,RightAscension,RightAscensionH,RightAscensionM,RightAscensionS,Declination,NS,DeclinationD,DeclinationM,DeclinationS,AppMag,AppMagFlag,CountStars,CountStarsToFinding,AngDiameterOld,AngDiameterNew,Class,Comment,PageNumber,PageCount")] CollinderCatalog collinderCatalog)
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
