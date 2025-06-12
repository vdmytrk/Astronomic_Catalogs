using Astronomic_Catalogs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Astronomic_Catalogs.Controllers;

[Authorize(Policy = "OverAge")]
public class HomeController : Controller
{
    // GET: Home
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

}
