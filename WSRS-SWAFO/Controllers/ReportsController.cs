using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using WSRS_SWAFO.Models;

namespace WSRS_SWAFO.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly ApplicationDbContext _context;

        public ReportsController(ILogger<ReportsController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public JsonResult GetCollegeReports()
        {
            try
            {
                // Group by CollegeID and count violations for each college
                var collegeReports = _context.ReportsEncoded
                    .GroupBy(r => r.CollegeID)
                    .Select(g => new
                    {
                        College = g.Key,
                        ViolationCount = g.Count()
                    })
                    .OrderBy(report => report.College) // Optional: Sort alphabetically by CollegeID
                    .ToList();

                // Prepare the labels and violation counts for the response
                var labels = collegeReports.Select(cr => cr.College).ToList();
                var violationNumbers = collegeReports.Select(cr => cr.ViolationCount).ToList();

                return Json(new { labels, violationNumbers });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching college reports");
                return Json(new { error = "Failed to fetch data. Please try again later." });
            }
        }
    }
}
