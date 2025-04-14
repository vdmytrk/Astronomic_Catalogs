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

namespace Astronomic_Catalogs.Areas.Catalogs.Controllers
{
    [Area("Catalogs")]
    public class SourceTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SourceTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Catalogs/SourceTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.SourceTypes.ToListAsync());
        }

        // GET: Catalogs/SourceTypes/Details/5
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sourceType = await _context.SourceTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sourceType == null)
            {
                return NotFound();
            }

            return View(sourceType);
        }

        // GET: Catalogs/SourceTypes/Create
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Catalogs/SourceTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Count,Code,Meaning")] SourceType sourceType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sourceType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sourceType);
        }

        // GET: Catalogs/SourceTypes/Edit/5
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sourceType = await _context.SourceTypes.FindAsync(id);
            if (sourceType == null)
            {
                return NotFound();
            }
            return View(sourceType);
        }

        // POST: Catalogs/SourceTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Count,Code,Meaning")] SourceType sourceType)
        {
            if (id != sourceType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sourceType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SourceTypeExists(sourceType.Id))
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
            return View(sourceType);
        }

        // GET: Catalogs/SourceTypes/Delete/5
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sourceType = await _context.SourceTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sourceType == null)
            {
                return NotFound();
            }

            return View(sourceType);
        }

        // POST: Catalogs/SourceTypes/Delete/5
        [Authorize(Roles = RoleNames.Admin)]
        [Authorize(Policy = "AdminPolicy")]
        [Authorize(Policy = "UsersAccessClaim")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sourceType = await _context.SourceTypes.FindAsync(id);
            if (sourceType != null)
            {
                _context.SourceTypes.Remove(sourceType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SourceTypeExists(int id)
        {
            return _context.SourceTypes.Any(e => e.Id == id);
        }
    }
}
