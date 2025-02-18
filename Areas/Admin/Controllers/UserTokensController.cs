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
        return View(await _context.UserTokens.ToListAsync());
    }

    // GET: Admin/UserTokens/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetUserToken = await _context.UserTokens
            .FirstOrDefaultAsync(m => m.UserId == id);
        if (aspNetUserToken == null)
        {
            return NotFound();
        }

        return View(aspNetUserToken);
    }

    // GET: Admin/UserTokens/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Admin/UserTokens/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("UserId,LoginProvider,Name,Value")] AspNetUserToken aspNetUserToken)
    {
        if (ModelState.IsValid)
        {
            _context.Add(aspNetUserToken);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(aspNetUserToken);
    }

    // GET: Admin/UserTokens/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetUserToken = await _context.UserTokens.FindAsync(id);
        if (aspNetUserToken == null)
        {
            return NotFound();
        }
        return View(aspNetUserToken);
    }

    // POST: Admin/UserTokens/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("UserId,LoginProvider,Name,Value")] AspNetUserToken aspNetUserToken)
    {
        if (id != aspNetUserToken.UserId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(aspNetUserToken);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetUserTokenExists(aspNetUserToken.UserId))
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
        return View(aspNetUserToken);
    }

    // GET: Admin/UserTokens/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetUserToken = await _context.UserTokens
            .FirstOrDefaultAsync(m => m.UserId == id);
        if (aspNetUserToken == null)
        {
            return NotFound();
        }

        return View(aspNetUserToken);
    }

    // POST: Admin/UserTokens/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var aspNetUserToken = await _context.UserTokens.FindAsync(id);
        if (aspNetUserToken != null)
        {
            _context.UserTokens.Remove(aspNetUserToken);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool AspNetUserTokenExists(string id)
    {
        return _context.UserTokens.Any(e => e.UserId == id);
    }
}
