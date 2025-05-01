using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WSRS_SWAFO.ViewModels;

namespace WSRS_SWAFO.Controllers
{
    [Authorize(Roles = "AppRole.Admin, AppRole.Member")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly ApplicationDbContext _context;

        public DashboardController(ILogger<DashboardController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                var accountVM = new AccountViewModel
                {
                    Name = user.FirstName + " " + user.Surname
                };
                return View(accountVM);
            }
            else
            {
                _logger.LogError("Some User Information is Missing!");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
