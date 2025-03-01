using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;

namespace Astronomic_Catalogs.Areas.Admin.Controllers;

[Area("Admin")]
public class RoleClaimsController : Controller
{
    private readonly ApplicationDbContext _context;

    public RoleClaimsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/RoleClaims
    public async Task<IActionResult> Index()
    {
        var aspNetRoleClaim = _context.RoleClaims.Include(a => a.Role).OrderBy(rc => rc.Id);
        return View(await aspNetRoleClaim.ToListAsync());
    }

    // GET: Admin/RoleClaims/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetRoleClaim = await _context.RoleClaims
            .Include(a => a.Role)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (aspNetRoleClaim == null)
        {
            return NotFound();
        }

        return View(aspNetRoleClaim);
    }

    // GET: Admin/RoleClaims/Create
    public IActionResult Create()
    {
        ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
        return View();
    }

    // POST: Admin/RoleClaims/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("RoleId,ClaimType,ClaimValue")] AspNetRoleClaim aspNetRoleClaim)
    {
        ModelState.Remove("Role");
        if (ModelState.IsValid)
        {
            _context.Add(aspNetRoleClaim);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", aspNetRoleClaim.RoleId);
        return View(aspNetRoleClaim);
    }

    // GET: Admin/RoleClaims/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetRoleClaim = await _context.RoleClaims.FindAsync(id);
        if (aspNetRoleClaim == null)
        {
            return NotFound();
        }
        ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", aspNetRoleClaim.RoleId);
        return View(aspNetRoleClaim);
    }

    // POST: Admin/RoleClaims/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,RoleId,ClaimType,ClaimValue")] AspNetRoleClaim aspNetRoleClaim)
    {
        if (id != aspNetRoleClaim.Id)
        {
            return NotFound();
        }

        ModelState.Remove("Role");
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(aspNetRoleClaim);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetRoleClaimExists(aspNetRoleClaim.Id))
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

        ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", aspNetRoleClaim.RoleId);
        return View(aspNetRoleClaim);
    }

    // GET: Admin/RoleClaims/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetRoleClaim = await _context.RoleClaims
            .Include(a => a.Role)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (aspNetRoleClaim == null)
        {
            return NotFound();
        }

        return View(aspNetRoleClaim);
    }

    // POST: Admin/RoleClaims/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var aspNetRoleClaim = await _context.RoleClaims.FindAsync(id);
        if (aspNetRoleClaim != null)
        {
            _context.RoleClaims.Remove(aspNetRoleClaim);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AspNetRoleClaimExists(int id)
    {
        return _context.RoleClaims.Any(e => e.Id == id);
    }
}
