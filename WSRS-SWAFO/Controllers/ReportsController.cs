using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;
using WSRS_SWAFO.Data;
using WSRS_SWAFO.Data.Enum;
using WSRS_SWAFO.Models;
using Microsoft.EntityFrameworkCore;

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

                List<int> violationClassificationIds = new();
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
                    (!end.HasValue || r.CommissionDate <= end) &&
                    r.College != null && r.College.CollegeID != null)
        .GroupBy(r => r.College.CollegeID)
        .Select(g => new { College = g.Key, ViolationCount = g.Count() })
        .ToList()
    : _context.ReportsEncoded
        .Where(r => violationClassificationIds.Contains(r.OffenseId) &&
                    (!start.HasValue || r.CommissionDate >= start) &&
                    (!end.HasValue || r.CommissionDate <= end) &&
                    r.College != null && r.College.CollegeID != null)
        .GroupBy(r => r.College.CollegeID)
        .Select(g => new { College = g.Key, ViolationCount = g.Count() })
        .ToList();

                _logger.LogInformation("Fetched {count} reports", reports.Count);

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
        public IActionResult GenerateReport(string fileName, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "Student_Offenses_Report";
            }

            DateOnly start = DateOnly.FromDateTime(startDate);
            DateOnly end = DateOnly.FromDateTime(endDate);

            var reportRecords = _context.ReportsEncoded 
                .Include(r => r.College)
                .Where(r => r.CommissionDate >= start && r.CommissionDate <= end)
                .Select(r => new ReportEncoded 
                {
                    Id = r.Id,
                    CommissionDate = r.CommissionDate,
                    College = r.College
                })
                .ToList();

            var trafficRecords = _context.TrafficReportsEncoded
                .Include(t => t.College)
                .Where(t => t.CommissionDate >= start && t.CommissionDate <= end)
                .Select(t => new TrafficReportsEncoded
                {
                    Id = t.Id,
                    CommissionDate = t.CommissionDate,
                    College = t.College
                })
                .ToList();

            using var workbook = new XLWorkbook();
            PopulateWorksheet(workbook.Worksheets.Add("Total Colleges Reports"), reportRecords, trafficRecords);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
        }


        [HttpGet]
        public IActionResult ExportViolationsByCollege()
        {
            var violationCounts = _context.ReportsEncoded
                .Where(r => r.College != null) 
                .GroupBy(r => r.College.CollegeID)
                .Select(g => new { College = g.Key, ViolationCount = g.Count() });

            var trafficViolationCounts = _context.TrafficReportsEncoded
                .Where(t => t.College != null) 
                .GroupBy(t => t.College.CollegeID)
                .Select(g => new { College = g.Key, ViolationCount = g.Count() });

            var combinedViolations = violationCounts.Concat(trafficViolationCounts)
                .GroupBy(v => v.College)
                .Select(g => new { College = g.Key, TotalViolations = g.Sum(v => v.ViolationCount) })
                .OrderByDescending(v => v.TotalViolations)
                .ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Violations by College");

            // Headers
            worksheet.Cell(1, 1).Value = "College";
            worksheet.Cell(1, 2).Value = "Total Violations";
            worksheet.Range("A1:B1").Style.Font.Bold = true;

            // Insert data
            int row = 2;
            foreach (var item in combinedViolations)
            {
                worksheet.Cell(row, 1).Value = item.College;
                worksheet.Cell(row, 2).Value = item.TotalViolations;
                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ViolationsReport.xlsx");
        }




        private void PopulateWorksheet(IXLWorksheet worksheet, List<ReportEncoded> reports, List<TrafficReportsEncoded> trafficReports)
        {
            // ✅ College name mapping
            var collegeNames = new Dictionary<string, string>
    {
        { "CBAA", "College of Business Administration and Accountancy" },
        { "COED", "College of Education" },
        { "CEAT", "College of Engineering, Architecture and Technology" },
        { "CTHM", "College of Tourism and Hospitality Management" },
        { "CCJE", "College of Criminal Justice Education" },
        { "CLAC", "College of Liberal Arts and Communication" },
        { "CICS", "College of Information and Computer Studies" },
        { "COS", "College of Science" }
    };

            // Ensure we only fetch records within the selected date range
            var startDate = reports.Any() ? reports.Min(r => r.CommissionDate) : new DateOnly(2024, 1, 1);
            var endDate = reports.Any() ? reports.Max(r => r.CommissionDate) : new DateOnly(2025, 1, 1);

            // Fetch violations grouped by CollegeID and Month
            var violationRecords = reports
                .Where(r => r.CommissionDate >= startDate && r.CommissionDate <= endDate)
                .Select(r => new
                {
                    CollegeID = r.College?.CollegeID ?? "Unknown",
                    Month = r.CommissionDate.Month
                })
                .ToList();

            var trafficViolationRecords = trafficReports
                .Where(r => r.CommissionDate >= startDate && r.CommissionDate <= endDate)
                .Select(r => new
                {
                    CollegeID = r.College?.CollegeID ?? "Unknown",
                    Month = r.CommissionDate.Month
                })
                .ToList();

            // Merge both normal violations and traffic violations
            var allViolations = violationRecords.Concat(trafficViolationRecords)
                .GroupBy(v => v.CollegeID)
                .Select(g => new
                {
                    College = collegeNames.ContainsKey(g.Key) ? collegeNames[g.Key] : "Unknown College",  // ✅ Convert CollegeID to Full Name
                    MonthlyViolations = g.GroupBy(m => m.Month)
                                         .ToDictionary(m => m.Key, m => m.Count())
                })
                .ToList();

            // 📌 Column headers
            worksheet.Cell(1, 1).Value = "College";
            string[] months = { "August", "September", "October", "November", "December",
                        "January", "February", "March", "April", "May", "June", "July" };

            for (int i = 0; i < months.Length; i++)
            {
                worksheet.Cell(1, i + 2).Value = months[i];
            }
            worksheet.Cell(1, 14).Value = "Total";

            worksheet.Row(1).Style.Font.Bold = true;

            int row = 2;

            // 🏫 Insert college-wise data
            foreach (var record in allViolations)
            {
                worksheet.Cell(row, 1).Value = record.College; // ✅ Full College Name

                int totalViolations = 0;

                for (int month = 8, col = 2; month <= 12; month++, col++)
                {
                    int count = record.MonthlyViolations.TryGetValue(month, out int val) ? val : 0;
                    worksheet.Cell(row, col).Value = count;
                    totalViolations += count;
                }

                for (int month = 1, col = 7; month <= 7; month++, col++)
                {
                    int count = record.MonthlyViolations.TryGetValue(month, out int val) ? val : 0;
                    worksheet.Cell(row, col).Value = count;
                    totalViolations += count;
                }

                worksheet.Cell(row, 14).Value = totalViolations;
                row++;
            }

            // 🟡 Add the total violations per month row
            worksheet.Cell(row, 1).Value = "Total No. of Violations Per Month";
            worksheet.Row(row).Style.Fill.BackgroundColor = XLColor.Yellow;
            worksheet.Row(row).Style.Font.Bold = true;

            for (int col = 2; col <= 14; col++)
            {
                int sum = 0;
                for (int r = 2; r < row; r++)
                {
                    sum += worksheet.Cell(r, col).GetValue<int>();
                }
                worksheet.Cell(row, col).Value = sum;
            }

            // Autofit columns for better readability
            worksheet.Columns().AdjustToContents();
        }



        private string GetCollege<T>(T record)
        {
            if (record is ReportEncoded report)
            {
                if (report.College == null || string.IsNullOrWhiteSpace(report.College.CollegeID))
                {
                    _logger.LogWarning("Missing College for ReportEncoded record.");
                    return "Unknown";
                }
                return report.College.CollegeID;
            }

            if (record is TrafficReportsEncoded trafficReport)
            {
                if (trafficReport.College == null || string.IsNullOrWhiteSpace(trafficReport.College.CollegeID))
                {
                    _logger.LogWarning("Missing College for TrafficReportsEncoded record.");
                    return "Unknown";
                }
                return trafficReport.College.CollegeID;
            }

            return "Unknown";
        }


        private int GetMonth<T>(T record)
        {
            if (record is ReportEncoded report)
                return report.CommissionDate.Month;
            if (record is TrafficReportsEncoded trafficReport)
                return trafficReport.CommissionDate.Month;

            return 0;
        }


        public class ViolationRequest
        {
            public string ViolationType { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
    }
}
