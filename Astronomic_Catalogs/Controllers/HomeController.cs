using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Authorization;
using Astronomic_Catalogs.Data;
using Microsoft.EntityFrameworkCore;

namespace Astronomic_Catalogs.Controllers;

[Authorize(Policy = "OverAge")]
public class HomeController(
    ApplicationDbContext context,
    ILogger<HomeController> logger
    ) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<HomeController> _logger = logger;

    // GET: Home
    public async Task<IActionResult> Index()
    {
        return View(await _context.ActualDates.ToListAsync());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
