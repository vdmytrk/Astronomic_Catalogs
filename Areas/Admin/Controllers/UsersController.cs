using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace Astronomic_Catalogs.Areas.Admin.Controllers;

[Area("Admin")]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/Users
    public async Task<IActionResult> Index()
    {
        return View(await _context.Users.ToListAsync());
    }

    // GET: Admin/Users/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetUser = await _context.Users
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
        return View();
    }

    // POST: Admin/Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AspNetUser aspNetUser)
    {
        if (ModelState.IsValid)
        {
            _context.Add(aspNetUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(aspNetUser);
    }

    // GET: Admin/Users/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetUser = await _context.Users.FindAsync(id);
        if (aspNetUser == null)
        {
            return NotFound();
        }
        return View(aspNetUser);
    }

    // POST: Admin/Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,Email,PhoneNumber,NormalizedUserName,NormalizedEmail,EmailConfirmed,PhoneNumberConfirmed,LockoutEnd,LockoutEnabled,AccessFailedCount,TwoFactorEnabled,PasswordHash,SecurityStamp,ConcurrencyStamp")] AspNetUser aspNetUser)
    {
        if (id != aspNetUser.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(aspNetUser);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetUserExists(aspNetUser.Id))
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
        return View(aspNetUser);
    }

    // GET: Admin/Users/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetUser = await _context.Users
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

// Id,
// UserName,
// NormalizedUserName,
// Email,
// NormalizedEmail,
// EmailConfirmed,
// PasswordHash,
// SecurityStamp,
// ConcurrencyStamp,
// PhoneNumber,
// PhoneNumberConfirmed,
// TwoFactorEnabled,
// LockoutEnd,
// LockoutEnabled,
// AccessFailedCount

