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
        //return View(await _context.Roles.ToListAsync());
        var roles = await _context.Roles
            .Include(r => r.RoleClaims)
            .Include(r => r.UserRoles)
            .ToListAsync();
        return View(roles);
    }

    // GET: Admin/Roles/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        //var aspNetRole = await _context.Roles
        //    .FirstOrDefaultAsync(m => m.Id == id);
        var aspNetRole = await _context.Roles
            .Include(r => r.RoleClaims)
            .Include(r => r.UserRoles)
            .ThenInclude(ur => ur.User)
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
        ViewData["Claims"] = new SelectList(_context.RoleClaims, "Id", "ClaimType");
        ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName");
        return View();
    }

    // POST: Admin/Roles/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create([Bind("Id,Name,NormalizedName,ConcurrencyStamp")] AspNetRole aspNetRole)
    public async Task<IActionResult> Create([Bind("Name,NormalizedName,ConcurrencyStamp")] AspNetRole aspNetRole, 
                                            string[] selectedClaims, string[] selectedUsers)
    {        
        if (ModelState.IsValid)
        {
            _context.Add(aspNetRole);

            foreach (var claimId in selectedClaims)
            {
                var claim = await _context.RoleClaims.FindAsync(int.Parse(claimId));
                if (claim != null)
                {
                    aspNetRole.RoleClaims.Add(claim);
                }
            }

            foreach (var userId in selectedUsers)
            {
                var userRole = new AspNetUserRole
                {
                    RoleId = aspNetRole.Id,
                    UserId = userId
                };
                aspNetRole.UserRoles.Add(userRole);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["Claims"] = new SelectList(_context.RoleClaims, "Id", "ClaimType", selectedClaims);
        ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName", selectedUsers);
        return View(aspNetRole);
    }

    // GET: Admin/Roles/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        //var aspNetRole = await _context.Roles.FindAsync(id);
        var aspNetRole = await _context.Roles
            .Include(r => r.RoleClaims)
            .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (aspNetRole == null)
        {
            return NotFound();
        }

        ViewData["Claims"] = new SelectList(_context.RoleClaims, "Id", "ClaimType", aspNetRole.RoleClaims?.Select(rc => rc.Id.ToString()) ?? new List<string>());
        ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName", aspNetRole.UserRoles?.Select(ur => ur.UserId) ?? new List<string>());

        return View(aspNetRole);
    }

    // POST: Admin/Roles/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(string id, [Bind("Id,Name,NormalizedName,ConcurrencyStamp")] AspNetRole aspNetRole)
    public async Task<IActionResult> Edit(string id, [Bind("Name,NormalizedName,ConcurrencyStamp")] AspNetRole aspNetRole, 
                                          string[] selectedClaims, string[] selectedUsers)
    {
        if (id != aspNetRole.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var existingRole = await _context.Roles
                .Include(r => r.RoleClaims)
                .Include(r => r.UserRoles)
                    .ThenInclude(ur => ur.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (existingRole == null)
            {
                return NotFound();
            }

            existingRole.Name = aspNetRole.Name;
            existingRole.NormalizedName = aspNetRole.NormalizedName;
            existingRole.ConcurrencyStamp = aspNetRole.ConcurrencyStamp;

            
            existingRole.RoleClaims.Clear();
            existingRole.UserRoles.Clear();
                        
            foreach (var claimId in selectedClaims)
            {
                var claim = await _context.RoleClaims.FindAsync(int.Parse(claimId));
                if (claim != null)
                {
                    existingRole.RoleClaims.Add(claim);
                }
            }

            foreach (var userId in selectedUsers)
            {
                var userRole = new AspNetUserRole
                {
                    RoleId = existingRole.Id,
                    UserId = userId
                };
                existingRole.UserRoles.Add(userRole);
            }

            try
            {
                //_context.Update(aspNetRole);
                _context.Update(existingRole);
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

        ViewData["Claims"] = new SelectList(_context.RoleClaims, "Id", "ClaimType", selectedClaims);
        ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName", selectedUsers);
        return View(aspNetRole);
    }

    // GET: Admin/Roles/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        //var aspNetRole = await _context.Roles
        //    .FirstOrDefaultAsync(m => m.Id == id);
        var aspNetRole = await _context.Roles
            .Include(r => r.RoleClaims)
            .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
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
