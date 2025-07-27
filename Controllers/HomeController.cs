using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Soil_Monitoring_Web_App.Models;
using System.Diagnostics;

namespace Soil_Monitoring_Web_App.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            TempData["Navbar"] = "Home";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
