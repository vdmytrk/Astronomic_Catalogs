using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        var aspNetUser = await _context.Users
            .Include(u => u.UserClaims)
            .Include(u => u.UserRoles)
                .ThenInclude(r => r.Role)
            .Include(u => u.UserLogins)
            .Include(u => u.UserTokens)
            .ToListAsync();
        return View(aspNetUser.OrderBy(u => u.RegistrationDate).OrderBy(u => u.UserName));
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
        // ОСКІЛЬКИ ЙДЕ СТВОРЕННЯ ОБ'ЄКТУ, ТОМУ ОБ'ЄКТИ ЯКІ   Б У Д У Т Ь   ЗВ'ЯЗАНІ З User ЗВ'ЯЗКОМ О-д-Б НЕ ПОТРІБНО СТВОРЮВАТИ
        // ПРИ СТВОРЕННІ User-А!!! АДЖЕ, НАПРИКЛАД, ОДИН Claims НЕ МОЖЕ БУТИ В ДЕКІЛЬКОХ User-ІВ, ТО І ВИБИРАТИ НІЧОГО!!!
        // А ОТ Role ОДИН User МОЖЕ МАТИ БАГАТО!!!
        ViewData["Roles"] = new SelectList(_context.Roles.Select(r => new { r.Id, r.Name }), "Id", "Name");
        //ViewData["Claims"] = new SelectList(_context.UserClaims.Select(c => new { c.Id, c.ClaimType }), "Id", "ClaimType");
        //ViewData["Logins"] = new SelectList(
        //    _context.UserLogins.Select(l => new { Id = l.LoginProvider + "|" + l.ProviderKey, Name = l.ProviderDisplayName }), "Id", "Name"
        //);
        //ViewData["Tokens"] = new SelectList(
        //    _context.UserTokens.Select(t => new { Id = t.UserId + "|" + t.LoginProvider + "|" + t.Name, t.Name }), "Id", "Name"
        //);

        return View();
    }

    // POST: Admin/Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("UserName,Email,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AspNetUser aspNetUser,
                                            string[] selectedRoles)
    {
        SetData(aspNetUser, selectedRoles);

        if (ModelState.IsValid)
        {

            _context.Add(aspNetUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["Roles"] = new SelectList(_context.Roles.Select(r => new { r.Id, r.Name }), "Id", "Name"); 
        //ViewData["Claims"] = new SelectList(_context.UserClaims.Select(c => new { c.Id, c.ClaimType }), "Id", "ClaimType");
        //ViewData["Logins"] = new SelectList(
        //    _context.UserLogins.Select(l => new { Id = l.LoginProvider + "|" + l.ProviderKey, Name = l.ProviderDisplayName }), "Id", "Name"
        //);
        //ViewData["Tokens"] = new SelectList(
        //    _context.UserTokens.Select(t => new { Id = t.UserId + "|" + t.LoginProvider + "|" + t.Name, t.Name }), "Id", "Name"
        //);

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
                                          string[] selectedRoles
                                          //string[] selectedClaims,
                                          //string[] selectedLogins,
                                          //string[] selectedTokens
        )
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

        SetData(aspNetUser, selectedRoles, existingUser);


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

        //ViewData["Claims"] = new SelectList(_context.UserClaims.Select(c => c.ClaimType).Distinct(), selectedClaims);
        ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name", selectedRoles);
        //ViewData["Logins"] = new SelectList(_context.UserLogins.Select(l => l.ProviderDisplayName).Distinct(), selectedLogins);
        //ViewData["Tokens"] = new SelectList(_context.UserTokens.Select(t => t.Name).Distinct(), selectedTokens);

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


    /// <summary>
    /// TODO: Check input values.
    /// </summary>
    private void SetData(AspNetUser inputUser, string[] selectedRoles, AspNetUser? existingUser = null
        //string[]? selectedClaims = null, string[]? selectedLogins = null, string[]? selectedTokens = null, 
        )
    {
        var targetUser = existingUser ?? inputUser; // Використовуємо existingUser, якщо він переданий, інакше inputUser

        targetUser.UserName = inputUser.UserName;
        targetUser.NormalizedUserName = inputUser.UserName?.ToUpper();
        targetUser.Email = inputUser.Email;
        targetUser.NormalizedEmail = inputUser.Email?.ToUpper();
        targetUser.EmailConfirmed = inputUser.EmailConfirmed;

        // Поля безпечності
        if (!string.IsNullOrWhiteSpace(inputUser.PasswordHash))
        {
            targetUser.PasswordHash = inputUser.PasswordHash;
        }
        ///
        /// Отримати старий пароль користувача.
        /// Захешувати новий пароль через PasswordHasher.
        /// Присвоїти хеш у PasswordHash.
        ///
        ///if (!string.IsNullOrWhiteSpace(newPassword))
        ///{
        ///    var passwordHasher = new PasswordHasher<AspNetUser>();
        ///    aspNetUser.PasswordHash = passwordHasher.HashPassword(aspNetUser, newPassword);
        ///    aspNetUser.SecurityStamp = Guid.NewGuid().ToString(); // Оновлюємо SecurityStamp
        ///}

        // Оновлюємо SecurityStamp, якщо змінюємо критичні дані (пароль, email)
        if (targetUser.Email != inputUser.Email || targetUser.PasswordHash != inputUser.PasswordHash)
        {
            targetUser.SecurityStamp = Guid.NewGuid().ToString();
        }

        // Оновлюємо ConcurrencyStamp для вирішення конфліктів оновлення
        targetUser.ConcurrencyStamp = Guid.NewGuid().ToString();

        targetUser.PhoneNumber = inputUser.PhoneNumber;
        targetUser.PhoneNumberConfirmed = inputUser.PhoneNumberConfirmed;

        targetUser.TwoFactorEnabled = inputUser.TwoFactorEnabled;
        targetUser.LockoutEnd = inputUser.LockoutEnd;
        targetUser.LockoutEnabled = inputUser.LockoutEnabled;
        targetUser.AccessFailedCount = inputUser.AccessFailedCount;

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // === Обробка Roles ===
        //aspNetUser.UserRoles.Clear();
        // Оновлення ролей. Видаляємо ролі, яких більше немає у selectedRoles
        var currentRoleIds = targetUser.UserRoles.Select(ur => ur.RoleId).ToList();
        var rolesToRemove = targetUser.UserRoles.Where(ur => !selectedRoles.Contains(ur.RoleId)).ToList();
        foreach (var role in rolesToRemove)
        {
            targetUser.UserRoles.Remove(role);
        }
        foreach (var roleId in selectedRoles)
        {
            if (!currentRoleIds.Contains(roleId))
            {
                targetUser.UserRoles.Add(new AspNetUserRole { RoleId = roleId, UserId = inputUser.Id });
            }
        }
        #region Not usable
        //selectedClaims ??= Array.Empty<string>();
        //selectedLogins ??= Array.Empty<string>();
        //selectedTokens ??= Array.Empty<string>();
        ////---------------------------------------------------------------------------------------------------------------
        //// === Обробка Claims ===
        ////aspNetUser.UserClaims.Clear();
        //var currentClaims = targetUser.UserClaims.Select(uc => uc.ClaimType).ToList();
        //var claimsToRemove = targetUser.UserClaims.Where(uc => !selectedClaims.Contains(uc.ClaimType)).ToList();
        //foreach (var claim in claimsToRemove)
        //{
        //    targetUser.UserClaims.Remove(claim);
        //}
        //foreach (var claim in selectedClaims)
        //{
        //    if (!currentClaims.Contains(claim))
        //    {
        //        targetUser.UserClaims.Add(new AspNetUserClaim { ClaimType = claim, UserId = inputUser.Id });
        //    }
        //}
        ////---------------------------------------------------------------------------------------------------------------
        //// === Обробка Logins ===
        ////aspNetUser.UserLogins.Clear();
        //var currentLogins = targetUser.UserLogins.Select(ul => ul.LoginProvider + "|" + ul.ProviderKey).ToList();
        //var loginsToRemove = targetUser.UserLogins.Where(ul => !selectedLogins.Contains(ul.LoginProvider + "|" + ul.ProviderKey)).ToList();
        //foreach (var login in loginsToRemove)
        //{
        //    targetUser.UserLogins.Remove(login);
        //}
        //foreach (var loginKey in selectedLogins)
        //{
        //    var loginParts = loginKey.Split('|');
        //    if (loginParts.Length == 2)
        //    {
        //        var loginProvider = loginParts[0];
        //        var providerKey = loginParts[1];

        //        if (!currentLogins.Contains(loginKey))
        //        {
        //            targetUser.UserLogins.Add(new AspNetUserLogin
        //            {
        //                LoginProvider = loginProvider,
        //                ProviderKey = providerKey,
        //                UserId = inputUser.Id // Foreign key
        //            });
        //        }
        //    }
        //}
        ////---------------------------------------------------------------------------------------------------------------
        //// === Обробка Tokens ===
        ////aspNetUser.UserTokens.Clear();
        //var currentTokens = targetUser.UserTokens.Select(ut => ut.UserId + "|" + ut.LoginProvider + "|" + ut.Name).ToList();
        //var tokensToRemove = targetUser.UserTokens.Where(ut => !selectedTokens.Contains(ut.UserId + "|" + ut.LoginProvider + "|" + ut.Name)).ToList();
        //foreach (var token in tokensToRemove)
        //{
        //    targetUser.UserTokens.Remove(token);
        //}
        //foreach (var tokenKey in selectedTokens)
        //{
        //    var tokenParts = tokenKey.Split('|');
        //    if (tokenParts.Length == 3)
        //    {
        //        var loginProvider = tokenParts[1];
        //        var name = tokenParts[2];

        //        if (!currentTokens.Contains(tokenKey))
        //        {
        //            targetUser.UserTokens.Add(new AspNetUserToken
        //            {
        //                UserId = inputUser.Id,
        //                LoginProvider = loginProvider,
        //                Name = name 
        //            });
        //        }
        //    }
        //}
        #endregion
    }

}


