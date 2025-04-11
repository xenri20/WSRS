using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSRS_Formators.Data;
using WSRS_Formators.Models;

namespace WSRS_Formators.Controllers
{
    public class HomeController : Controller
    {
        private readonly EmployeeDbContext _context;

        public HomeController(EmployeeDbContext context)
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
        public IActionResult CreateReport()
        {
            return View();
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
        public IActionResult WSRSEmp(int studentNumber)
        {
            var student = _context.Students.FirstOrDefault(s => s.StudentNumber == studentNumber);

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
        public IActionResult Index(int studentNumber)
        {
             var student = _context.Students.FirstOrDefault(s => s.StudentNumber == studentNumber);

            if (student == null)
            {
                ViewBag.NotFound = true;
                return View();
            }

            var reports = _context.ReportsEncoded
                .Include(r => r.Offense)
                .Where(r => r.StudentNumber == studentNumber)
                .ToList();

            var trafficReports = _context.TrafficReportsEncoded
                .Include(t => t.Offense)
                .Where(t => t.StudentNumber == studentNumber)
                .ToList();

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
