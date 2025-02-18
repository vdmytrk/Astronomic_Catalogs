using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol.Plugins;

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
        var aspNetUser = await _context.Users
            .Include(u => u.UserClaims)
            .Include(u => u.UserRoles)
                .ThenInclude(r => r.Role)
            .Include(u => u.UserLogins)
            .Include(u => u.UserTokens)
            .ToListAsync();
        return View(aspNetUser);
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
        ViewData["Claims"] = new SelectList(_context.UserClaims.Select(c => c.ClaimType).Distinct());
        ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name");
        ViewData["Logins"] = new SelectList(_context.UserLogins.Select(l => l.ProviderDisplayName).Distinct());
        ViewData["Tokens"] = new SelectList(_context.UserTokens.Select(t => t.Name).Distinct());

        return View();
    }

    // POST: Admin/Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("UserName,Email,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AspNetUser aspNetUser,
                                            string[] selectedClaims,
                                            string[] selectedRoles,
                                            string[] selectedLogins,
                                            string[] selectedTokens)
    {
        if (ModelState.IsValid)
        {
            aspNetUser = ValidateInputData(aspNetUser, selectedClaims, selectedRoles, selectedLogins, selectedTokens);
            
            _context.Add(aspNetUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["Claims"] = new SelectList(_context.UserClaims.Select(c => c.ClaimType).Distinct(), selectedClaims);
        ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name", selectedRoles);
        ViewData["Logins"] = new SelectList(_context.UserLogins.Select(l => l.ProviderDisplayName).Distinct(), selectedLogins);
        ViewData["Tokens"] = new SelectList(_context.UserTokens.Select(t => t.Name).Distinct(), selectedTokens);

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

        ViewData["Claims"] = new SelectList(_context.UserClaims.Select(c => c.ClaimType).Distinct());
        ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name");
        ViewData["Logins"] = new SelectList(_context.UserLogins.Select(l => l.ProviderDisplayName).Distinct());
        ViewData["Tokens"] = new SelectList(_context.UserTokens.Select(t => t.Name).Distinct());

        return View(aspNetUser);
    }

    // POST: Admin/Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("UserName,Email,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AspNetUser aspNetUser,
                                          string[] selectedClaims,
                                          string[] selectedRoles,
                                          string[] selectedLogins,
                                          string[] selectedTokens)
    {
        if (id != aspNetUser.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
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

            existingUser = ValidateInputData(aspNetUser, selectedClaims, selectedRoles, selectedLogins, selectedTokens);

            try
            {
                _context.Update(existingUser);
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

        ViewData["Claims"] = new SelectList(_context.UserClaims.Select(c => c.ClaimType).Distinct(), selectedClaims);
        ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name", selectedRoles);
        ViewData["Logins"] = new SelectList(_context.UserLogins.Select(l => l.ProviderDisplayName).Distinct(), selectedLogins);
        ViewData["Tokens"] = new SelectList(_context.UserTokens.Select(t => t.Name).Distinct(), selectedTokens);

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

    private AspNetUser ValidateInputData(AspNetUser aspNetUser, string[] selectedClaims, string[] selectedRoles,
                                         string[] selectedLogins, string[] selectedTokens)
    {
        aspNetUser.UserName = aspNetUser.UserName;
        aspNetUser.NormalizedUserName = aspNetUser.UserName?.ToUpper();

        aspNetUser.Email = aspNetUser.Email;
        aspNetUser.NormalizedEmail = aspNetUser.Email?.ToUpper();
        aspNetUser.EmailConfirmed = aspNetUser.EmailConfirmed;

        // Поля безпечності
        if (!string.IsNullOrWhiteSpace(aspNetUser.PasswordHash))
        {
            aspNetUser.PasswordHash = aspNetUser.PasswordHash;
        }
        ///
        /// Отримати старий пароль користувача.
        /// Захешувати новий пароль через PasswordHasher.
        /// Присвоїти хеш у PasswordHash.
        ///
        //if (!string.IsNullOrWhiteSpace(newPassword))
        //{
        //    var passwordHasher = new PasswordHasher<AspNetUser>();
        //    aspNetUser.PasswordHash = passwordHasher.HashPassword(aspNetUser, newPassword);
        //    aspNetUser.SecurityStamp = Guid.NewGuid().ToString(); // Оновлюємо SecurityStamp
        //}

        // Оновлюємо SecurityStamp, якщо змінюємо критичні дані (пароль, email)
        if (aspNetUser.Email != aspNetUser.Email ||
            aspNetUser.PasswordHash != aspNetUser.PasswordHash)
        {
            aspNetUser.SecurityStamp = Guid.NewGuid().ToString(); // Генеруємо новий SecurityStamp
        }

        // Оновлюємо ConcurrencyStamp для вирішення конфліктів оновлення
        aspNetUser.ConcurrencyStamp = Guid.NewGuid().ToString();

        aspNetUser.PhoneNumber = aspNetUser.PhoneNumber;
        aspNetUser.PhoneNumberConfirmed = aspNetUser.PhoneNumberConfirmed;

        aspNetUser.TwoFactorEnabled = aspNetUser.TwoFactorEnabled;
        aspNetUser.LockoutEnd = aspNetUser.LockoutEnd;
        aspNetUser.LockoutEnabled = aspNetUser.LockoutEnabled;
        aspNetUser.AccessFailedCount = aspNetUser.AccessFailedCount;

        
        foreach (var claim in selectedClaims)
        {
            aspNetUser.UserClaims.Add(new AspNetUserClaim { ClaimType = claim, UserId = aspNetUser.Id });
        }
        foreach (var roleId in selectedRoles)
        {
            aspNetUser.UserRoles.Add(new AspNetUserRole { RoleId = roleId, UserId = aspNetUser.Id });
        }
        foreach (var loginProvider in selectedLogins)
        {
            aspNetUser.UserLogins.Add(new AspNetUserLogin
            {
                LoginProvider = loginProvider,       // LoginProvider + UserId
                ProviderDisplayName = loginProvider, // ProviderDisplayName is just a text field for display
                UserId = aspNetUser.Id
            });
        }
        foreach (var tokenName in selectedTokens)
        {
            aspNetUser.UserTokens.Add(new AspNetUserToken { Name = tokenName, UserId = aspNetUser.Id });
        }

        return aspNetUser;
    }

}


