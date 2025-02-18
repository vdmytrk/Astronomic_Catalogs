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
public class UserClaimsController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserClaimsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/UserClaims
    public async Task<IActionResult> Index()
    {
        return View(await _context.UserClaims.ToListAsync());
    }

    // GET: Admin/UserClaims/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetUserClaim = await _context.UserClaims
            .FirstOrDefaultAsync(m => m.Id == id);
        if (aspNetUserClaim == null)
        {
            return NotFound();
        }

        return View(aspNetUserClaim);
    }

    // GET: Admin/UserClaims/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/UserClaims/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,UserId,ClaimType,ClaimValue")] AspNetUserClaim aspNetUserClaim)
    {
        if (ModelState.IsValid)
        {
            _context.Add(aspNetUserClaim);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(aspNetUserClaim);
    }

    // GET: Admin/UserClaims/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetUserClaim = await _context.UserClaims.FindAsync(id);
        if (aspNetUserClaim == null)
        {
            return NotFound();
        }
        return View(aspNetUserClaim);
    }

    // POST: Admin/UserClaims/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ClaimType,ClaimValue")] AspNetUserClaim aspNetUserClaim)
    {
        if (id != aspNetUserClaim.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(aspNetUserClaim);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetUserClaimExists(aspNetUserClaim.Id))
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
        return View(aspNetUserClaim);
    }

    // GET: Admin/UserClaims/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetUserClaim = await _context.UserClaims
            .FirstOrDefaultAsync(m => m.Id == id);
        if (aspNetUserClaim == null)
        {
            return NotFound();
        }

        return View(aspNetUserClaim);
    }

    // POST: Admin/UserClaims/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var aspNetUserClaim = await _context.UserClaims.FindAsync(id);
        if (aspNetUserClaim != null)
        {
            _context.UserClaims.Remove(aspNetUserClaim);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AspNetUserClaimExists(int id)
    {
        return _context.UserClaims.Any(e => e.Id == id);
    }
}
