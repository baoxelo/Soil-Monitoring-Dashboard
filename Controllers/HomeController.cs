using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Soil_Monitoring_Web_App.ExtensionServices;
using Soil_Monitoring_Web_App.IExtensionServices;
using Soil_Monitoring_Web_App.Models;
using System.Diagnostics;

namespace Soil_Monitoring_Web_App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICsvToSqlService _csvToSqlService;
        public override void OnActionExecuting(ActionExecutingContext context) => ViewData["Navbar"] = "Home";

        public HomeController(ILogger<HomeController> logger, ICsvToSqlService csvToSqlService)
        {
            _logger = logger;
            _csvToSqlService = csvToSqlService;
        }


        public async Task<IActionResult> IndexAsync()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RefreshDataAsync()
        {

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
