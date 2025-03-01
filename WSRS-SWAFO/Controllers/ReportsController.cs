using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using WSRS_SWAFO.Models;
using System.Linq;
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
                List<int> violationClassificationIds = new List<int>();
                bool isTrafficViolation = false;

                switch (request.ViolationType)
                {
                    case "MajorViolations":
                        violationClassificationIds = new List<int> { 9, 10, 11 };
                        break;
                    case "MinorViolations":
                        violationClassificationIds = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
                        break;
                    case "MinorTrafficViolations":
                        violationClassificationIds = new List<int> { 12, 13, 14, 15, 16, 17, 18 };
                        isTrafficViolation = true;
                        break;
                    case "MajorTrafficViolations":
                        violationClassificationIds = new List<int> { 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 };
                        isTrafficViolation = true;
                        break;
                    default:
                        throw new Exception("Invalid violation type.");
                }

                DateOnly? start = request.StartDate.HasValue ? DateOnly.FromDateTime(request.StartDate.Value) : null;
                DateOnly? end = request.EndDate.HasValue ? DateOnly.FromDateTime(request.EndDate.Value) : null;

                var reports = isTrafficViolation
                    ? _context.TrafficReportsEncoded
                        .Where(r => violationClassificationIds.Contains(r.OffenseId) &&
                                    (!request.StartDate.HasValue || r.CommissionDatetime >= request.StartDate.Value) &&
                                    (!request.EndDate.HasValue || r.CommissionDatetime <= request.EndDate.Value))
                        .GroupBy(r => r.CollegeID)
                        .Select(g => new { College = g.Key, ViolationCount = g.Count() })
                        .ToList()
                    : _context.ReportsEncoded
                        .Where(r => violationClassificationIds.Contains(r.OffenseId) &&
                                    (!start.HasValue || r.CommissionDate >= start) &&
                                    (!end.HasValue || r.CommissionDate <= end))
                        .GroupBy(r => r.CollegeID)
                        .Select(g => new { College = g.Key, ViolationCount = g.Count() })
                        .ToList();

                _logger.LogInformation("Fetched {count} reports", reports.Count); // Debugging

                if (!reports.Any())
                {
                    _logger.LogWarning("No reports found for given filters!");
                }

                return Json(new
                {
                    labels = reports.Select(r => r.College).ToList(),
                    violationNumbers = reports.Select(r => r.ViolationCount).ToList(),
                    totalViolations = reports.Sum(r => r.ViolationCount)
                });
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
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

