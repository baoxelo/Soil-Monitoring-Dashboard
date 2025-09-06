using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Soil_Monitoring_Web_App.ExtensionServices;
using Soil_Monitoring_Web_App.IExtensionServices;
using Soil_Monitoring_Web_App.Models;
using System.Diagnostics;

namespace Soil_Monitoring_Web_App.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseContext _context;
        private readonly ICsvToSqlService _csvToSqlService;

        public HomeController(ILogger<HomeController> logger, ICsvToSqlService csvToSqlService, DatabaseContext context)
        {
            _logger = logger;
            _context = context;
            _csvToSqlService = csvToSqlService;
        }


        public async Task<IActionResult> IndexAsync()
        {
            var data = await _context.SoilDatas
                .Include(q => q.Sensor)
                .OrderByDescending( q => q.Date)
                .OrderByDescending( q => q.Time)
                .FirstOrDefaultAsync();


            var humiditties = await _context.SoilDatas
                .Select(s => s.Humidity)
                .ToListAsync();

            TempData["Humiditties"] = JsonConvert.SerializeObject(humiditties);
            ViewData["Navbar"] = "Home";

            return View(data);
        }
        [HttpPost]
        public async Task<IActionResult> RefreshDataAsync()
        {
            await _csvToSqlService.ImportCsvToDatabase();

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            ViewData["Navbar"] = "Privacy";
            return View();
        }

    }
}
