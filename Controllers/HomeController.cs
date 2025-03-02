using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Astronomic_Catalogs.Controllers;

public class HomeController(
    ApplicationDbContext context,
    ILogger<DatabaseInitializer> logger
    ) : Controller
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<DatabaseInitializer> _logger = logger;

    // GET: Home
    public async Task<IActionResult> Index()
    {
        return View(await _context.ActualDates.ToListAsync());
    }

    public IActionResult Privacy()
    {
        _logger.LogInformation("Privacy action invoked");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
