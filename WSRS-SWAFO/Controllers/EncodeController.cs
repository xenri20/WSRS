using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WSRS_SWAFO.Data;
using WSRS_SWAFO.Data.Enum;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.ViewModels;
using WSRS_SWAFO.Interfaces;
using System.Reflection;

namespace WSRS_SWAFO.Controllers
{
    [Authorize]
    public class EncodeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EncodeController> _logger;
        private readonly IEmailSender _emailSender;

        public EncodeController(ApplicationDbContext context, ILogger<EncodeController> logger, IEmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(EncodingMode));
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
                .AsNoTracking()
                .Take(5)
                .ToListAsync();

            ViewData["ViolationType"] = violationType == "Student Violation" ? "Regular" : "Traffic";

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
                return View(nameof(StudentRecordViolation), ExistingStudent);
            }
            return View(nameof(StudentRecordViolation), null);
        }

        // Page 2 - Create Student Data (If no student present) - Index
        public IActionResult CreateStudentRecord()
        {
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction(nameof(StudentRecordViolation));
            }

            return View();
        }

        // Create Student Entry - POST Function
        [HttpPost]
        public async Task<IActionResult> CreateNewStudent(StudentRecordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                SetToastMessage(message: "Please fill in the required fields.");
                return View(nameof(CreateStudentRecord), model);
            }

            var student = new Student
            {
                StudentNumber = model.StudentNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email.ToLower()
            };

            try
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                SetToastMessage(title: "Success", message: "A student was registered successfully.");
            }
            catch (Exception ex)
            {
                SetToastMessage(title: "Error", message: "Something went wrong while submitting your data.", cssClassName: "bg-danger text-white");
                _logger.LogError(ex.Message);
            }

            try
            {
                await _emailSender.SendEmailAsync("swafomatortest@outlook.com", "You have been violated!", "This is just a test");
                Console.WriteLine("Email Sent Successfully~");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email send failed: {ex.Message}");
            }

            return RedirectToAction(nameof(EncodeStudentViolation), new
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

        public IActionResult RedirectToView(
            [FromQuery] int studentNumber,
            [FromQuery] string firstName,
            [FromQuery] string lastName)
        {
            var violationType = HttpContext.Session.GetString("ViolationType");

            if (string.IsNullOrEmpty(violationType))
            {
                return RedirectToAction(nameof(EncodingMode));
            }

            if (violationType == "Student Violation")
            {
                return RedirectToAction(nameof(EncodeStudentViolation), new
                {
                    studentNumber,
                    firstName,
                    lastName
                });
            }

            if (violationType == "Traffic Violation")
            {
                return RedirectToAction(nameof(EncodeTrafficViolation), new
                {
                    studentNumber,
                    firstName,
                    lastName
                });
            }

            // Redirect to first page if no mode is selected
            return RedirectToAction(nameof(EncodingMode));
        }

        // Page 3 - Encode Student Violation (If student data exist)
        public IActionResult EncodeStudentViolation(
            [FromQuery] int studentNumber,
            [FromQuery] string firstName,
            [FromQuery] string lastName)
        {
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction(nameof(StudentRecordViolation));
            }

            var studentInfo = new ReportEncodedViewModel
            {
                StudentNumber = studentNumber,
                FirstName = firstName,
                LastName = lastName
            };

            ViewBag.Colleges = _context.College.ToList();

            return View(studentInfo);
        }

        // Creates a violation record - POST Function
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EncodeStudentViolation(ReportEncodedViewModel reportEncodedVM)
        {
            if (!ModelState.IsValid)
            {
                SetToastMessage(message: "Please fill in the required fields.");

                return RedirectToAction(nameof(EncodeStudentViolation), new
                {
                    studentNumber = reportEncodedVM.StudentNumber,
                    firstName = reportEncodedVM.FirstName,
                    lastName = reportEncodedVM.LastName
                });
            }

            try
            {
                var studentReport = new ReportEncoded
                {
                    OffenseId = reportEncodedVM.OffenseId,
                    StudentNumber = reportEncodedVM.StudentNumber,
                    CollegeID = reportEncodedVM.CollegeID,
                    CommissionDate = reportEncodedVM.CommissionDate,
                    // FormatorId = reportEncodedVM.FormatorId,
                    Course = reportEncodedVM.Course,
                    Description = reportEncodedVM.Description,
                    Sanction = reportEncodedVM.Sanction,
                    StatusOfSanction = reportEncodedVM.StatusOfSanction
                };

                _context.ReportsEncoded.Add(studentReport);
                await _context.SaveChangesAsync();

                SetToastMessage(title: "Success", message: "A report has been encoded successfully.");

                return RedirectToAction(nameof(StudentRecordViolation));
            }
            catch (Exception ex)
            {
                SetToastMessage(title: "Error", message: "Something went wrong encoding your data.", cssClassName: "bg-danger text-white");
                _logger.LogError(ex.Message);

                return RedirectToAction(nameof(EncodeStudentViolation), new
                {
                    studentNumber = reportEncodedVM.StudentNumber,
                    firstName = reportEncodedVM.FirstName,
                    lastName = reportEncodedVM.LastName
                });
            }
        }

        [HttpGet]
        public IActionResult EncodeTrafficViolation(
            [FromQuery] int studentNumber,
            [FromQuery] string firstName,
            [FromQuery] string lastName)
        {
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction(nameof(StudentRecordViolation));
            }

            var studentInfo = new TrafficReportEncodedViewModel
            {
                StudentNumber = studentNumber,
                FirstName = firstName,
                LastName = lastName
            };

            ViewBag.Colleges = _context.College.ToList();

            return View(studentInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EncodeTrafficViolation(TrafficReportEncodedViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                SetToastMessage(message: "Please fill in the required fields.");
                
                return RedirectToAction(nameof(EncodeTrafficViolation), new
                {
                    studentNumber = viewModel.StudentNumber,
                    firstName = viewModel.FirstName,
                    lastName = viewModel.LastName
                });
            }

            var studentTrafficReport = new TrafficReportsEncoded
            {
                OffenseId = viewModel.OffenseId,
                StudentNumber = viewModel.StudentNumber,
                CollegeID = viewModel.CollegeID,
                CommissionDate = viewModel.CommissionDate,
                PlateNumber = viewModel.PlateNumber,
                Place = viewModel.Place,
                Remarks = viewModel.Remarks,
                ORNumber = viewModel.ORNumber,
                DatePaid = viewModel.DatePaid
            };

            try
            {
                _context.TrafficReportsEncoded.Add(studentTrafficReport);
                await _context.SaveChangesAsync();

                SetToastMessage(title: "Success", message: "A traffic report has been encoded successfully.");

                return RedirectToAction(nameof(StudentRecordViolation));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save data: " + ex.Message);
                SetToastMessage(title: "Error", message: "Something went wrong encoding your data.", cssClassName: "bg-danger text-white");
                _logger.LogError(ex.Message);
            }

            return View(studentTrafficReport);
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

        public IActionResult CreateOffense()
        {
            // Retrieve all offenses and sort them by Classification
            ViewBag.Offenses = _context.Offenses
                .OrderBy(o => o.Classification)
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateOffense(OffenseClassification classification, string nature)
        {
            if (string.IsNullOrWhiteSpace(nature))
            {
                TempData["ErrorMessage"] = "Offense nature cannot be empty.";
            }
            else if (nature.Length > 50)
            {
                TempData["ErrorMessage"] = "Offense nature cannot exceed 50 characters.";
            }
            else
            {
                try
                {
                    // Find the last ID in the database and increment it
                    int nextId = _context.Offenses.Any()
                        ? _context.Offenses.Max(o => o.Id) + 1
                        : 1;

                    var offense = new Offense
                    {
                        Id = nextId, // Assign the next available ID
                        Classification = classification,
                        Nature = nature
                    };

                    _context.Offenses.Add(offense);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = $"Offense added successfully with ID {nextId}.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error adding offense: {ex.Message}";
                }
            }

            ViewBag.Offenses = _context.Offenses.ToList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteOffense(int id)
        {
            var offense = _context.Offenses.FirstOrDefault(o => o.Id == id);
            if (offense != null)
            {
                try
                {
                    _context.Offenses.Remove(offense);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Offense deleted successfully.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error deleting offense: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Offense not found.";
            }

            return RedirectToAction(nameof(CreateOffense));
        }

        public IActionResult CreateCollege()
        {
            ViewBag.College = _context.College.ToList();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCollege(string collegeID)
        {
            if (string.IsNullOrWhiteSpace(collegeID))
            {
                TempData["ErrorMessage"] = "College name cannot be empty.";
            }
            else if (_context.College.Any(c => c.CollegeID == collegeID))
            {
                TempData["ErrorMessage"] = $"The college '{collegeID}' already exists.";
            }
            else
            {
                try
                {
                    var college = new College
                    {
                        CollegeID = collegeID
                    };

                    _context.College.Add(college);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = $"The college '{collegeID}' has been added successfully.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error adding college: {ex.Message}";
                }
            }

            ViewBag.College = _context.College.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult DeleteCollege(string collegeID)
        {
            var college = _context.College.FirstOrDefault(c => c.CollegeID == collegeID);
            if (college != null)
            {
                try
                {
                    _context.College.Remove(college);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = $"The college '{collegeID}' has been deleted successfully.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error deleting college: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = $"The college '{collegeID}' was not found.";
            }


            return RedirectToAction(nameof(CreateCollege));
        }

        private void SetToastMessage(string message, string title = "", string cssClassName = "bg-white")
        {
            var toastMessage = new ToastViewModel
            {
                Title = title,
                Message = message,
                CssClassName = cssClassName
            };
            TempData["Result"] = JsonSerializer.Serialize(toastMessage);
        }
    }
}
