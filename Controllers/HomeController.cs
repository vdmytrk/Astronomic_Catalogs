using System.Diagnostics;
using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace Astronomic_Catalogs.Controllers
{
    public class HomeController : Controller
    {
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();

        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        public HomeController()
        {
            //_logger = logger;
        }

        public IActionResult Index()
        {
            Logger.Info("Index action invoked");
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
    }
}
