using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Areas.Admin.Controllers;

[Area("Admin")]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserService _userService;

    public UsersController(ApplicationDbContext context, UserService userService)
    {
        _context = context;
        _userService = userService;
    }

    // GET: Admin/Users
    public async Task<IActionResult> Index()
    {
        var aspNetUser = await _context.Users
            .Include(u => u.UserClaims)
            .Include(u => u.UserRoles)
                .ThenInclude(r => r.Role)
            .Include(u => u.UserLogins)
            .Include(u => u.UserTokens)
            .ToListAsync();
        return View(aspNetUser.OrderBy(u => u.UserName).OrderBy(u => u.RegistrationDate));
    }

    // GET: Admin/Users/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetUser = await _context.Users
            .Include(u => u.UserClaims)
            .Include(u => u.UserRoles)
                .ThenInclude(r => r.Role)
            .Include(u => u.UserLogins)
            .Include(u => u.UserTokens)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (aspNetUser == null)
        {
            return NotFound();
        }

        return View(aspNetUser);
    }

    // GET: Admin/Users/Create
    public IActionResult Create()
    {
        ViewData["Roles"] = new SelectList(_context.Roles.Select(r => new { r.Id, r.Name }), "Id", "Name");

        return View();
    }

    // POST: Admin/Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("UserName,Email,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AspNetUser aspNetUser,
                                            string[] selectedRoles)
    {
        _userService.SetData(aspNetUser, selectedRoles);

        if (ModelState.IsValid)
        {

            _context.Add(aspNetUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["Roles"] = new SelectList(_context.Roles.Select(r => new { r.Id, r.Name }), "Id", "Name"); 
        return View(aspNetUser);
    }

    // GET: Admin/Users/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetUser = await _context.Users
            .Include(u => u.UserClaims)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.UserLogins)
            .Include(u => u.UserTokens)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (aspNetUser == null)
        {
            return NotFound();
        }

        ViewData["Roles"] = new SelectList(_context.Roles.Select(r => new { r.Id, r.Name }), "Id", "Name");

        return View(aspNetUser);
    }

    // POST: Admin/Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id,
                                          [Bind("Id,UserName,Email,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AspNetUser aspNetUser,
                                          string[] selectedRoles)
    {
        if (id != aspNetUser.Id)
        {
            return NotFound();
        }

        var existingUser = await _context.Users
            .Include(u => u.UserClaims)
            .Include(u => u.UserRoles)
                .ThenInclude(r => r.Role)
            .Include(u => u.UserLogins)
            .Include(u => u.UserTokens)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (existingUser == null)
        {
            return NotFound();
        }

        _userService.SetData(aspNetUser, selectedRoles, existingUser);


        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(existingUser);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetUserExists(existingUser.Id))
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

        ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name", selectedRoles);
        return View(existingUser);
    }

    // GET: Admin/Users/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetUser = await _context.Users
            .Include(u => u.UserClaims)
            .Include(u => u.UserRoles)
                .ThenInclude(r => r.Role)
            .Include(u => u.UserLogins)
            .Include(u => u.UserTokens)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (aspNetUser == null)
        {
            return NotFound();
        }

        return View(aspNetUser);
    }

    // POST: Admin/Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var aspNetUser = await _context.Users.FindAsync(id);
        if (aspNetUser != null)
        {
            _context.Users.Remove(aspNetUser);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AspNetUserExists(string id)
    {
        return _context.Users.Any(e => e.Id == id);
    }

}


