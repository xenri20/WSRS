using System.Diagnostics;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using WSRS_SWAFO.Data;
using WSRS_SWAFO.Data.Enum;
using WSRS_SWAFO.Models;

namespace WSRS_SWAFO.Controllers
{
    [Authorize(Roles = "AppRole.Admin, AppRole.Member")]
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

            // Date Range (Adjust format as needed)
            violationSheet.Cell(currentRow, 1).Value = $"{startDate:MMMM yyyy} - {endDateTime:MMMM yyyy}";
            violationSheet.Row(currentRow).Style.Font.Bold = true;
            violationSheet.Row(currentRow).Style.Font.FontSize = 12;
            violationSheet.Row(currentRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            violationSheet.Range(currentRow, 1, currentRow, 2).Merge();
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

            // Date Range (Adjust format as needed)
            statsPopulationRep.Cell(row, 1).Value = $"{startDate:MMMM yyyy} - {endDateTime:MMMM yyyy}";
            statsPopulationRep.Row(row).Style.Font.Bold = true;
            statsPopulationRep.Row(row).Style.Font.FontSize = 12;
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

            var sanctionNatureList = workbook.Worksheets.Add("Nature of sanctions");


            // Title: "SANCTIONS IMPOSED FOR MAJOR OFFENSES COMMITTED"
            int sanctionRow = 1;
            sanctionNatureList.Cell(sanctionRow, 1).Value = "SANCTIONS IMPOSED FOR MAJOR OFFENSES COMMITTED";
            sanctionNatureList.Row(sanctionRow).Style.Font.Bold = true;
            sanctionNatureList.Row(sanctionRow).Style.Font.FontSize = 14;
            sanctionNatureList.Row(sanctionRow).Style.Font.FontColor = XLColor.Red;
            sanctionNatureList.Row(sanctionRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sanctionNatureList.Range(sanctionRow, 1, sanctionRow, 2).Merge();
            sanctionRow++;

            // Date Range (Adjust format as needed)
            sanctionNatureList.Cell(sanctionRow, 1).Value = $"{startDate:MMMM yyyy} - {endDateTime:MMMM yyyy}";
            sanctionNatureList.Row(sanctionRow).Style.Font.Bold = true;
            sanctionNatureList.Row(sanctionRow).Style.Font.FontSize = 12;
            sanctionNatureList.Row(sanctionRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            sanctionNatureList.Range(sanctionRow, 1, sanctionRow, 2).Merge();
            sanctionRow += 2;

            // Headers
            sanctionNatureList.Cell(sanctionRow, 1).Value = "Nature of Sanctions";
            sanctionNatureList.Cell(sanctionRow, 2).Value = "Frequency";
            sanctionNatureList.Row(sanctionRow).Style.Font.Bold = true;
            sanctionNatureList.Row(sanctionRow).Style.Fill.BackgroundColor = XLColor.Gray;
            sanctionNatureList.Row(sanctionRow).Style.Font.FontColor = XLColor.White;
            sanctionRow++;

            // **Get All Sanctions Grouped and Counted**
            var sanctionsByNature = _context.ReportsEncoded
                .Where(r => r.CommissionDate >= DateOnly.FromDateTime(startDate) &&
                            r.CommissionDate <= DateOnly.FromDateTime(endDateTime))
                .Where(r => !string.IsNullOrEmpty(r.Sanction)) // Ensure no empty values
                .GroupBy(r => r.Sanction)
                .Select(g => new
                {
                    Nature = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(v => v.Count)
                .ToList();

            // **Write Data (Sanction & Count)**
            int totalSanctions = 0;
            foreach (var record in sanctionsByNature)
            {
                sanctionNatureList.Cell(sanctionRow, 1).Value = record.Nature;
                sanctionNatureList.Cell(sanctionRow, 2).Value = record.Count;
                totalSanctions += record.Count;
                sanctionRow++;
            }

            // **Total Row**
            sanctionNatureList.Cell(sanctionRow, 1).Value = "TOTAL";
            sanctionNatureList.Cell(sanctionRow, 2).Value = totalSanctions;
            sanctionNatureList.Row(sanctionRow).Style.Font.Bold = true;
            sanctionNatureList.Row(sanctionRow).Style.Font.FontColor = XLColor.Red;
            sanctionNatureList.Row(sanctionRow).Style.Fill.BackgroundColor = XLColor.Yellow;

            // **Adjust Column Width for Readability**
            sanctionNatureList.Columns().AdjustToContents();



            var majorByNatureSheet = workbook.Worksheets.Add("Major Offense by Nature");

            // Title: "Major Offenses by Nature"
            int majorNatureRow = 1;
            majorByNatureSheet.Cell(majorNatureRow, 1).Value = "Major Offenses by Nature";
            majorByNatureSheet.Row(majorNatureRow).Style.Font.Bold = true;
            majorByNatureSheet.Row(majorNatureRow).Style.Font.FontSize = 14;
            majorByNatureSheet.Row(majorNatureRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            majorByNatureSheet.Range(majorNatureRow, 1, majorNatureRow, 3).Merge();
            majorNatureRow += 2;

            // Date Range (Adjust format as needed)
            majorByNatureSheet.Cell(majorNatureRow, 1).Value = $"{startDate:MMMM yyyy} - {endDateTime:MMMM yyyy}";
            majorByNatureSheet.Row(majorNatureRow).Style.Font.Bold = true;
            majorByNatureSheet.Row(majorNatureRow).Style.Font.FontSize = 12;
            majorByNatureSheet.Row(majorNatureRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            majorByNatureSheet.Range(majorNatureRow, 1, majorNatureRow, 2).Merge();
            majorNatureRow += 2;

            // Headers
            majorByNatureSheet.Cell(majorNatureRow, 1).Value = "Nature of Offense";
            majorByNatureSheet.Cell(majorNatureRow, 2).Value = "Total Cases";
            majorByNatureSheet.Cell(majorNatureRow, 3).Value = "Total Descriptions";
            majorByNatureSheet.Row(majorNatureRow).Style.Font.Bold = true;
            majorByNatureSheet.Row(majorNatureRow).Style.Fill.BackgroundColor = XLColor.Gray;
            majorByNatureSheet.Row(majorNatureRow).Style.Font.FontColor = XLColor.White;
            majorNatureRow++;

            // **Get Major Offenses Grouped by Nature**
            var majorViolationsByNature = _context.ReportsEncoded
                .Where(r => r.CommissionDate >= DateOnly.FromDateTime(startDate) &&
                            r.CommissionDate <= DateOnly.FromDateTime(endDateTime) &&
                            r.Offense.Classification == OffenseClassification.Major)
                .GroupBy(v => v.Offense.Nature)
                .Select(g => new
                {
                    Nature = g.Key,
                    Count = g.Count(),
                    DescriptionCount = g.Count(v => !string.IsNullOrEmpty(v.Description)),
                    Descriptions = g.Select(v => v.Description).Where(d => !string.IsNullOrEmpty(d)).ToList()
                })
                .OrderByDescending(v => v.Count)
                .ToList();

            // **Write Data (Nature of Offense & Total Cases)**
            foreach (var record in majorViolationsByNature)
            {
                majorByNatureSheet.Cell(majorNatureRow, 1).Value = record.Nature;
                majorByNatureSheet.Cell(majorNatureRow, 2).Value = record.Count;
                majorByNatureSheet.Cell(majorNatureRow, 3).Value = record.DescriptionCount;
                majorNatureRow++;
            }

            // **Add a Space Before Descriptions**
            majorNatureRow += 1;

            // **Section Header: "STATISTICS OF MAJOR OFFENSES COMMITTED BY STUDENTS"**
            majorByNatureSheet.Cell(majorNatureRow, 1).Value = "STATISTICS OF MAJOR OFFENSES COMMITTED BY STUDENTS";
            majorByNatureSheet.Row(majorNatureRow).Style.Font.Bold = true;
            majorByNatureSheet.Row(majorNatureRow).Style.Font.FontSize = 12;
            majorByNatureSheet.Row(majorNatureRow).Style.Font.FontColor = XLColor.Red;
            majorByNatureSheet.Row(majorNatureRow).Style.Fill.BackgroundColor = XLColor.White;
            majorByNatureSheet.Range(majorNatureRow, 1, majorNatureRow, 3).Merge();
            majorNatureRow += 2;

            // **Headers for Description Section**
            majorByNatureSheet.Cell(majorNatureRow, 1).Value = "Description";
            majorByNatureSheet.Cell(majorNatureRow, 2).Value = "Frequency";
            majorByNatureSheet.Row(majorNatureRow).Style.Font.Bold = true;
            majorByNatureSheet.Row(majorNatureRow).Style.Fill.BackgroundColor = XLColor.LightGray;
            majorNatureRow++;

            // **Write Descriptions with Frequency**
            foreach (var record in majorViolationsByNature)
            {
                foreach (var description in record.Descriptions.GroupBy(d => d)
                                                              .Select(g => new { Text = g.Key, Count = g.Count() }))
                {
                    majorByNatureSheet.Cell(majorNatureRow, 1).Value = description.Text;
                    majorByNatureSheet.Cell(majorNatureRow, 2).Value = description.Count;
                    majorNatureRow++;
                }
            }

            // **Adjust Column Width for Readability**
            majorByNatureSheet.Columns().AdjustToContents();

            var minorByNatureSheet = workbook.Worksheets.Add("Minor Offense by Nature");

            // Title: "Minor Offenses by Nature"
            int minorNatureRow = 1;
            minorByNatureSheet.Cell(minorNatureRow, 1).Value = "Minor Offenses by Nature";
            minorByNatureSheet.Row(minorNatureRow).Style.Font.Bold = true;
            minorByNatureSheet.Row(minorNatureRow).Style.Font.FontSize = 14;
            minorByNatureSheet.Row(minorNatureRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            minorByNatureSheet.Range(minorNatureRow, 1, minorNatureRow, 3).Merge();
            minorNatureRow += 2;

            // Date Range (Adjust format as needed)
            minorByNatureSheet.Cell(minorNatureRow, 1).Value = $"{startDate:MMMM yyyy} - {endDateTime:MMMM yyyy}";
            minorByNatureSheet.Row(minorNatureRow).Style.Font.Bold = true;
            minorByNatureSheet.Row(minorNatureRow).Style.Font.FontSize = 12;
            minorByNatureSheet.Row(minorNatureRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            minorByNatureSheet.Range(minorNatureRow, 1, minorNatureRow, 2).Merge();
            minorNatureRow += 2;

            // Headers
            minorByNatureSheet.Cell(minorNatureRow, 1).Value = "Nature of Offense";
            minorByNatureSheet.Cell(minorNatureRow, 2).Value = "Total Cases";
            minorByNatureSheet.Cell(minorNatureRow, 3).Value = "Total Descriptions";
            minorByNatureSheet.Row(minorNatureRow).Style.Font.Bold = true;
            minorByNatureSheet.Row(minorNatureRow).Style.Fill.BackgroundColor = XLColor.Gray;
            minorByNatureSheet.Row(minorNatureRow).Style.Font.FontColor = XLColor.White;
            minorNatureRow++;

            // **Get Minor Offenses Grouped by Nature**
            var minorViolationsByNature = _context.ReportsEncoded
                .Where(r => r.CommissionDate >= DateOnly.FromDateTime(startDate) &&
                            r.CommissionDate <= DateOnly.FromDateTime(endDateTime) &&
                            r.Offense.Classification == OffenseClassification.Minor)
                .GroupBy(v => v.Offense.Nature)
                .Select(g => new
                {
                    Nature = g.Key,
                    Count = g.Count(),
                    DescriptionCount = g.Count(v => !string.IsNullOrEmpty(v.Description)),
                    Descriptions = g.Select(v => v.Description).Where(d => !string.IsNullOrEmpty(d)).ToList()
                })
                .OrderByDescending(v => v.Count)
                .ToList();

            // **Write Data (Nature of Offense & Total Cases)**
            foreach (var record in minorViolationsByNature)
            {
                minorByNatureSheet.Cell(minorNatureRow, 1).Value = record.Nature;
                minorByNatureSheet.Cell(minorNatureRow, 2).Value = record.Count;
                minorByNatureSheet.Cell(minorNatureRow, 3).Value = record.DescriptionCount;
                minorNatureRow++;
            }

            // **Add a Space Before Descriptions**
            minorNatureRow += 1;

            // **Section Header: "STATISTICS OF MINOR OFFENSES COMMITTED BY STUDENTS"**
            minorByNatureSheet.Cell(minorNatureRow, 1).Value = "STATISTICS OF MINOR OFFENSES COMMITTED BY STUDENTS";
            minorByNatureSheet.Row(minorNatureRow).Style.Font.Bold = true;
            minorByNatureSheet.Row(minorNatureRow).Style.Font.FontSize = 12;
            minorByNatureSheet.Row(minorNatureRow).Style.Font.FontColor = XLColor.Red;
            minorByNatureSheet.Row(minorNatureRow).Style.Fill.BackgroundColor = XLColor.White;
            minorByNatureSheet.Range(minorNatureRow, 1, minorNatureRow, 3).Merge();
            minorNatureRow += 2;

            // **Headers for Description Section**
            minorByNatureSheet.Cell(minorNatureRow, 1).Value = "Description";
            minorByNatureSheet.Cell(minorNatureRow, 2).Value = "Frequency";
            minorByNatureSheet.Row(minorNatureRow).Style.Font.Bold = true;
            minorByNatureSheet.Row(minorNatureRow).Style.Fill.BackgroundColor = XLColor.LightGray;
            minorNatureRow++;

            // **Write Descriptions with Frequency**
            foreach (var record in minorViolationsByNature)
            {
                foreach (var description in record.Descriptions.GroupBy(d => d)
                                                              .Select(g => new { Text = g.Key, Count = g.Count() }))
                {
                    minorByNatureSheet.Cell(minorNatureRow, 1).Value = description.Text;
                    minorByNatureSheet.Cell(minorNatureRow, 2).Value = description.Count;
                    minorNatureRow++;
                }
            }

            // **Adjust Column Width for Readability**
            var minorTrafficByNatureSheet = workbook.Worksheets.Add("Minor Traffic Offense by Nature");

            // Title: "Minor Traffic Offenses by Nature"
            int minorTrafficNatureRow = 1;
            minorTrafficByNatureSheet.Cell(minorTrafficNatureRow, 1).Value = "Minor Traffic Offenses by Nature";
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Font.Bold = true;
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Font.FontSize = 14;
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            minorTrafficByNatureSheet.Range(minorTrafficNatureRow, 1, minorTrafficNatureRow, 2).Merge();
            minorTrafficNatureRow += 2;

            minorTrafficByNatureSheet.Cell(minorTrafficNatureRow, 1).Value = $"{startDate:MMMM yyyy} - {endDateTime:MMMM yyyy}";
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Font.Bold = true;
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Font.FontSize = 12;
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            minorTrafficByNatureSheet.Range(minorTrafficNatureRow, 1, minorTrafficNatureRow, 2).Merge();
            minorTrafficNatureRow += 2;

            // Headers
            minorTrafficByNatureSheet.Cell(minorTrafficNatureRow, 1).Value = "Nature of Offense";
            minorTrafficByNatureSheet.Cell(minorTrafficNatureRow, 2).Value = "Total Cases";
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Font.Bold = true;
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Fill.BackgroundColor = XLColor.Gray;
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Font.FontColor = XLColor.White;
            minorTrafficNatureRow++;

            // **Get Minor Traffic Offenses Grouped by Nature**
            var minorTrafficViolationsByNature = _context.TrafficReportsEncoded
                .Where(r => r.CommissionDate >= DateOnly.FromDateTime(startDate) &&
                            r.CommissionDate <= DateOnly.FromDateTime(endDateTime) &&
                            r.Offense.Classification == OffenseClassification.MinorTraffic)
                .GroupBy(v => v.Offense.Nature)
                .Select(g => new
                {
                    Nature = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(v => v.Count)
                .ToList();

            // **Write Data (Nature of Offense & Total Cases)**
            foreach (var record in minorTrafficViolationsByNature)
            {
                minorTrafficByNatureSheet.Cell(minorTrafficNatureRow, 1).Value = record.Nature;
                minorTrafficByNatureSheet.Cell(minorTrafficNatureRow, 2).Value = record.Count;
                minorTrafficNatureRow++;
            }

            // **Add a Space Before Descriptions**
            minorTrafficNatureRow += 1;

            // **Section Header: "STATISTICS OF MINOR TRAFFIC OFFENSES COMMITTED BY STUDENTS"**
            minorTrafficByNatureSheet.Cell(minorTrafficNatureRow, 1).Value = "STATISTICS OF MINOR TRAFFIC OFFENSES COMMITTED BY STUDENTS";
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Font.Bold = true;
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Font.FontSize = 12;
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Font.FontColor = XLColor.Red;
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Fill.BackgroundColor = XLColor.White;
            minorTrafficByNatureSheet.Range(minorTrafficNatureRow, 1, minorTrafficNatureRow, 2).Merge();
            minorTrafficNatureRow += 2;

            // **Headers for Description Section**
            minorTrafficByNatureSheet.Cell(minorTrafficNatureRow, 1).Value = "Description";
            minorTrafficByNatureSheet.Cell(minorTrafficNatureRow, 2).Value = "Frequency";
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Font.Bold = true;
            minorTrafficByNatureSheet.Row(minorTrafficNatureRow).Style.Fill.BackgroundColor = XLColor.LightGray;
            minorTrafficNatureRow++;

            // **Get Unique Descriptions with Count**
            var minorTrafficDescriptions = _context.TrafficReportsEncoded
                .Where(r => r.CommissionDate >= DateOnly.FromDateTime(startDate) &&
                            r.CommissionDate <= DateOnly.FromDateTime(endDateTime) &&
                            r.Offense.Classification == OffenseClassification.MinorTraffic)
                .Where(r => !string.IsNullOrEmpty(r.Remarks)) // Exclude empty descriptions
                .GroupBy(r => r.Remarks)
                .Select(g => new { Description = g.Key, Count = g.Count() })
                .OrderByDescending(d => d.Count)
                .ToList();

            // **Write Unique Descriptions & Their Counts**
            foreach (var record in minorTrafficDescriptions)
            {
                minorTrafficByNatureSheet.Cell(minorTrafficNatureRow, 1).Value = record.Description;
                minorTrafficByNatureSheet.Cell(minorTrafficNatureRow, 2).Value = record.Count;
                minorTrafficNatureRow++;
            }

            // **Adjust Column Width for Readability**
            minorTrafficByNatureSheet.Columns().AdjustToContents();

            var majorTrafficByNatureSheet = workbook.Worksheets.Add("Major Traffic Offense by Nature");

            // Title: "Major Traffic Offenses by Nature"
            int majorTrafficNatureRow = 1;
            majorTrafficByNatureSheet.Cell(majorTrafficNatureRow, 1).Value = "Major Traffic Offenses by Nature";
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Font.Bold = true;
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Font.FontSize = 14;
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            majorTrafficByNatureSheet.Range(majorTrafficNatureRow, 1, majorTrafficNatureRow, 2).Merge();
            majorTrafficNatureRow += 2;

            majorTrafficByNatureSheet.Cell(majorTrafficNatureRow, 1).Value = $"{startDate:MMMM yyyy} - {endDateTime:MMMM yyyy}";
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Font.Bold = true;
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Font.FontSize = 12;
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            majorTrafficByNatureSheet.Range(majorTrafficNatureRow, 1, majorTrafficNatureRow, 2).Merge();
            majorTrafficNatureRow += 2;

            // Headers
            majorTrafficByNatureSheet.Cell(majorTrafficNatureRow, 1).Value = "Nature of Offense";
            majorTrafficByNatureSheet.Cell(majorTrafficNatureRow, 2).Value = "Total Cases";
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Font.Bold = true;
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Fill.BackgroundColor = XLColor.Gray;
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Font.FontColor = XLColor.White;
            majorTrafficNatureRow++;

            // **Get Major Traffic Offenses Grouped by Nature**
            var majorTrafficViolationsByNature = _context.TrafficReportsEncoded
                .Where(r => r.CommissionDate >= DateOnly.FromDateTime(startDate) &&
                            r.CommissionDate <= DateOnly.FromDateTime(endDateTime) &&
                            r.Offense.Classification == OffenseClassification.MajorTraffic)
                .GroupBy(v => v.Offense.Nature)
                .Select(g => new
                {
                    Nature = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(v => v.Count)
                .ToList();

            // **Write Data (Nature of Offense & Total Cases)**
            foreach (var record in majorTrafficViolationsByNature)
            {
                majorTrafficByNatureSheet.Cell(majorTrafficNatureRow, 1).Value = record.Nature;
                majorTrafficByNatureSheet.Cell(majorTrafficNatureRow, 2).Value = record.Count;
                majorTrafficNatureRow++;
            }

            // **Add a Space Before Descriptions**
            majorTrafficNatureRow += 1;

            // **Section Header: "STATISTICS OF MAJOR TRAFFIC OFFENSES COMMITTED BY STUDENTS"**
            majorTrafficByNatureSheet.Cell(majorTrafficNatureRow, 1).Value = "STATISTICS OF MAJOR TRAFFIC OFFENSES COMMITTED BY STUDENTS";
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Font.Bold = true;
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Font.FontSize = 12;
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Font.FontColor = XLColor.Red;
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Fill.BackgroundColor = XLColor.White;
            majorTrafficByNatureSheet.Range(majorTrafficNatureRow, 1, majorTrafficNatureRow, 2).Merge();
            majorTrafficNatureRow += 2;

            // **Headers for Description Section**
            majorTrafficByNatureSheet.Cell(majorTrafficNatureRow, 1).Value = "Description";
            majorTrafficByNatureSheet.Cell(majorTrafficNatureRow, 2).Value = "Frequency";
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Font.Bold = true;
            majorTrafficByNatureSheet.Row(majorTrafficNatureRow).Style.Fill.BackgroundColor = XLColor.LightGray;
            majorTrafficNatureRow++;

            // **Get Unique Descriptions with Count**
            var majorTrafficDescriptions = _context.TrafficReportsEncoded
                .Where(r => r.CommissionDate >= DateOnly.FromDateTime(startDate) &&
                            r.CommissionDate <= DateOnly.FromDateTime(endDateTime) &&
                            r.Offense.Classification == OffenseClassification.MajorTraffic)
                .Where(r => !string.IsNullOrEmpty(r.Remarks)) // Exclude empty descriptions
                .GroupBy(r => r.Remarks)
                .Select(g => new { Description = g.Key, Count = g.Count() })
                .OrderByDescending(d => d.Count)
                .ToList();

            // **Write Unique Descriptions & Their Counts**
            foreach (var record in majorTrafficDescriptions)
            {
                majorTrafficByNatureSheet.Cell(majorTrafficNatureRow, 1).Value = record.Description;
                majorTrafficByNatureSheet.Cell(majorTrafficNatureRow, 2).Value = record.Count;
                majorTrafficNatureRow++;
            }

            // **Adjust Column Width for Readability**
            majorTrafficByNatureSheet.Columns().AdjustToContents();



            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{fileName}.xlsx");
        }

        public class ViolationRequest
        {
            public string ViolationType { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }
    }
}
