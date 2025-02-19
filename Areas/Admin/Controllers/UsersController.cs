using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol.Plugins;
using System;

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
        return View(aspNetUser.OrderBy(u => u.Id));
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
        SetData(aspNetUser, aspNetUser, selectedClaims, selectedRoles, selectedLogins, selectedTokens);

        if (ModelState.IsValid)
        {
            
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
    public async Task<IActionResult> Edit(string id, 
                                          [Bind("Id,UserName,Email,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AspNetUser aspNetUser,
                                          string[] selectedClaims,
                                          string[] selectedRoles,
                                          string[] selectedLogins,
                                          string[] selectedTokens)
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

        SetData(existingUser, aspNetUser, selectedClaims, selectedRoles, selectedLogins, selectedTokens);

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

        ViewData["Claims"] = new SelectList(_context.UserClaims.Select(c => c.ClaimType).Distinct(), selectedClaims);
        ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name", selectedRoles);
        ViewData["Logins"] = new SelectList(_context.UserLogins.Select(l => l.ProviderDisplayName).Distinct(), selectedLogins);
        ViewData["Tokens"] = new SelectList(_context.UserTokens.Select(t => t.Name).Distinct(), selectedTokens);

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


    /// <summary>
    /// TODO: Check input values.
    /// </summary>
    private void SetData(AspNetUser existingUser, AspNetUser inputUser, string[] selectedClaims, string[] selectedRoles,
                                         string[] selectedLogins, string[] selectedTokens)
    {
        existingUser.UserName = inputUser.UserName;
        existingUser.NormalizedUserName = inputUser.UserName?.ToUpper();
        existingUser.Email = inputUser.Email;
        existingUser.NormalizedEmail = inputUser.Email?.ToUpper();
        existingUser.EmailConfirmed = inputUser.EmailConfirmed;

        // Поля безпечності
        if (!string.IsNullOrWhiteSpace(inputUser.PasswordHash))
        {
            existingUser.PasswordHash = inputUser.PasswordHash;
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
        if (existingUser.Email != inputUser.Email || existingUser.PasswordHash != inputUser.PasswordHash)
        {
            existingUser.SecurityStamp = Guid.NewGuid().ToString(); // Генеруємо новий SecurityStamp
        }

        // Оновлюємо ConcurrencyStamp для вирішення конфліктів оновлення
        existingUser.ConcurrencyStamp = Guid.NewGuid().ToString();

        existingUser.PhoneNumber = inputUser.PhoneNumber;
        existingUser.PhoneNumberConfirmed = inputUser.PhoneNumberConfirmed;

        existingUser.TwoFactorEnabled = inputUser.TwoFactorEnabled;
        existingUser.LockoutEnd = inputUser.LockoutEnd;
        existingUser.LockoutEnabled = inputUser.LockoutEnabled;
        existingUser.AccessFailedCount = inputUser.AccessFailedCount;

        //aspNetUser.UserClaims.Clear();
        var currentClaims = existingUser.UserClaims.Select(uc => uc.ClaimType).ToList();
        var claimsToRemove = existingUser.UserClaims.Where(uc => !selectedClaims.Contains(uc.ClaimType)).ToList();
        foreach (var claim in claimsToRemove)
        {
            existingUser.UserClaims.Remove(claim);
        }
        foreach (var claim in selectedClaims)
        {
            if (!currentClaims.Contains(claim))
            {
                existingUser.UserClaims.Add(new AspNetUserClaim { ClaimType = claim, UserId = inputUser.Id });
            }
        }

        //aspNetUser.UserRoles.Clear();
        // Оновлення ролей. Видаляємо ролі, яких більше немає у selectedRoles
        var currentRoleIds = existingUser.UserRoles.Select(ur => ur.RoleId).ToList();
        var rolesToRemove = existingUser.UserRoles.Where(ur => !selectedRoles.Contains(ur.RoleId)).ToList();
        foreach (var role in rolesToRemove)
        {
            existingUser.UserRoles.Remove(role);
        }
        foreach (var roleId in selectedRoles)
        {
            if (!currentRoleIds.Contains(roleId))
            {
                existingUser.UserRoles.Add(new AspNetUserRole { RoleId = roleId, UserId = inputUser.Id });
            }
        }

        //aspNetUser.UserLogins.Clear();
        var currentLogins = existingUser.UserLogins.Select(ul => ul.LoginProvider).ToList();
        var loginsToRemove = existingUser.UserLogins.Where(ul => !selectedLogins.Contains(ul.LoginProvider)).ToList();
        foreach (var login in loginsToRemove)
        {
            existingUser.UserLogins.Remove(login);
        }
        foreach (var loginProvider in selectedLogins)
        {
            if (!currentLogins.Contains(loginProvider))
            {
                existingUser.UserLogins.Add(new AspNetUserLogin
                {
                    LoginProvider = loginProvider,       // LoginProvider + UserId
                    ProviderDisplayName = loginProvider, // ProviderDisplayName is just a text field for display
                    UserId = inputUser.Id
                });
            }
        }

        //aspNetUser.UserTokens.Clear();
        var currentTokens = existingUser.UserTokens.Select(ut => ut.Name).ToList();
        var tokensToRemove = existingUser.UserTokens.Where(ut => !selectedTokens.Contains(ut.Name)).ToList();
        foreach (var token in tokensToRemove)
        {
            existingUser.UserTokens.Remove(token);
        }
        foreach (var tokenName in selectedTokens)
        {
            if (!currentTokens.Contains(tokenName))
            {
                existingUser.UserTokens.Add(new AspNetUserToken { Name = tokenName, UserId = inputUser.Id });
            }
        }
    }

}


