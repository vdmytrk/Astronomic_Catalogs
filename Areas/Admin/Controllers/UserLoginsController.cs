using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Areas.Admin.Controllers;

[Area("Admin")]
public class UserLoginsController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserLoginsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/UserLogins
    public async Task<IActionResult> Index()
    {
        var aspNetUserLogin = _context.UserLogins.Include(a => a.User).OrderBy(ul => ul.LoginProvider);
        return View(await aspNetUserLogin.ToListAsync());
    }

    // GET: Admin/UserLogins/Details/5
    public async Task<IActionResult> Details(string loginProvider, string providerKey)
    {
        if (loginProvider == null || providerKey == null)
        {
            return NotFound();
        }

        var aspNetUserLogin = await _context.UserLogins
            .FirstOrDefaultAsync(m => m.LoginProvider == loginProvider && m.ProviderKey == providerKey);

        if (aspNetUserLogin == null)
        {
            return NotFound();
        }

        return View(aspNetUserLogin);
    }

    // GET: Admin/UserLogins/Create
    public IActionResult Create()
    {
        ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName");
        return View();
    }

    // POST: Admin/UserLogins/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("LoginProvider,ProviderKey,ProviderDisplayName,UserId")] AspNetUserLogin aspNetUserLogin)
    {
        ModelState.Remove("User");
        if (ModelState.IsValid)
        {
            _context.Add(aspNetUserLogin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", aspNetUserLogin.UserId);
        return View(aspNetUserLogin);
    }

    // GET: Admin/UserLogins/Edit/5
    public async Task<IActionResult> Edit(string loginProvider, string providerKey)
    {
        if (loginProvider == null || providerKey == null)
        {
            return NotFound();
        }

        var aspNetUserLogin = await _context.UserLogins
            .FirstOrDefaultAsync(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);


        if (aspNetUserLogin == null)
        {
            return NotFound();
        }

        ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", aspNetUserLogin.UserId);
        return View(aspNetUserLogin);
    }

    // POST: Admin/UserLogins/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string loginProvider, string providerKey, [Bind("LoginProvider,ProviderKey,ProviderDisplayName,UserId")] AspNetUserLogin aspNetUserLogin)
    {
        if (loginProvider != aspNetUserLogin.LoginProvider || providerKey != aspNetUserLogin.ProviderKey)
        {
            return NotFound();
        }

        ModelState.Remove("User");
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(aspNetUserLogin);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetUserLoginExists(aspNetUserLogin.LoginProvider, aspNetUserLogin.ProviderKey))
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

        ViewData["UserId"] = new SelectList(_context.Users, "Id", "UserName", aspNetUserLogin.UserId);
        return View(aspNetUserLogin);
    }

    // GET: Admin/UserLogins/Delete/5
    public async Task<IActionResult> Delete(string loginProvider, string providerKey)
    {
        if (loginProvider == null || providerKey == null)
        {
            return NotFound();
        }

        var aspNetUserLogin = await _context.UserLogins
            .FirstOrDefaultAsync(m => m.LoginProvider == loginProvider && m.ProviderKey == providerKey);

        if (aspNetUserLogin == null)
        {
            return NotFound();
        }

        return View(aspNetUserLogin);
    }

    // POST: Admin/UserLogins/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string loginProvider, string providerKey)
    {
        var aspNetUserLogin = await _context.UserLogins
            .FirstOrDefaultAsync(m => m.LoginProvider == loginProvider && m.ProviderKey == providerKey);

        if (aspNetUserLogin != null)
        {
            _context.UserLogins.Remove(aspNetUserLogin);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool AspNetUserLoginExists(string loginProvider, string providerKey)
    {
        return _context.UserLogins.Any(e => e.LoginProvider == loginProvider && e.ProviderKey == providerKey);
    }
}
