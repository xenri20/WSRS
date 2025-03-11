using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.Data;

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
        public JsonResult GetCollegeReports([FromBody] ViolationRequest request)
        {
            try
            {
                // Define the violation Nature IDs based on the selected violation type
                List<int> violationClassificationIds = new List<int>();
                bool isTrafficViolation = false; // Flag to check if we should use TrafficReportsEncoded

                switch (request.ViolationType)
                {
                    case "MajorViolations":
                        violationClassificationIds = new List<int> { 9, 10, 11 }; // Major Violations
                        break;
                    case "MinorViolations":
                        violationClassificationIds = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 }; // Minor Violations
                        break;
                    case "MinorTrafficViolations":
                        violationClassificationIds = new List<int> { 12, 13, 14, 15, 16, 17, 18 }; // Minor Traffic Violations
                        isTrafficViolation = true;
                        break;
                    case "MajorTrafficViolations":
                        violationClassificationIds = new List<int> { 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 }; // Major Traffic Violations
                        isTrafficViolation = true;
                        break;
                    default:
                        throw new Exception("Invalid violation type.");
                }

                // Fetch data from the correct table
                var collegeReports = isTrafficViolation
                    ? _context.TrafficReportsEncoded
                        .Where(r => violationClassificationIds.Contains(r.OffenseId)) // Fetch from TrafficReportsEncoded
                        .GroupBy(r => r.CollegeID)
                        .Select(g => new
                        {
                            College = g.Key,
                            ViolationCount = g.Count()
                        })
                        .OrderBy(report => report.College)
                        .ToList()
                    : _context.ReportsEncoded
                        .Where(r => violationClassificationIds.Contains(r.OffenseId)) // Fetch from ReportsEncoded
                        .GroupBy(r => r.CollegeID)
                        .Select(g => new
                        {
                            College = g.Key,
                            ViolationCount = g.Count()
                        })
                        .OrderBy(report => report.College)
                        .ToList();

                // Calculate the total violations
                var totalViolations = collegeReports.Sum(cr => cr.ViolationCount);

                // Prepare the labels and violation counts for the response
                var labels = collegeReports.Select(cr => cr.College).ToList();
                var violationNumbers = collegeReports.Select(cr => cr.ViolationCount).ToList();

                return Json(new { labels, violationNumbers, totalViolations });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching college reports");
                return Json(new { error = "Failed to fetch data. Please try again later." });
            }
        }
    }
}
    

    // Model for the request to include violation type
    public class ViolationRequest
    {
        public string ViolationType { get; set; }
    }

