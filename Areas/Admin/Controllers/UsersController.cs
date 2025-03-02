using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;

namespace Astronomic_Catalogs.Areas.Admin.Controllers;

[Area("Admin")]
public class UsersController : Controller
{
    private readonly UserControllerService _userService;
    private readonly RoleManager<AspNetRole> _roleManager;
    private readonly UserManager<AspNetUser> _userManager;

    public UsersController(RoleManager<AspNetRole> roleManager, UserManager<AspNetUser> userManager, UserControllerService userService)
    {
        _userService = userService;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    // GET: Admin/Users
    public async Task<IActionResult> Index()
    {
        var aspNetUser = await _userManager.Users
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

        var aspNetUser = await _userManager.Users
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
        ViewData["Years"] = Enumerable.Range(DateTime.Now.Year - 120, 121).Reverse().ToList();        
        ViewData["Roles"] = new SelectList(_roleManager.Roles.Select(r => new { r.Id, r.Name }), "Id", "Name");
        ViewData["AccessFailedCount"] = 0;

        return View();
    }

    // POST: Admin/Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("UserName,Email,EmailConfirmed,PhoneNumber,PhoneNumberConfirmed,YearOfBirth," +
                                              "TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AspNetUser aspNetUser,
                                            string password,
                                            string[] selectedRoles)
    {
        if (selectedRoles == null || selectedRoles.Length == 0)
        {
            ModelState.AddModelError("selectedRoles", "The user must have at least one role.");
        }
        if (ModelState.IsValid)
        {
            _userService.SetData(aspNetUser, selectedRoles!);

            var result = await _userManager.CreateAsync(aspNetUser, password);
            if (result.Succeeded)
                return RedirectToAction(nameof(Index));
            
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }

        ViewData["Years"] = Enumerable.Range(DateTime.Now.Year - 120, 121).Reverse().ToList();
        ViewData["Roles"] = new SelectList(_roleManager.Roles.Select(r => new { r.Id, r.Name }), "Id", "Name");
        return View(aspNetUser);
    }

    // GET: Admin/Users/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetUser = await _userManager.Users
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

        ViewData["Years"] = Enumerable.Range(DateTime.Now.Year - 120, 121).Reverse().ToList();
        ViewData["Roles"] = new SelectList(
            _roleManager.Roles.Select(r => new { r.Id, r.Name }), 
            "Id", 
            "Name",
            aspNetUser.UserRoles?.Select(ur => ur.RoleId) ?? new List<string>());

        return View(aspNetUser);
    }

    // POST: Admin/Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id,
                                          [Bind("Id,UserName,Email,EmailConfirmed,PhoneNumber,PhoneNumberConfirmed,YearOfBirth," +
                                            "TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AspNetUser aspNetUser,
                                          string[] selectedRoles)
    {
        if (id != aspNetUser.Id)
            return NotFound();

        var existingUser = await _userManager.Users
            .Include(u => u.UserClaims)
            .Include(u => u.UserRoles)
                .ThenInclude(r => r.Role)
            .Include(u => u.UserLogins)
            .Include(u => u.UserTokens)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (existingUser == null)
            return NotFound();

        var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync(); 
        var userRoles = await _userManager.GetRolesAsync(existingUser);
        var roleDictionary = await _roleManager.Roles.ToDictionaryAsync(r => r.Id, r => r.Name);


        if (selectedRoles == null || selectedRoles.Length == 0)
        {
            ModelState.AddModelError("selectedRoles", "The user must have at least one role.");
        }
        if (ModelState.IsValid)
        {
            try
            {
                _userService.SetData(aspNetUser, selectedRoles!, existingUser);
                var result = await _userManager.UpdateAsync(existingUser);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(existingUser);
                }

                foreach (var roleId in selectedRoles!)
                {
                    if (roleDictionary.TryGetValue(roleId, out var roleName))
                    {
                        if (!await _userManager.IsInRoleAsync(existingUser, roleName!))
                            await _userManager.AddToRoleAsync(existingUser, roleName!);
                    }
                }

                foreach (var roleName in userRoles)
                {
                    if (!selectedRoles.Contains(roleDictionary.FirstOrDefault(r => r.Value == roleName).Key))
                    {
                        await _userManager.RemoveFromRoleAsync(existingUser, roleName);
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user is null)
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

        ViewData["Years"] = Enumerable.Range(DateTime.Now.Year - 120, 121).Reverse().ToList();
        ViewData["Roles"] = new SelectList(_roleManager.Roles.Select(r => new { r.Id, r.Name }), "Id", "Name", selectedRoles ?? Array.Empty<string>());

        return View(existingUser);
    }

    // GET: Admin/Users/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
            return NotFound();

        var aspNetUser = await _userManager.Users
            .Include(u => u.UserClaims)
            .Include(u => u.UserRoles)
                .ThenInclude(r => r.Role)
            .Include(u => u.UserLogins)
            .Include(u => u.UserTokens)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (aspNetUser == null)
            return NotFound();

        return View(aspNetUser);
    }

    // POST: Admin/Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var aspNetUser = await _userManager.FindByIdAsync(id);

        if (aspNetUser != null)
        {
            var result = await _userManager.DeleteAsync(aspNetUser);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(aspNetUser);
            }
        }

        return RedirectToAction(nameof(Index));
    }

}


