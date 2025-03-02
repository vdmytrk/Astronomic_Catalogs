using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Infrastructure;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Astronomic_Catalogs.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseInitializer> _logger;
    private string connectionString = null!;

    public HomeController(
        ApplicationDbContext context, 
        ILogger<DatabaseInitializer> logger, 
        ConnectionStringProvider connectionStringProvider,
        UserManager<AspNetUser> userManager)
    {
        _logger = logger;
        _context = context;
        connectionString = connectionStringProvider.ConnectionString
            ?? throw new NullReferenceException("Connecction string is empty!");

    }

    // GET: Home
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        bool isNullOrEmpty = string.IsNullOrEmpty(userId);
        List<AspNetUserClaim> claims = new List<AspNetUserClaim>();
        if (!isNullOrEmpty)
        {
            claims = await _context.UserClaims.Where(c => c.UserId == userId).ToListAsync();
        }
        ViewBag.Claims = claims;

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
