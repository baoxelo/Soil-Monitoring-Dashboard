using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Soil_Monitoring_Web_App.Controllers
{
    public class StatisticController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public override void OnActionExecuting(ActionExecutingContext context) => ViewData["Navbar"] = "Statistic";
        public IActionResult Index()
        {
            return View();
        }
    }
}
