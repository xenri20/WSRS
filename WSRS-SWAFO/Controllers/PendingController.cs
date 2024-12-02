using Microsoft.AspNetCore.Mvc;

namespace WSRS_SWAFO.Controllers
{
    public class PendingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
