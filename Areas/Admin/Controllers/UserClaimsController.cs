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
public class UserClaimsController : Controller
{
    private readonly ApplicationDbContext _context;

    public UserClaimsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/UserClaims
    public async Task<IActionResult> Index()
    {
        var aspNetUserClaim = _context.UserClaims.Include(a => a.User).OrderBy(uc => uc.Id);
        return View(await aspNetUserClaim.ToListAsync());
    }

    // GET: Admin/UserClaims/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetUserClaim = await _context.UserClaims
            .FirstOrDefaultAsync(m => m.Id == id);
        if (aspNetUserClaim == null)
        {
            return NotFound();
        }

        return View(aspNetUserClaim);
    }

}
