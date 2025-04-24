using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSRS_Formators.Models;
using WSRS_Student.Data;

namespace WSRS_Formators.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AzureDbContext _context;

        public HomeController(AzureDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult WSRSEmp()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> CreateReport(int? StudentNumber = null)
        {
            var report = new ReportViolation();

            if (StudentNumber.HasValue)
            {
                var student = await _context.Students.FindAsync(StudentNumber.Value);
                if (student != null)
                {
                    report.StudentNumber = student.StudentNumber;
                    report.StudentName = $"{student.FirstName} {student.LastName}";
                    report.College = " ";  // Set the college if needed
                    report.CommissionDate = DateOnly.FromDateTime(DateTime.Today);  // Set today's date as CommissionDate

                    // Indicate that the student was found
                    ViewBag.StudentFound = true;
                }
                else
                {
                    // Indicate that no student was found
                    ViewBag.StudentFound = false;
                    // In case no student is found, we clear the student number and make fields editable
                    report.CommissionDate = DateOnly.FromDateTime(DateTime.Today);  // Set today's date for CommissionDate
                }
            }
            else
            {
                // If no student number is provided, create a new empty report and set the CommissionDate
                ViewBag.StudentFound = false;
                report.CommissionDate = DateOnly.FromDateTime(DateTime.Today);
            }

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReport(ReportViolation report)
        {
            if (ModelState.IsValid)
            {
                _context.ReportsViolation.Add(report);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(report);
        }
        [HttpPost]
        public async Task<IActionResult> WSRSEmp(int studentNumber)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentNumber == studentNumber);

            if (student == null)
            {
                ViewBag.NotFound = true;
                return View(new StudentRecordViewModel());
            }

            var viewModel = new StudentRecordViewModel
            {
                Student = student,
                ReportsEncoded = _context.ReportsEncoded
                    .Where(r => r.StudentNumber == studentNumber)
                    .Include(r => r.Offense)
                    .ToList(),
                TrafficReportsEncoded = _context.TrafficReportsEncoded
                    .Where(t => t.StudentNumber == studentNumber)
                    .Include(t => t.Offense)
                    .ToList()
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Index(int studentNumber)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentNumber == studentNumber);

            if (student == null)
            {
                ViewBag.NotFound = true;
                return View();
            }

            var reports = await _context.ReportsEncoded
                .Include(r => r.Offense)
                .Where(r => r.StudentNumber == studentNumber)
                .ToListAsync();

            var trafficReports = await _context.TrafficReportsEncoded
                .Include(t => t.Offense)
                .Where(t => t.StudentNumber == studentNumber)
                .ToListAsync();

            var model = new StudentRecordViewModel
            {
                Student = student,
                ReportsEncoded = reports,
                TrafficReportsEncoded = trafficReports
            };

            return View(model);
        }
    }
}
