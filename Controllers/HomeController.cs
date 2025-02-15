using System.Data;
using System.Diagnostics;
using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Infrastructure;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Astronomic_Catalogs.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();
    public ConnectionStringProvider Configuration { get; set; } = null!;
    private string connectionString = null!;

    public HomeController(ApplicationDbContext context, ConnectionStringProvider connectionStringProvider)
    {
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
        Logger.Info("Privacy action invoked");
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    public string GetDateFromProcedureADO()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("GETACTUALDATE", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DateTime dateRequest = (DateTime)cmd.ExecuteScalar();

            string date = dateRequest.ToString() ?? "Data absence.";
            return date;
        }
    }

    // Довго спілкувався з ChatGPT, пробував різні коди, дійшли висновуку що за допомогою Entity Framewok тут 
    //   нічого не зробиш! :(
    // The database connection has been established.
    public string GetDateFromProcedureEF()
    {
        using (_context)
        {
            var dateRequest = _context.ActualDates
                .FromSqlRaw("EXEC GETACTUALDATE")
                .AsEnumerable()  // Переводимо в пам'ять
                .FirstOrDefault();

            return dateRequest?.ActualDateProperty.ToString() ?? "Data absence.";
        }
    }
}
