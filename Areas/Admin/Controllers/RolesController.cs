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
public class RolesController : Controller
{
    private readonly ApplicationDbContext _context;

    public RolesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Roles
    public async Task<IActionResult> Index()
    {
        return View(await _context.Roles.ToListAsync());
    }

    // GET: Admin/Roles/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetRole = await _context.Roles
            .FirstOrDefaultAsync(m => m.Id == id);
        if (aspNetRole == null)
        {
            return NotFound();
        }

        return View(aspNetRole);
    }

    // GET: Admin/Roles/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/Roles/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,NormalizedName,ConcurrencyStamp")] AspNetRole aspNetRole)
    {
        if (ModelState.IsValid)
        {
            _context.Add(aspNetRole);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(aspNetRole);
    }

    // GET: Admin/Roles/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetRole = await _context.Roles.FindAsync(id);
        if (aspNetRole == null)
        {
            return NotFound();
        }
        return View(aspNetRole);
    }

    // POST: Admin/Roles/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,Name,NormalizedName,ConcurrencyStamp")] AspNetRole aspNetRole)
    {
        if (id != aspNetRole.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(aspNetRole);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetRoleExists(aspNetRole.Id))
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
        return View(aspNetRole);
    }

    // GET: Admin/Roles/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetRole = await _context.Roles
            .FirstOrDefaultAsync(m => m.Id == id);
        if (aspNetRole == null)
        {
            return NotFound();
        }

        return View(aspNetRole);
    }

    // POST: Admin/Roles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var aspNetRole = await _context.Roles.FindAsync(id);
        if (aspNetRole != null)
        {
            _context.Roles.Remove(aspNetRole);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AspNetRoleExists(string id)
    {
        return _context.Roles.Any(e => e.Id == id);
    }
}
