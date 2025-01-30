using System.Data;
using System.Diagnostics;
using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace Astronomic_Catalogs.Controllers;

public class HomeController : Controller
{
    private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();
    public IConfigurationRoot Configuration { get; set; } = null!;
    public IConfiguration Configuration_2 { get; }
    private string connectionString = null!;

    public HomeController(ApplicationDbContext context, IConfiguration config)
    {

        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        connectionString = Configuration.GetConnectionString("DefaultConnection")
            ?? throw new NullReferenceException("Connecction string is empty!");

        Configuration_2 = config;
        connectionString = Configuration_2.GetConnectionString("DefaultConnection")
            ?? throw new NullReferenceException("Connecction string is empty!"); ;
    }
    public IActionResult Index()
    {
        Logger.Error("Index action invoked");
        return View();
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
}
