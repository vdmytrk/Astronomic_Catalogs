using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Astronomic_Catalogs.Areas.Admin.Models;
using Astronomic_Catalogs.Data;

namespace Astronomic_Catalogs.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserRolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserRolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/UserRoles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AspNetUserRole.Include(a => a.Role).Include(a => a.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/UserRoles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspNetUserRole = await _context.AspNetUserRole
                .Include(a => a.Role)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (aspNetUserRole == null)
            {
                return NotFound();
            }

            return View(aspNetUserRole);
        }

        // GET: Admin/UserRoles/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.AspNetRole, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.AspNetUser, "Id", "Id");
            return View();
        }

        // POST: Admin/UserRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,RoleId")] AspNetUserRole aspNetUserRole)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aspNetUserRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.AspNetRole, "Id", "Id", aspNetUserRole.RoleId);
            ViewData["UserId"] = new SelectList(_context.AspNetUser, "Id", "Id", aspNetUserRole.UserId);
            return View(aspNetUserRole);
        }

        // GET: Admin/UserRoles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspNetUserRole = await _context.AspNetUserRole.FindAsync(id);
            if (aspNetUserRole == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.AspNetRole, "Id", "Id", aspNetUserRole.RoleId);
            ViewData["UserId"] = new SelectList(_context.AspNetUser, "Id", "Id", aspNetUserRole.UserId);
            return View(aspNetUserRole);
        }

        // POST: Admin/UserRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,RoleId")] AspNetUserRole aspNetUserRole)
        {
            if (id != aspNetUserRole.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aspNetUserRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AspNetUserRoleExists(aspNetUserRole.UserId))
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
            ViewData["RoleId"] = new SelectList(_context.AspNetRole, "Id", "Id", aspNetUserRole.RoleId);
            ViewData["UserId"] = new SelectList(_context.AspNetUser, "Id", "Id", aspNetUserRole.UserId);
            return View(aspNetUserRole);
        }

        // GET: Admin/UserRoles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspNetUserRole = await _context.AspNetUserRole
                .Include(a => a.Role)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (aspNetUserRole == null)
            {
                return NotFound();
            }

            return View(aspNetUserRole);
        }

        // POST: Admin/UserRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var aspNetUserRole = await _context.AspNetUserRole.FindAsync(id);
            if (aspNetUserRole != null)
            {
                _context.AspNetUserRole.Remove(aspNetUserRole);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AspNetUserRoleExists(string id)
        {
            return _context.AspNetUserRole.Any(e => e.UserId == id);
        }
    }
}
