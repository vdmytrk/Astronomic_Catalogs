using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services;
using Astronomic_Catalogs.Services.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
[Authorize(Policy = "AdminPolicy")]
[Authorize(Policy = "UsersAccessClaim")]
[Authorize(Policy = "OverAge")]
public class RolesController : Controller
{
    private readonly RoleControllerService _roleService;
    private readonly RoleManager<AspNetRole> _roleManager;
    private readonly UserManager<AspNetUser> _userManager;
    private readonly SignInManager<AspNetUser> _signInManager;

    public RolesController(
        RoleManager<AspNetRole> roleManager,
        UserManager<AspNetUser> userManager,
        RoleControllerService roleService,
        SignInManager<AspNetUser> signInManager)
    {
        _roleService = roleService;
        _roleManager = roleManager;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET: Admin/Roles
    public async Task<IActionResult> Index()
    {
        var aspNetRole = await _roleManager.Roles
            .Include(r => r.RoleClaims)
            .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
            .ToListAsync();
        return View(aspNetRole.OrderBy(r => r.Name));
    }

    // GET: Admin/Roles/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
            return NotFound();

        var aspNetRole = await _roleManager.Roles
            .Include(r => r.RoleClaims)
            .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (aspNetRole == null)
            return NotFound();

        return View(aspNetRole);
    }

    // GET: Admin/Roles/Create
    public IActionResult Create()
    {
        ViewData["Users"] = new SelectList(_userManager.Users, "Id", "UserName");
        return View();
    }

    // POST: Admin/Roles/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,ConcurrencyStamp")] AspNetRole aspNetRole, string[] selectedUsers)
    {
        if (ModelState.IsValid)
        {
            _roleService.SetData(aspNetRole, selectedUsers);
            var result = await _roleManager.CreateAsync(aspNetRole);
            if (result.Succeeded)
            {
                foreach (var userId in selectedUsers) 
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                        await _signInManager.RefreshSignInAsync(user);
                }
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }

        ViewData["Users"] = new SelectList(_userManager.Users, "Id", "UserName", selectedUsers);
        return View(aspNetRole);
    }

    // GET: Admin/Roles/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
            return NotFound();

        var aspNetRole = await _roleManager.Roles
            .Include(r => r.RoleClaims)
            .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (aspNetRole == null)
            return NotFound();

        ViewData["Users"] = new SelectList(
            _userManager.Users.Select(u => new { u.Id, u.UserName }),
            "Id",
            "UserName",
            aspNetRole.UserRoles?.Select(ur => ur.UserId) ?? new List<string>());
        return View(aspNetRole);
    }

    // POST: Admin/Roles/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("Id,Name,ConcurrencyStamp")] AspNetRole aspNetRole, string[] selectedUsers)
    {
        if (id != aspNetRole.Id)
            return NotFound();

        var existingRole = await _roleManager.Roles
            .Include(r => r.RoleClaims)
            .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (existingRole == null)
            return NotFound();

        var allUsers = await _userManager.Users.ToListAsync();

        if (ModelState.IsValid)
        {
            try
            {
                _roleService.SetData(aspNetRole, selectedUsers, existingRole);
                var result = await _roleManager.UpdateAsync(existingRole);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(existingRole);
                }

                foreach (var user in allUsers)
                {
                    if (selectedUsers.Contains(user.Id))
                    {
                        if (!await _userManager.IsInRoleAsync(user, existingRole.Name!))
                            await _userManager.AddToRoleAsync(user, existingRole.Name!);
                    }
                    else
                    {
                        if (await _userManager.IsInRoleAsync(user, existingRole.Name!))
                            await _userManager.RemoveFromRoleAsync(user, existingRole.Name!);
                    }
                    await _signInManager.RefreshSignInAsync(user);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role is null)
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

        ViewData["Users"] = new SelectList(allUsers, "Id", "UserName", selectedUsers);
        return View(aspNetRole);
    }

    // GET: Admin/Roles/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
            return NotFound();

        var aspNetRole = await _roleManager.Roles
            .Include(r => r.RoleClaims)
            .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (aspNetRole == null)
            return NotFound();

        return View(aspNetRole);
    }

    // POST: Admin/Roles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var aspNetRole = await _roleManager.FindByIdAsync(id);

        if (aspNetRole != null)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(aspNetRole.Name!); 

            var result = await _roleManager.DeleteAsync(aspNetRole);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(aspNetRole);
            }

            foreach (var user in usersInRole) 
            {
                await _signInManager.RefreshSignInAsync(user);
            }
        }

        return RedirectToAction(nameof(Index));
    }

}
