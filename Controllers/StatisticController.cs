using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Soil_Monitoring_Web_App.Models;

namespace Soil_Monitoring_Web_App.Controllers
{
    [Authorize]

    public class StatisticController : Controller
    {
        private readonly ILogger<StatisticController> _logger;
        private readonly DatabaseContext _context;
        public override void OnActionExecuting(ActionExecutingContext context) => ViewData["Navbar"] = "Statistic";
        public StatisticController (ILogger<StatisticController> logger, DatabaseContext context )
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var statistic = new StatisticPageModel()
            {
                N = new List<StatisticData>(),
                P = new List<StatisticData>(),
                K = new List<StatisticData>(),
                Temp = new List<StatisticData>(),
                Humiditty = new List<StatisticData>(),
                pH = new List<StatisticData>(),
                EC = new List<StatisticData>()
            };

            var soilDatas = await _context.SoilDatas.ToListAsync();

            foreach (var data in soilDatas)
            {
                var fulldatetime = data.Date.Date.Add(data.Time).ToString("yyyy-MM-ddTHH:mm:ss+00:00");

                statistic.N.Add(new StatisticData { Data = data.N, Date = fulldatetime });
                statistic.P.Add(new StatisticData { Data = data.P, Date = fulldatetime });
                statistic.K.Add(new StatisticData { Data = data.K, Date = fulldatetime });
                statistic.Temp.Add(new StatisticData { Data = data.Temp, Date = fulldatetime });
                statistic.Humiditty.Add(new StatisticData { Data = data.Humidity, Date = fulldatetime });
                statistic.pH.Add(new StatisticData { Data = data.PH, Date = fulldatetime });
                statistic.EC.Add(new StatisticData { Data = data.EC, Date = fulldatetime });
            }
            var sensor = _context.Sensors
                     .OrderByDescending(q => q.Id)
                     .FirstOrDefault();

            if (sensor != null)
            {
                ViewData["Location"] = new
                {
                    Latitude = sensor.Latitude,
                    Longitude = sensor.Longitude
                };
            }
            return View(statistic);
        }
    }
}
