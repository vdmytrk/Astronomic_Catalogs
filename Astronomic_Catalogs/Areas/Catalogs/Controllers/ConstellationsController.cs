using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;

namespace Astronomic_Catalogs.Areas.Catalogs.Controllers
{
    [Area("Catalogs")]
    public class ConstellationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConstellationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Catalogs/Constellations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Constellations.ToListAsync());
        }

        // GET: Catalogs/Constellations/Details/5
        public async Task<IActionResult> Details(string id)
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

        // GET: Catalogs/Constellations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Catalogs/Constellations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShortName,LatineNameNominativeCase,LatineNameGenitiveCase,UkraineName,Area,NumberStars")] Constellation constellation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(constellation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(constellation);
        }

        // GET: Catalogs/Constellations/Edit/5
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
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConstellationExists(constellation.ShortName))
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
            return View(constellation);
        }

        // GET: Catalogs/Constellations/Delete/5
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var constellation = await _context.Constellations.FindAsync(id);
            if (constellation != null)
            {
                _context.Constellations.Remove(constellation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConstellationExists(string id)
        {
            return _context.Constellations.Any(e => e.ShortName == id);
        }
    }
}
