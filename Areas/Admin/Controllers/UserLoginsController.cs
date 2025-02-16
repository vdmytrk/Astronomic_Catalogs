using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Astronomic_Catalogs.Areas.Admin.Models;
using Astronomic_Catalogs.Data;

namespace Astronomic_Catalogs.Areas.Admin.Controllers
{
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
            var applicationDbContext = _context.AspNetUserLogin.Include(a => a.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/UserLogins/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspNetUserLogin = await _context.AspNetUserLogin
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.LoginProvider == id);
            if (aspNetUserLogin == null)
            {
                return NotFound();
            }

            return View(aspNetUserLogin);
        }

        // GET: Admin/UserLogins/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.AspNetUser, "Id", "Id");
            return View();
        }

        // POST: Admin/UserLogins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoginProvider,ProviderKey,ProviderDisplayName,UserId")] AspNetUserLogin aspNetUserLogin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(aspNetUserLogin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.AspNetUser, "Id", "Id", aspNetUserLogin.UserId);
            return View(aspNetUserLogin);
        }

        // GET: Admin/UserLogins/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspNetUserLogin = await _context.AspNetUserLogin.FindAsync(id);
            if (aspNetUserLogin == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.AspNetUser, "Id", "Id", aspNetUserLogin.UserId);
            return View(aspNetUserLogin);
        }

        // POST: Admin/UserLogins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("LoginProvider,ProviderKey,ProviderDisplayName,UserId")] AspNetUserLogin aspNetUserLogin)
        {
            if (id != aspNetUserLogin.LoginProvider)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aspNetUserLogin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AspNetUserLoginExists(aspNetUserLogin.LoginProvider))
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
            ViewData["UserId"] = new SelectList(_context.AspNetUser, "Id", "Id", aspNetUserLogin.UserId);
            return View(aspNetUserLogin);
        }

        // GET: Admin/UserLogins/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspNetUserLogin = await _context.AspNetUserLogin
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.LoginProvider == id);
            if (aspNetUserLogin == null)
            {
                return NotFound();
            }

            return View(aspNetUserLogin);
        }

        // POST: Admin/UserLogins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var aspNetUserLogin = await _context.AspNetUserLogin.FindAsync(id);
            if (aspNetUserLogin != null)
            {
                _context.AspNetUserLogin.Remove(aspNetUserLogin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AspNetUserLoginExists(string id)
        {
            return _context.AspNetUserLogin.Any(e => e.LoginProvider == id);
        }
    }
}
