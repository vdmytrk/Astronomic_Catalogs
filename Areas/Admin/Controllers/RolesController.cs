using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Areas.Admin.Controllers;

[Area("Admin")]
public class RolesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly RoleService _roleService;

    public RolesController(ApplicationDbContext context, RoleService roleService)
    {
        _context = context;
        _roleService = roleService;
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
        _roleService.SetData(aspNetRole, selectedUsers);
        if (ModelState.IsValid)
        {
            _context.Add(aspNetRole);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
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

        _roleService.SetData(aspNetRole, selectedUsers, existingRole);

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

}
