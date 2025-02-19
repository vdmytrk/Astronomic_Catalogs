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
public class UserTokensController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserTokensController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/UserTokens
    public async Task<IActionResult> Index()
    {
        var aspNetUserToken = _context.UserTokens.Include(a => a.User).OrderBy(ul => ul.UserId);
        return View(await aspNetUserToken.ToListAsync());
    }

    // GET: Admin/UserTokens/Details/5
    public async Task<IActionResult> Details(string UserId, string loginProvider)
    {
        if (UserId == null|| loginProvider == null)
        {
            return NotFound();
        }

        var aspNetUserToken = await _context.UserTokens
            .FirstOrDefaultAsync(m => m.UserId == UserId && m.LoginProvider == loginProvider);

        if (aspNetUserToken == null)
        {
            return NotFound();
        }

        return View(aspNetUserToken);
    }

    // GET: Admin/UserTokens/Create
    public IActionResult Create()
    {
        ViewData["userId"] = new SelectList(_context.Users, "Id", "UserName");
        return View();
    }

    // POST: Admin/UserTokens/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("UserId,LoginProvider,Name,Value")] AspNetUserToken aspNetUserToken)
    {
        ModelState.Remove("User");
        if (ModelState.IsValid)
        {
            _context.Add(aspNetUserToken);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["userId"] = new SelectList(_context.Users, "Id", "UserName", aspNetUserToken.UserId);
        return View(aspNetUserToken);
    }

    // GET: Admin/UserTokens/Edit/5
    public async Task<IActionResult> Edit(string userId, string loginProvider)
    {
        if (userId == null || loginProvider == null)
        {
            return NotFound();
        }

        var aspNetUserToken = await _context.UserTokens
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LoginProvider == loginProvider);

        if (aspNetUserToken == null)
        {
            return NotFound();
        }

        ViewData["userId"] = new SelectList(_context.Users, "Id", "UserName", aspNetUserToken.UserId);
        return View(aspNetUserToken);
    }

    // POST: Admin/UserTokens/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string userId, string loginProvider, [Bind("UserId,LoginProvider,Name,Value")] AspNetUserToken aspNetUserToken)
    {
        if (userId == null || loginProvider == null)
        {
            return NotFound();
        }

        ModelState.Remove("User");
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(aspNetUserToken);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetUserTokenExists(aspNetUserToken.UserId, aspNetUserToken.LoginProvider))
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

        ViewData["userId"] = new SelectList(_context.Users, "Id", "UserName", aspNetUserToken.UserId);
        return View(aspNetUserToken);
    }

    // GET: Admin/UserTokens/Delete/5
    public async Task<IActionResult> Delete(string userId, string loginProvider)
    {
        if (userId == null || loginProvider == null)
        {
            return NotFound();
        }

        var aspNetUserToken = await _context.UserTokens
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LoginProvider == loginProvider);

        if (aspNetUserToken == null)
        {
            return NotFound();
        }

        return View(aspNetUserToken);
    }

    // POST: Admin/UserTokens/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string userId, string loginProvider)
    {
        var aspNetUserToken = await _context.UserTokens
            .FirstOrDefaultAsync(p => p.UserId == userId && p.LoginProvider == loginProvider);

        if (aspNetUserToken != null)
        {
            _context.UserTokens.Remove(aspNetUserToken);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool AspNetUserTokenExists(string userId, string loginProvider)
    {
        return _context.UserLogins.Any(e => e.UserId == userId && e.LoginProvider == loginProvider);
    }
}
