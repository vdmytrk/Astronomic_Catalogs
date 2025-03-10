using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
[Authorize(Policy = "AdminPolicy")]
[Authorize(Policy = "UsersAccessClaim")]
[Authorize(Policy = "OverAge")]
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

}
