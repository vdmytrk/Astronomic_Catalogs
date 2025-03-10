using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Astronomic_Catalogs.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
[Authorize(Policy = "AdminPolicy")]
[Authorize(Policy = "UsersAccessClaim")]
[Authorize(Policy = "OverAge")]
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

}
