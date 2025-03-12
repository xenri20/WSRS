using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.Data;
using ClosedXML.Excel;
using WSRS_SWAFO.Data.Enum;

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
                if (request == null)
                {
                    _logger.LogError("ViolationRequest is null.");
                    return Json(new { error = "Invalid request data." });
                }

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

                if (start.HasValue && end.HasValue && start > end)
                {
                    return Json(new { error = "Start date cannot be after end date." });
                }

                var reports = isTrafficViolation
            ? _context.TrafficReportsEncoded
                .Where(r => violationClassificationIds.Contains(r.OffenseId) &&
                            (!start.HasValue || r.CommissionDate >= start) &&
                            (!end.HasValue || r.CommissionDate <= end))
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

        [HttpGet]
        public IActionResult ExportToExcel(string fileName, DateTime? startDate, DateTime? endDate)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Stats by College");
                worksheet.Cell(1, 1).Value = "College";
                worksheet.Cell(1, 2).Value = "Violation Type";
                worksheet.Cell(1, 3).Value = "Total Violations";

                // Adding months as headers
                for (int i = 0; i < 12; i++)
                {
                    worksheet.Cell(1, 4 + i).Value = new DateTime(2000, i + 1, 1).ToString("MMMM");
                }

                worksheet.Range("A1:P1").Style.Font.Bold = true;

                DateOnly? start = startDate.HasValue ? DateOnly.FromDateTime(startDate.Value) : null;
                DateOnly? end = endDate.HasValue ? DateOnly.FromDateTime(endDate.Value) : null;

                var violationTypes = new Dictionary<string, List<int>>
                {
                    { "Major Violations", new List<int> { 9, 10, 11 } },
                    { "Minor Violations", new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 } },
                    { "Major Traffic Violations", new List<int> { 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32 } },
                    { "Minor Traffic Violations", new List<int> { 12, 13, 14, 15, 16, 17, 18 } }
                };

                var allData = new List<ViolationStatsDto>();

                foreach (var (type, offenseIds) in violationTypes)
                {
                    var majorMinorData = _context.ReportsEncoded
                        .Where(r => offenseIds.Contains(r.OffenseId) &&
                                   (!start.HasValue || r.CommissionDate >= start) &&
                                   (!end.HasValue || r.CommissionDate <= end))
                        .AsEnumerable() // Forces client-side evaluation
                        .GroupBy(r => new { r.CollegeID, Month = r.CommissionDate.Month })
                        .Select(g => new ViolationStatsDto
                        {
                            College = g.Key.CollegeID,
                            ViolationType = type,
                            TotalViolations = g.Count(),
                            MonthlyViolations = g.GroupBy(r => r.CommissionDate.Month)
                                                 .ToDictionary(mg => mg.Key, mg => mg.Count())
                        })
                        .ToList();

                    allData.AddRange(majorMinorData);
                }

                int row = 2;
                foreach (var record in allData)
                {
                    worksheet.Cell(row, 1).Value = record.College;
                    worksheet.Cell(row, 2).Value = record.ViolationType;
                    worksheet.Cell(row, 3).Value = record.TotalViolations;

                    for (int i = 1; i <= 12; i++)
                    {
                        worksheet.Cell(row, 3 + i).Value = record.MonthlyViolations.ContainsKey(i) ? record.MonthlyViolations[i] : 0;
                    }

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
                }
            }
        }

    }


}


// Model for the request to include violation type

public class ExcelExportRequest
{
    public string FileName { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
public class ViolationStatsDto
{
    public string College { get; set; }
    public string ViolationType { get; set; }
    public int TotalViolations { get; set; }
    public Dictionary<int, int> MonthlyViolations { get; set; } = new();
    public int MajorViolations { get; set; } = 0;
    public int MinorViolations { get; set; } = 0;
    public int MajorTrafficViolations { get; set; } = 0;
    public int MinorTrafficViolations { get; set; } = 0;
}
public class ViolationRequest
{
    public string ViolationType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

