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

            var selectedViolations = _context.ReportsEncoded
             .Where(r => r.CommissionDate >= start && r.CommissionDate <= end && r.College != null)
             .Select(r => new { CollegeID = r.College.CollegeID, Month = r.CommissionDate.Month, Classification = r.Offense.Classification })
             .ToList();

            var selectedTrafficViolations = _context.TrafficReportsEncoded
            .Where(r => r.CommissionDate >= start && r.CommissionDate <= end && r.College != null)
            .Select(r => new { CollegeID = r.College.CollegeID, Month = r.CommissionDate.Month, Classification = r.Offense.Classification })
            .ToList();

            var groupedViolations = selectedViolations.Concat(selectedTrafficViolations)
            .GroupBy(v => new { v.CollegeID, v.Classification, v.Month })
            .Select(g => new
            {
                College = collegeNames.ContainsKey(g.Key.CollegeID) ? collegeNames[g.Key.CollegeID] : "Unknown College",
                Classification = g.Key.Classification,
                Month = g.Key.Month,
                Count = g.Count()
            })
            .ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Violations Report");

            var categories = new Dictionary<string, string>
            {
                { "Total Violations", "All" },
                { "Minor Offenses", "Minor" },
                { "Major Offenses", "Major" },
                { "Minor Traffic Violations", "MinorTraffic" },
                { "Major Traffic Violations", "MajorTraffic" }
            };


            int currentRow = 1;
            List<int> selectedMonths = Enumerable.Range(start.Month, end.Month - start.Month + 1).ToList();
            string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

            foreach (var category in categories)
            {
                worksheet.Cell(currentRow, 1).Value = category.Key;
                worksheet.Row(currentRow).Style.Font.Bold = true;
                currentRow++;

                worksheet.Cell(currentRow, 1).Value = "College";
                worksheet.Row(currentRow).Style.Font.Bold = true;

                for (int i = 0; i < selectedMonths.Count; i++)
                {
                    worksheet.Cell(currentRow, i + 2).Value = monthNames[selectedMonths[i] - 1];
                }
                worksheet.Cell(currentRow, selectedMonths.Count + 2).Value = "Total";
                currentRow++;

                var categoryViolations = groupedViolations
    .Where(v => category.Value == "All" || v.Classification.ToString() == category.Value)
    .GroupBy(v => v.College)
    .Select(g => new
    {
        College = g.Key,
        MonthlyViolations = g.GroupBy(m => m.Month)
                             .ToDictionary(m => m.Key, m => m.Sum(v => v.Count))
    })
    .ToList();
                if (categoryViolations.Any())
                {
                    foreach (var record in categoryViolations)
                    {
                        worksheet.Cell(currentRow, 1).Value = record.College;
                        int total = 0;

                        for (int i = 0; i < selectedMonths.Count; i++)
                        {
                            int month = selectedMonths[i];
                            int count = record.MonthlyViolations.TryGetValue(month, out int val) ? val : 0;
                            worksheet.Cell(currentRow, i + 2).Value = count;
                            total += count;
                        }

                        worksheet.Cell(currentRow, selectedMonths.Count + 2).Value = total;
                        currentRow++;
                    }
                }
                else
                {
                    worksheet.Cell(currentRow, 1).Value = "No Data Available";
                    currentRow++;
                }

                // 🔸 Insert the "Total No. of [Category] Committed Per Month" row
                worksheet.Cell(currentRow, 1).Value = $"Total No. of {category.Key} Committed Per Month";
                worksheet.Range(currentRow, 1, currentRow, selectedMonths.Count + 2).Style.Fill.BackgroundColor = XLColor.Yellow;
                worksheet.Row(currentRow).Style.Font.Bold = true;

                int grandTotal = 0;
                for (int i = 0; i < selectedMonths.Count; i++)
                {
                    int month = selectedMonths[i];
                    int totalForMonth = categoryViolations.Sum(r => r.MonthlyViolations.TryGetValue(month, out int val) ? val : 0);
                    worksheet.Cell(currentRow, i + 2).Value = totalForMonth;
                    grandTotal += totalForMonth;
                }

                // 🔹 Insert Grand Total at the Last Column
                worksheet.Cell(currentRow, selectedMonths.Count + 2).Value = grandTotal;
                worksheet.Cell(currentRow, selectedMonths.Count + 2).Style.Font.Bold = true;

                // Move to next row after the summary
                currentRow++;

            }
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
        }

        [HttpGet]
        public IActionResult ExportViolationsByCollege()
        {
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
            var violationCounts = _context.ReportsEncoded
                .Where(r => r.College != null)
                .GroupBy(r => r.College.CollegeID)
                .Select(g => new
                {
                    College = g.Key,
                    TotalViolations = g.Count(),
                    MonthlyViolations = g.GroupBy(r => r.CommissionDate.Month)
                                         .ToDictionary(m => m.Key, m => m.Count())
                });

            var trafficViolationCounts = _context.TrafficReportsEncoded
                .Where(t => t.College != null)
                .GroupBy(t => t.College.CollegeID)
                .Select(g => new
                {
                    College = g.Key,
                    TotalViolations = g.Count(),
                    MonthlyViolations = g.GroupBy(t => t.CommissionDate.Month)
                                         .ToDictionary(m => m.Key, m => m.Count())
                });

            var combinedViolations = violationCounts.Concat(trafficViolationCounts)
                .GroupBy(v => v.College)
                .Select(g => new
                {
                    College = collegeNames.ContainsKey(g.Key) ? collegeNames[g.Key] : "Unknown College",
                    TotalViolations = g.Sum(v => v.TotalViolations),
                    MonthlyViolations = g.SelectMany(v => v.MonthlyViolations)
                                         .GroupBy(m => m.Key)
                                         .ToDictionary(m => m.Key, m => m.Sum(v => v.Value))
                })
                .OrderByDescending(v => v.TotalViolations)
                .ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Violations by College");

            worksheet.Cell(1, 1).Value = "College";
            worksheet.Cell(1, 2).Value = "Total Violations";

            var allMonths = combinedViolations.SelectMany(v => v.MonthlyViolations.Keys).Distinct().OrderBy(m => m).ToList();
            string[] monthNames = CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;

            for (int i = 0; i < allMonths.Count; i++)
            {
                worksheet.Cell(1, i + 3).Value = monthNames[allMonths[i] - 1];
            }

            worksheet.Row(1).Style.Font.Bold = true;

            int row = 2;
            var schoolMonthlyTotals = new Dictionary<int, int>();

            foreach (var item in combinedViolations)
            {
                worksheet.Cell(row, 1).Value = item.College;
                worksheet.Cell(row, 2).Value = item.TotalViolations;

                for (int i = 0; i < allMonths.Count; i++)
                {
                    int month = allMonths[i];
                    int count = item.MonthlyViolations.ContainsKey(month) ? item.MonthlyViolations[month] : 0;
                    worksheet.Cell(row, i + 3).Value = count;

                    if (!schoolMonthlyTotals.ContainsKey(month))
                        schoolMonthlyTotals[month] = 0;
                    schoolMonthlyTotals[month] += count;
                }
                row++;
            }

            worksheet.Cell(row, 1).Value = "TOTAL SCHOOL-WIDE VIOLATIONS";
            worksheet.Row(row).Style.Font.Bold = true;

            int overallTotal = 0;
            for (int i = 0; i < allMonths.Count; i++)
            {
                int month = allMonths[i];
                int total = schoolMonthlyTotals.ContainsKey(month) ? schoolMonthlyTotals[month] : 0;
                worksheet.Cell(row, i + 3).Value = total;
                overallTotal += total;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ViolationsReport.xlsx");
        }

        private void PopulateWorksheet(IXLWorksheet worksheet, List<ReportEncoded> reports, List<TrafficReportsEncoded> trafficReports)
        {
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
            var startDate = reports.Any() ? reports.Min(r => r.CommissionDate) : new DateOnly(2024, 1, 1);
            var endDate = reports.Any() ? reports.Max(r => r.CommissionDate) : new DateOnly(2025, 1, 1);

            var violationRecords = reports
                .Where(r => r.CommissionDate >= startDate && r.CommissionDate <= endDate)
                .Select(r => new { CollegeID = r.College?.CollegeID ?? "Unknown", Month = r.CommissionDate.Month })
                .ToList();

            var trafficViolationRecords = trafficReports
                .Where(r => r.CommissionDate >= startDate && r.CommissionDate <= endDate)
                .Select(r => new { CollegeID = r.College?.CollegeID ?? "Unknown", Month = r.CommissionDate.Month })
                .ToList();

            var allViolations = violationRecords.Concat(trafficViolationRecords)
                .GroupBy(v => v.CollegeID)
                .Select(g => new
                {
                    College = collegeNames.ContainsKey(g.Key) ? collegeNames[g.Key] : "Unknown College",
                    MonthlyViolations = g.GroupBy(m => m.Month)
                                         .ToDictionary(m => m.Key, m => m.Count())
                })
                .ToList();

            var totalViolationsPerMonth = allViolations
                .SelectMany(record => record.MonthlyViolations)
                .GroupBy(v => v.Key)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalViolations = g.Sum(v => v.Value)
                })
                .OrderBy(g => g.Month)
                .ToList();

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
            var monthlyTotals = new Dictionary<int, int>();

            foreach (var record in allViolations)
            {
                worksheet.Cell(row, 1).Value = record.College;
                int totalViolations = 0;

                for (int month = 8, col = 2; month <= 12; month++, col++)
                {
                    int count = record.MonthlyViolations.TryGetValue(month, out int val) ? val : 0;
                    worksheet.Cell(row, col).Value = count;
                    totalViolations += count;
                    if (!monthlyTotals.ContainsKey(month)) monthlyTotals[month] = 0;
                    monthlyTotals[month] += count;
                }

                for (int month = 1, col = 7; month <= 7; month++, col++)
                {
                    int count = record.MonthlyViolations.TryGetValue(month, out int val) ? val : 0;
                    worksheet.Cell(row, col).Value = count;
                    totalViolations += count;
                    if (!monthlyTotals.ContainsKey(month)) monthlyTotals[month] = 0;
                    monthlyTotals[month] += count;
                }
                worksheet.Cell(row, 14).Value = totalViolations;
                row++;
            }
            // Move row down after college records
            // ✅ Ensure the next row after all colleges is used for totals
            row++;

            // 🟡 Add the "Total Violations Per Month" row
            worksheet.Cell(row, 1).Value = "Total Violations Per Month";
            worksheet.Range(row, 1, row, 14).Style.Fill.BackgroundColor = XLColor.Yellow;
            worksheet.Range(row, 1, row, 14).Style.Font.Bold = true;

            // ✅ Insert total violations per month for August to December
            for (int month = 8, col = 2; month <= 12; month++, col++)
            {
                worksheet.Cell(row, col).Value = monthlyTotals.ContainsKey(month) ? monthlyTotals[month] : 0;
                worksheet.Cell(row, col).Style.Font.Bold = true;
            }

            // ✅ Insert total violations per month for January to July
            for (int month = 1, col = 7; month <= 7; month++, col++)
            {
                worksheet.Cell(row, col).Value = monthlyTotals.ContainsKey(month) ? monthlyTotals[month] : 0;
                worksheet.Cell(row, col).Style.Font.Bold = true;
            }

            // ✅ Compute total for all months (sum of all months' total)
            int grandTotal = monthlyTotals.Values.Sum();
            worksheet.Cell(row, 14).Value = grandTotal;
            worksheet.Cell(row, 14).Style.Font.Bold = true;

            // ✅ Auto-fit columns for better readability
            worksheet.Columns().AdjustToContents();

            // 🔍 Debugging: Print row index
            Console.WriteLine($"Total Violations Per Month written at row {row}");


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
