using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;



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
    public async Task<IActionResult> Create([Bind("UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AspNetUser aspNetUser,
                                            string[] selectedClaims,
                                            string[] selectedRoles,
                                            string[] selectedLogins,
                                            string[] selectedTokens)
    {
        if (ModelState.IsValid)
        {
            // Add Claims
            foreach (var claim in selectedClaims)
            {
                aspNetUser.UserClaims.Add(new AspNetUserClaim { ClaimType = claim, UserId = aspNetUser.Id });
            }

            // Add Roles
            foreach (var roleId in selectedRoles)
            {
                aspNetUser.UserRoles.Add(new AspNetUserRole { RoleId = roleId, UserId = aspNetUser.Id });
            }

            // Update Logins
            //var existingLogins = _context.UserLogins.Where(l => l.UserId == aspNetUser.Id).ToList();
            //_context.UserLogins.RemoveRange(existingLogins);
            //foreach (var login in userLogins)
            //{
            //    aspNetUser.UserLogins.Add(login);
            //}
            foreach (var loginProvider in selectedLogins)
            {
                aspNetUser.UserLogins.Add(new AspNetUserLogin { LoginProvider = loginProvider, UserId = aspNetUser.Id });
            }

            // Update Tokens
            //var existingTokens = _context.UserTokens.Where(t => t.UserId == aspNetUser.Id).ToList();
            //_context.UserTokens.RemoveRange(existingTokens);
            //foreach (var token in userTokens)
            //{
            //    aspNetUser.UserTokens.Add(token);
            //}
            foreach (var tokenName in selectedTokens)
            {
                aspNetUser.UserTokens.Add(new AspNetUserToken { Name = tokenName, UserId = aspNetUser.Id });
            }

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
    public async Task<IActionResult> Edit(string id, [Bind("UserName,Email,PhoneNumber,NormalizedUserName,NormalizedEmail,EmailConfirmed,PhoneNumberConfirmed,LockoutEnd,LockoutEnabled,AccessFailedCount,TwoFactorEnabled,PasswordHash,SecurityStamp,ConcurrencyStamp")] AspNetUser aspNetUser,
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

            existingUser.UserName = aspNetUser.UserName;
            existingUser.NormalizedUserName = aspNetUser.UserName?.ToUpper(); 

            existingUser.Email = aspNetUser.Email;
            existingUser.NormalizedEmail = aspNetUser.Email?.ToUpper(); 
            existingUser.EmailConfirmed = aspNetUser.EmailConfirmed;

            // Поля безпечності
            if (!string.IsNullOrWhiteSpace(aspNetUser.PasswordHash))
            {
                existingUser.PasswordHash = aspNetUser.PasswordHash; 
            }
            ///
            /// Отримати старий пароль користувача.
            /// Захешувати новий пароль через PasswordHasher.
            /// Присвоїти хеш у PasswordHash.
            ///
            //if (!string.IsNullOrWhiteSpace(newPassword))
            //{
            //    var passwordHasher = new PasswordHasher<AspNetUser>();
            //    existingUser.PasswordHash = passwordHasher.HashPassword(existingUser, newPassword);
            //    existingUser.SecurityStamp = Guid.NewGuid().ToString(); // Оновлюємо SecurityStamp
            //}

            // Оновлюємо SecurityStamp, якщо змінюємо критичні дані (пароль, email)
            if (existingUser.Email != aspNetUser.Email ||
                existingUser.PasswordHash != aspNetUser.PasswordHash)
            {
                existingUser.SecurityStamp = Guid.NewGuid().ToString(); // Генеруємо новий SecurityStamp
            }

            // Оновлюємо ConcurrencyStamp для вирішення конфліктів оновлення
            existingUser.ConcurrencyStamp = Guid.NewGuid().ToString();

            existingUser.PhoneNumber = aspNetUser.PhoneNumber;
            existingUser.PhoneNumberConfirmed = aspNetUser.PhoneNumberConfirmed;

            existingUser.TwoFactorEnabled = aspNetUser.TwoFactorEnabled;
            existingUser.LockoutEnd = aspNetUser.LockoutEnd;
            existingUser.LockoutEnabled = aspNetUser.LockoutEnabled;
            existingUser.AccessFailedCount = aspNetUser.AccessFailedCount;




            existingUser.UserClaims.Clear();
            foreach (var claim in selectedClaims)
            {
                existingUser.UserClaims.Add(new AspNetUserClaim { ClaimType = claim, UserId = aspNetUser.Id });
            }
            existingUser.UserRoles.Clear();
            foreach (var roleId in selectedRoles)
            {
                existingUser.UserRoles.Add(new AspNetUserRole { RoleId = roleId, UserId = aspNetUser.Id });
            }
            existingUser.UserLogins.Clear();
            foreach (var login in selectedLogins)
            {
                existingUser.UserLogins.Add(new AspNetUserLogin { ProviderDisplayName = login, UserId = aspNetUser.Id });
            }
            existingUser.UserTokens.Clear();
            foreach (var token in selectedTokens)
            {
                existingUser.UserTokens.Add(new AspNetUserToken { Name = token, UserId = aspNetUser.Id });
            }

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
}


