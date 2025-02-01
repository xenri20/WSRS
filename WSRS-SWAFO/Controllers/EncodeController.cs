using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSRS_SWAFO.Data;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.ViewModels;

namespace WSRS_SWAFO.Controllers
{
    public class EncodeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EncodeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mode Switch
        // Ticks whether Student Violation or Traffic Violation
        public IActionResult EncodingMode()
        {
            return View();
        }

        // Page One - Student Violation Controller
        // Checks Student Record Database upon page load - Index
        [HttpPost]
        [HttpGet] // Adding for optional 
        public async Task<IActionResult> StudentRecordViolation(string violationType = null)
        {
            if (HttpContext.Request.Method == HttpMethods.Post)
            {
                // Store the violation type in session or view data if needed
                HttpContext.Session.SetString("ViolationType", violationType);
            }

            // Checks student data in the Database
            var students = await _context.Students
                .Select(student => new StudentRecordViewModel
                {
                    // For every student in the Database, compiler creates the query
                    StudentNumber = student.StudentNumber,
                    LastName = student.LastName,
                    FirstName = student.FirstName
                })
                .Take(5)
                .ToListAsync();

            // Returns queried list
            return View(students.AsQueryable());
        }

        // Search Student - GET Function
        [HttpGet]
        public IActionResult Search(string searchStudent)
        {
            // Checks searchStudent
            if (!string.IsNullOrEmpty(searchStudent))
            {
                var studentsQuery = _context.Students.AsQueryable();
                // Compiler creates query for searchStudent
                if (int.TryParse(searchStudent, out int studentNumber))
                {
                    studentsQuery = studentsQuery.Where(s => s.StudentNumber == studentNumber);
                }
                else
                {
                    studentsQuery = studentsQuery.Where(s => s.LastName.Contains(searchStudent) || s.FirstName.Contains(searchStudent));
                }
                // Once existed, compiler creates table query
                var ExistingStudent = studentsQuery.Select(s => new StudentRecordViewModel
                {
                    StudentNumber = s.StudentNumber,
                    LastName = s.LastName,
                    FirstName = s.FirstName
                });
                return View("StudentRecordViolation", ExistingStudent);
            }
            return View("StudentRecordViolation", null);
        }

        // Page 2 - Create Student Data (If no student present) - Index
        public IActionResult CreateStudentRecord()
        {
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction("StudentRecordViolation");
            }

            return View();
        }

        // Create Student Entry - POST Function
        [HttpPost]
        public IActionResult CreateNewStudent(StudentRecordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateStudentRecord", model);
            }

            var student = new Student
            {
                StudentNumber = model.StudentNumber,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            _context.Students.Add(student);
            _context.SaveChanges();

            return RedirectToAction("EncodeStudentViolation", new
            {
                studentNumber = student.StudentNumber,
                firstName = student.FirstName,
                lastName = student.LastName
            });
        }

        // Checks whether studentID is existing or not - POST, GET Function
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> CheckStudentID(StudentRecordViewModel model)
        {
            var exists = await _context.Students.AnyAsync(student => student.StudentNumber == model.StudentNumber);
            return Json(!exists); 
        }

        public IActionResult RedirectToView(int studentNumber, string firstName, string lastName)
        {
            var violationType = HttpContext.Session.GetString("ViolationType");

            if(string.IsNullOrEmpty(violationType))
            {
                return RedirectToAction("EncodeMode"); 
            }

            if (violationType == "Student Violation")
            {
                return RedirectToAction("EncodeStudentViolation", new
                {
                    studentNumber = studentNumber,
                    firstName = firstName,
                    lastName = lastName
                });
            }

            if (violationType == "Traffic Violation")
            {
                return RedirectToAction();
            }

            // Redirect to first page if no mode is selected
            return RedirectToAction("EncodeMode"); 
        }

        // Page 3 - Encode Student Violation (If student data exist)
        public IActionResult EncodeStudentViolation(int studentNumber, string firstName, string lastName)
        {
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction("StudentRecordViolation");
            }
            var studentInfo = new ReportEncodedViewModel
            {
                StudentNumber = studentNumber,
                Student = new Student
                {
                    StudentNumber = studentNumber, 
                    FirstName = firstName,
                    LastName = lastName
                }
            };

            ViewBag.Colleges = _context.College.ToList();
            return View(studentInfo);
        }

        // Creates a violation record - POST Function
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReport(ReportEncoded reportEncoded)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    _context.ReportsEncoded.Add(reportEncoded);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("StudentRecordViolation");  
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Unable to save data: " + ex.Message);
                }
            }
            return View(reportEncoded); 
        }

        // Returns offense Nature to JSON - GET Function
        [HttpGet]
        public IActionResult GetOffenseNature(int classification)
        {
            var offenseNature = _context.Offenses
                .Where(offense => (int)offense.Classification == classification)
                .Select(nature => new { nature.Id, nature.Nature })
                .ToList();

            return Json(offenseNature);
        }

        public IActionResult Pending()
        {
            return View();
        }

        public IActionResult TrafficViolation()
        {
            return View();
        }
    }
}
