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
public class RoleClaimsController : Controller
{
    private readonly ApplicationDbContext _context;

    public RoleClaimsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin/RoleClaims
    public async Task<IActionResult> Index()
    {
        var aspNetRoleClaim = _context.RoleClaims.Include(a => a.Role).OrderBy(rc => rc.Id);
        return View(await aspNetRoleClaim.ToListAsync());
    }

    // GET: Admin/RoleClaims/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var aspNetRoleClaim = await _context.RoleClaims
            .Include(a => a.Role)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (aspNetRoleClaim == null)
        {
            return NotFound();
        }

        return View(aspNetRoleClaim);
    }    
}
