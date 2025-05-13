using Microsoft.AspNetCore.Mvc;

namespace Astronomic_Catalogs.Areas.Admin.TelescopeTools.Controllers
{
    [Area("TelescopeTools")]
    public class TelescopeViewCalculatorController : Controller
    {
        // GET: TelescopeTools/TelescopeViewCalculator
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
