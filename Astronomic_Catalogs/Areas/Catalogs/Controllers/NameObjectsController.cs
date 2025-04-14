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
    public class NameObjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NameObjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Catalogs/NameObjects
        public async Task<IActionResult> Index()
        {
            return View(await _context.NameObjects.ToListAsync());
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

            var nameObject = await _context.NameObjects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (nameObject == null)
            {
                return NotFound();
            }

            return View(nameObject);
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
                _context.Add(nameObject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
                catch (DbUpdateConcurrencyException)
                {
                    if (!NameObjectExists(nameObject.Id))
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
            var nameObject = await _context.NameObjects.FindAsync(id);
            if (nameObject != null)
            {
                _context.NameObjects.Remove(nameObject);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NameObjectExists(int id)
        {
            return _context.NameObjects.Any(e => e.Id == id);
        }
    }
}
