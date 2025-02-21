using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
        var aspNetRole = await _context.Roles
            .Include(r => r.RoleClaims)
            .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
            .ToListAsync();
        return View(aspNetRole.OrderBy(r => r.Name));
    }

    // GET: Admin/Roles/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

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
        //ViewData["Claims"] = new SelectList(_context.RoleClaims, "Id", "ClaimType");
        ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName");
        return View();
    }

    // POST: Admin/Roles/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,NormalizedName,ConcurrencyStamp")] AspNetRole aspNetRole, string[] selectedUsers)
    {
        SetData(aspNetRole, selectedUsers);

        if (ModelState.IsValid)
        {
            _context.Add(aspNetRole);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //ViewData["Claims"] = new SelectList(_context.RoleClaims, "Id", "ClaimType", selectedClaims);
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

        var aspNetRole = await _context.Roles
            .Include(r => r.RoleClaims)
            .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (aspNetRole == null)
        {
            return NotFound();
        }

        //ViewData["Claims"] = new SelectList(_context.RoleClaims, "Id", "ClaimType", aspNetRole.RoleClaims?.Select(rc => rc.Id.ToString()) ?? new List<string>());
        ViewData["Users"] = new SelectList(_context.Users, "Id", "UserName", aspNetRole.UserRoles?.Select(ur => ur.UserId) ?? new List<string>());

        return View(aspNetRole);
    }

    // POST: Admin/Roles/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,Name,NormalizedName,ConcurrencyStamp")] AspNetRole aspNetRole, string[] selectedUsers)
    {
        if (id != aspNetRole.Id)
        {
            return NotFound();
        }

        var existingRole = await _context.Roles
            .Include(r => r.RoleClaims)
            .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (existingRole == null)
        {
            return NotFound();
        }

        SetData(aspNetRole, selectedUsers, existingRole);

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(existingRole);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetRoleExists(existingRole.Id))
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

        //ViewData["Claims"] = new SelectList(_context.RoleClaims, "Id", "ClaimType", selectedClaims);
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

    /// <summary>
    /// TODO: Check input values.
    /// </summary>
    private void SetData(AspNetRole inputRole, string[] selectedUsers, AspNetRole? existingRole = null)
    {
        var targetUser = existingRole ?? inputRole;

        targetUser.Name = inputRole.Name;
        targetUser.NormalizedName = inputRole.NormalizedName?.ToUpper();
        targetUser.ConcurrencyStamp = inputRole?.ConcurrencyStamp;

        // Оновлення користувачів у ролі
        var currentUserIds = targetUser.UserRoles.Select(ur => ur.UserId).ToList();
        var usersToRemove = targetUser.UserRoles.Where(ur => !selectedUsers.Contains(ur.UserId)).ToList();
        foreach (var userRole in usersToRemove)
        {
            targetUser.UserRoles.Remove(userRole);
        }
        foreach (var userId in selectedUsers)
        {
            if (!currentUserIds.Contains(userId))
            {
                targetUser.UserRoles.Add(new AspNetUserRole { RoleId = targetUser.Id, UserId = userId });
            }
        }
        #region Not usable
        //// Оновлення клеймів ролі
        //var currentClaimIds = existingRole.RoleClaims.Select(rc => rc.Id.ToString()).ToList();
        //var claimsToRemove = existingRole.RoleClaims.Where(rc => !selectedClaims.Contains(rc.Id.ToString())).ToList();
        //foreach (var claim in claimsToRemove)
        //{
        //    existingRole.RoleClaims.Remove(claim);
        //}
        //foreach (var claimId in selectedClaims)
        //{
        //    if (!currentClaimIds.Contains(claimId))
        //    {
        //        var claim = await _context.RoleClaims.FindAsync(int.Parse(claimId));
        //        if (claim != null)
        //        {
        //            existingRole.RoleClaims.Add(claim);
        //        }
        //    }
        //}
        #endregion
    }
}
