using Microsoft.AspNetCore.Mvc;

namespace WSRS_SWAFO.Controllers
{
    public class TrafficViolationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
