using Microsoft.AspNetCore.Mvc;
using WSRS_Student.Data;

namespace WSRS_Formators.Controllers
{
    public class ViolationController : Controller
    {
        private readonly AzureDbContext _context;

        public ViolationController(AzureDbContext context)
        {
            _context = context;
        }

        // GET: ViolationController
        public ActionResult Index()
        {
            return View();
        }

    }
}
