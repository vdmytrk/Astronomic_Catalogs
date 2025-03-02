using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Infrastructure;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.Data;
using System.Diagnostics;

namespace Astronomic_Catalogs.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "Over18")]
[Authorize(Roles = RoleNames.Admin)]
public class HomeAdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseInitializer> _logger;
    private string connectionString = null!;

    public HomeAdminController(
        ApplicationDbContext context,
        ILogger<DatabaseInitializer> logger,
        ConnectionStringProvider connectionStringProvider)
    {
        _logger = logger;
        _context = context;
        connectionString = connectionStringProvider.ConnectionString
            ?? throw new NullReferenceException("Connecction string is empty!");
    }

    // GET: ActualDates
    public async Task<IActionResult> Index()
    {
        return View(await _context.ActualDates.ToListAsync());
    }

    public IActionResult Privacy()
    {
        _logger.LogInformation("Privacy action invoked");
        return View();
    }

    // GET: Admin/ActualDates/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var actualDate = await _context.ActualDates
            .FirstOrDefaultAsync(m => m.Id == id);
        if (actualDate == null)
        {
            return NotFound();
        }

        return View(actualDate);
    }

    // POST: Admin/ActualDates/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var actualDate = await _context.ActualDates.FindAsync(id);
        if (actualDate != null)
        {
            _context.ActualDates.Remove(actualDate);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<string> GetDateFromProcedureADOAsync()
    {
        await using (SqlConnection conn = new SqlConnection(connectionString))
        {
            await conn.OpenAsync();
            await using (SqlCommand cmd = new SqlCommand("GETACTUALDATE", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                object? result = await cmd.ExecuteScalarAsync();
                if (result is DateTime dateRequest)
                {
                    return dateRequest.ToString();
                }
                return "Data absence.";
            }
        }
    }

    public async Task<string> GetDateFromProcedureEFAsync()
    {
        await foreach (var dateRequest in _context.ActualDates
        .FromSqlRaw("EXEC GETACTUALDATE")
        .AsAsyncEnumerable())
        {
            return dateRequest.ActualDateProperty.ToString();
        }
        return "Data absence.";
    }

    public async Task<IActionResult> CallCreateNewDateProcedureAsync()
    {
        await _context.Database.ExecuteSqlRawAsync("EXEC CREATENEWDATE");
        var updatedData = await _context.ActualDates.ToListAsync();
        return PartialView("_ActualDateTable", updatedData);
    }

}
