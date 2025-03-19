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
using DocumentFormat.OpenXml.Bibliography;

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

            // Ensure date range includes full last day
            DateTime startDateTime = startDate.Date;
            DateTime endDateTime = endDate.Date.AddDays(1).AddSeconds(-1);

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

            // Retrieve selected violations within the date range
            var selectedViolations = _context.ReportsEncoded
                .Where(r => r.CommissionDate >= DateOnly.FromDateTime(startDate) &&
                            r.CommissionDate <= DateOnly.FromDateTime(endDateTime))
                .Select(v => new
                {
                    v.CollegeID,
                    Classification = v.Offense.Classification,
                    v.CommissionDate.Year,
                    v.CommissionDate.Month,
                    Count = 1
                });

            var selectedTrafficViolations = _context.TrafficReportsEncoded
                .Where(r => r.CommissionDate >= DateOnly.FromDateTime(startDate) &&
                            r.CommissionDate <= DateOnly.FromDateTime(endDateTime))
                .Select(v => new
                {
                    v.CollegeID,
                    Classification = v.Offense.Classification,
                    v.CommissionDate.Year,
                    v.CommissionDate.Month,
                    Count = 1
                });

            var groupedViolations = selectedViolations
                .Concat(selectedTrafficViolations)
                .GroupBy(v => new { v.CollegeID, v.Classification, v.Year, v.Month })
                .Select(g => new
                {
                    College = collegeNames.ContainsKey(g.Key.CollegeID) ? collegeNames[g.Key.CollegeID] : "Unknown College",
                    Classification = g.Key.Classification,
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToList();

            using var workbook = new XLWorkbook();
            var violationSheet = workbook.Worksheets.Add("Violations Report");

            var categories = new Dictionary<string, string>
    {
        { "Total Violations", "All" },
        { "Minor Offenses", "Minor" },
        { "Major Offenses", "Major" },
        { "Minor Traffic Violations", "MinorTraffic" },
        { "Major Traffic Violations", "MajorTraffic" }
    };

            int currentRow = 1;
            violationSheet.Cell(currentRow, 1).Value = "Violations Report";
            violationSheet.Row(currentRow).Style.Font.Bold = true;
            violationSheet.Row(currentRow).Style.Font.FontSize = 14;
            violationSheet.Row(currentRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            violationSheet.Range(currentRow, 1, currentRow, 6).Merge();
            currentRow += 2;

            foreach (var category in categories)
            {
                violationSheet.Cell(currentRow, 1).Value = category.Key;
                violationSheet.Row(currentRow).Style.Font.Bold = true;
                violationSheet.Row(currentRow).Style.Font.FontSize = 12;
                violationSheet.Row(currentRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                violationSheet.Range(currentRow, 1, currentRow, 6).Merge();
                currentRow++;

                // Add header row
                violationSheet.Cell(currentRow, 1).Value = "College";
                int col = 2;
                List<DateTime> months = new();
                for (DateTime date = startDateTime; date <= endDateTime; date = date.AddMonths(1))
                {
                    months.Add(date);
                    violationSheet.Cell(currentRow, col).Value = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month)} {date.Year}";
                    col++;
                }
                violationSheet.Cell(currentRow, col).Value = "Total";
                violationSheet.Row(currentRow).Style.Font.Bold = true;
                violationSheet.Row(currentRow).Style.Fill.BackgroundColor = XLColor.LightGray;
                currentRow++;

                // Filter violations based on category
                var categoryEnum = category.Value != "All"
                    ? Enum.Parse<OffenseClassification>(category.Value)
                    : (OffenseClassification?)null;

                var categoryViolations = groupedViolations
                    .Where(v => categoryEnum == null || v.Classification == categoryEnum)
                    .GroupBy(v => v.College)
                    .Select(g => new
                    {
                        College = g.Key,
                        MonthlyCounts = months.Select(m => g.Where(v => v.Year == m.Year && v.Month == m.Month).Sum(v => v.Count)).ToList(),
                        Total = g.Sum(v => v.Count)
                    })
                    .ToList();

                // Populate data per college
                foreach (var record in categoryViolations)
                {
                    violationSheet.Cell(currentRow, 1).Value = record.College;
                    for (int i = 0; i < record.MonthlyCounts.Count; i++)
                    {
                        violationSheet.Cell(currentRow, i + 2).Value = record.MonthlyCounts[i];
                    }
                    violationSheet.Cell(currentRow, record.MonthlyCounts.Count + 2).Value = record.Total;
                    currentRow++;
                }

                // Total per month
                violationSheet.Cell(currentRow, 1).Value = $"Total No. of {category.Key} Committed Per Month";
                for (int i = 0; i < months.Count; i++)
                {
                    violationSheet.Cell(currentRow, i + 2).Value = categoryViolations.Sum(v => v.MonthlyCounts[i]);
                }
                violationSheet.Cell(currentRow, months.Count + 2).Value = categoryViolations.Sum(v => v.Total);

                violationSheet.Range(currentRow, 1, currentRow, months.Count + 2).Style.Fill.BackgroundColor = XLColor.Yellow;
                violationSheet.Row(currentRow).Style.Font.Bold = true;
                currentRow += 2;
            }

            violationSheet.Columns().AdjustToContents();

            // **Stats-Population per College sheet**
            var statsPopulationRep = workbook.Worksheets.Add("Stats-Population per College");

            int row = 1;
            statsPopulationRep.Cell(row, 1).Value = "Comparative Statistics of Violations per College";
            statsPopulationRep.Row(row).Style.Font.Bold = true;
            statsPopulationRep.Row(row).Style.Font.FontSize = 14;
            statsPopulationRep.Row(row).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            statsPopulationRep.Range(row, 1, row, 2).Merge();
            row += 2;

            // Function to add sections dynamically
            void AddViolationCategory(string title, Dictionary<string, int> data)
            {
                statsPopulationRep.Cell(row, 1).Value = title;
                statsPopulationRep.Row(row).Style.Font.Bold = true;
                statsPopulationRep.Row(row).Style.Fill.BackgroundColor = XLColor.Gray;
                statsPopulationRep.Range(row, 1, row, 2).Merge();
                row++;

                // Headers
                statsPopulationRep.Cell(row, 1).Value = "College";
                statsPopulationRep.Cell(row, 2).Value = "Total Violations";
                statsPopulationRep.Cell(row, 3).Value = "Total College Population ( INPUT HERE )";
                statsPopulationRep.Cell(row, 4).Value = "Equivalent Percentage of Violations per College ( INPUT HERE )";
                statsPopulationRep.Row(row).Style.Font.Bold = true;
                statsPopulationRep.Row(row).Style.Fill.BackgroundColor = XLColor.LightGray;
                row++;

                // Data Rows
                foreach (var item in data)
                {
                    statsPopulationRep.Cell(row, 1).Value = item.Key;
                    statsPopulationRep.Cell(row, 2).Value = item.Value;
                    row++;
                }

                // Total Row
                statsPopulationRep.Cell(row, 1).Value = $"Total {title}";
                statsPopulationRep.Cell(row, 2).Value = data.Values.Sum();
                statsPopulationRep.Row(row).Style.Font.Bold = true;
                statsPopulationRep.Range(row, 1, row, 2).Style.Fill.BackgroundColor = XLColor.Yellow;
                row += 2;
            }

            // Group violations by category
            var totalViolations = groupedViolations
                .GroupBy(v => v.College)
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Count));

            var minorViolations = groupedViolations
                .Where(v => v.Classification.ToString() == "Minor")
                .GroupBy(v => v.College)
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Count));

            var majorViolations = groupedViolations
                .Where(v => v.Classification.ToString() == "Major")
                .GroupBy(v => v.College)
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Count));

            var minorTrafficViolations = groupedViolations
                .Where(v => v.Classification.ToString() == "MinorTraffic")
                .GroupBy(v => v.College)
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Count));

            var majorTrafficViolations = groupedViolations
                .Where(v => v.Classification.ToString() == "MajorTraffic")
                .GroupBy(v => v.College)
                .ToDictionary(g => g.Key, g => g.Sum(v => v.Count));

            // Add sections to the Excel sheet
            AddViolationCategory("Total Violations by College", totalViolations);
            AddViolationCategory("Minor Offenses by College", minorViolations);
            AddViolationCategory("Major Offenses by College", majorViolations);
            AddViolationCategory("Minor Traffic Violations by College", minorTrafficViolations);
            AddViolationCategory("Major Traffic Violations by College", majorTrafficViolations);

            statsPopulationRep.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
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
