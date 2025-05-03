using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using WSRS_SWAFO.Data;
using WSRS_SWAFO.Data.Enum;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.ViewModels;
using WSRS_SWAFO.Interfaces;
using Hangfire;
using WSRS_SWAFO.Dtos;

namespace WSRS_SWAFO.Controllers
{
    [Authorize(Roles = "AppRole.Admin")]
    public class EncodeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EncodeController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IHttpClientFactory _httpClientFactory;

        public EncodeController(ApplicationDbContext context, ILogger<EncodeController> logger, IEmailSender emailSender, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _emailSender = emailSender;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(EncodingMode));
        }

        // Mode Switch
        // Ticks whether Student Violation or Traffic Violation
        public IActionResult EncodingMode()
        {
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return Content("<script>alert('External links are disabled. Use the in-app interface to proceed.'); window.history.back();</script>", "text/html");
            }
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

            TempData["ViolationType"] = violationType; // for pill indicator
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
        [HttpGet]
        public IActionResult CreateStudentRecord(
            int? studentNumber,
            string? firstName,
            string? lastName)
        {
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction(nameof(StudentRecordViolation));
            }

            if (HttpContext.Session.GetString("ViolationType") is string violationType &&
                violationType == "Traffic Violation")
            {
                TempData["FromTraffic"] = true;
            }

            return View();
        }

        // Create Student Entry - POST Function
        [HttpPost]
        public async Task<IActionResult> CreateNewStudent(StudentRecordViewModel model, bool fromPending = false, bool fromTraffic = false)
        {
            if (!ModelState.IsValid)
            {
                SetToastMessage(message: "Please fill in the required fields appropriately.");
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

            if (fromPending)
            {
                return RedirectToAction(nameof(Pending));
            }

            var redirect = fromTraffic ? nameof(EncodeTrafficViolation) : nameof(EncodeStudentViolation);

            return RedirectToAction(redirect, new
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
            [FromQuery] string lastName,
            [FromQuery] string? course = null,
            [FromQuery] string? collegeId = null,
            [FromQuery] string? note = null)
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
                LastName = lastName,
                Course = course,
                CollegeID = collegeId,
                Note = note,
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
                SetToastMessage(message: "Please fill in the required fields appropriately.");
                ViewBag.Colleges = _context.College.ToList();

                return View(reportEncodedVM);
            }

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

            var student = _context.Students
                        .Where(s => s.StudentNumber == reportEncodedVM.StudentNumber)
                        .FirstOrDefault();

            try
            {
                _context.ReportsEncoded.Add(studentReport);
                await _context.SaveChangesAsync();

                var emailSubjectVM = new EmailSubjectViewModel
                {
                    email = student!.Email,
                    emailMode = 0,
                    id = _context.ReportsEncoded.Last().Id,
                    name = student!.FirstName + " " + student!.LastName,
                    sanction = reportEncodedVM.Sanction
                };
                
                BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(emailSubjectVM));
                SetToastMessage(title: "Success", message: "A report has been encoded successfully.");

                return RedirectToAction(nameof(EncodingMode));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save data: " + ex.Message);
                SetToastMessage(title: "Error", message: "Something went wrong encoding your data.", cssClassName: "bg-danger text-white");
                _logger.LogError(ex.Message);

            }

            return RedirectToAction(nameof(EncodeStudentViolation), new
            {
                studentNumber = reportEncodedVM.StudentNumber,
                firstName = reportEncodedVM.FirstName,
                lastName = reportEncodedVM.LastName
            });
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

            if (HttpContext.Session.GetString("ViolationType") is string violationType &&
                violationType == "Traffic Violation")
            {
                TempData["FromTraffic"] = true;
            }

            return View(studentInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EncodeTrafficViolation(TrafficReportEncodedViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                SetToastMessage(message: "Please fill in the required fields appropriately.");
                ViewBag.Colleges = _context.College.ToList();

                return View(viewModel);
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

            var student = _context.Students
                        .Where(s => s.StudentNumber == studentTrafficReport.StudentNumber)
                        .FirstOrDefault();

            try
            {
                _context.TrafficReportsEncoded.Add(studentTrafficReport);
                await _context.SaveChangesAsync();

                var emailSubjectVM = new EmailSubjectViewModel
                {
                    email = student!.Email,
                    emailMode = 0,
                    id = _context.TrafficReportsEncoded.Last().Id,
                    name = student!.FirstName + " " + student!.LastName,
                    sanction = viewModel.Sanction
                };

                BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(emailSubjectVM));
                SetToastMessage(title: "Success", message: "A traffic report has been encoded successfully.");

                return RedirectToAction(nameof(EncodingMode));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save data: " + ex.Message);
                SetToastMessage(title: "Error", message: "Something went wrong encoding your data.", cssClassName: "bg-danger text-white");
                _logger.LogError(ex.Message);
            }

            return RedirectToAction(nameof(EncodeTrafficViolation), new
            {
                studentNumber = viewModel.StudentNumber,
                firstName = viewModel.FirstName,
                lastName = viewModel.LastName
            });
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

        [HttpGet]
        public async Task<IActionResult> Pending()
        {
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction(nameof(StudentRecordViolation));
            }

            var client = _httpClientFactory.CreateClient("WSRS-Api");

            var response = await client.GetAsync("report");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<List<ReportPendingDto>>();

                if (data != null)
                {
                    var activeReports = data.Where(r => !r.IsArchived);

                    var pendingVM = new PendingViewModel
                    {
                        ReportsPending = activeReports
                    };

                    HttpContext.Session.SetString("ViolationType", "Student Violation");
                    return View(pendingVM);
                }
            }
            else
            {
                _logger.LogError($"Something went wrong getting report pending data with status code: {response.StatusCode}");

            }

            return View();
        }

        [HttpPost("[controller]/archive/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchivePending(int id)
        {
            var client = _httpClientFactory.CreateClient("WSRS-Api");

            try
            {
                // patch document
                var archivePatch = new[]
                {
                    new
                    {
                        op = "replace", path = "/isArchived", value = "true"
                    }
                };

                var jsonPatch = JsonSerializer.Serialize(archivePatch);

                var content = new StringContent(jsonPatch, Encoding.UTF8, "application/json-patch+json");
                var request = new HttpRequestMessage(HttpMethod.Patch, $"report/{id}")
                {
                    Content = content
                };

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    SetToastMessage(
                        message: "A pending report has been archived.", 
                        button: new ToastButton
                        {
                            Name = "Undo", 
                            Action = "UndoArchivePending",
                            Controller = "Encode",
                            RouteValues = new Dictionary<string, object>
                            {
                                { "id", id }
                            }
                        });
                    return RedirectToAction(nameof(Pending));
                }
                else
                {
                    SetToastMessage(title: "Error", message: "Failed to archive the report.", cssClassName: "bg-danger text-white");
                    _logger.LogError($"PATCH failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                SetToastMessage(title: "Error", message: "Something went wrong with your message", cssClassName: "bg-danger text-white");
                _logger.LogError(ex.Message);
            }

            return RedirectToAction(nameof(Pending));
        }

        [HttpPost("[controller]/undo-archive/")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoArchivePending(int id)
        {
            var client = _httpClientFactory.CreateClient("WSRS-Api");

            try
            {
                // patch document
                var archivePatch = new[]
                {
                    new
                    {
                        op = "replace", path = "/isArchived", value = "false"
                    }
                };

                var jsonPatch = JsonSerializer.Serialize(archivePatch);

                var content = new StringContent(jsonPatch, Encoding.UTF8, "application/json-patch+json");
                var request = new HttpRequestMessage(HttpMethod.Patch, $"report/{id}")
                {
                    Content = content
                };

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    SetToastMessage("An action has been undone.");
                    return RedirectToAction(nameof(Pending));
                }
                else
                {
                    SetToastMessage(title: "Error", message: "Failed to undo your action.", cssClassName: "bg-danger text-white");
                    _logger.LogError($"PATCH failed with status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                SetToastMessage(title: "Error", message: "Something went wrong with your message", cssClassName: "bg-danger text-white");
                _logger.LogError(ex.Message);
            }

            return RedirectToAction(nameof(Pending));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EncodeFromPending(ReportPendingDto report)
        {
            // Check if student exists
            var student = _context.Students.Find(report.StudentNumber);

            if (student != null)
            {
                var studentInfo = new ReportEncodedViewModel
                {
                    StudentNumber = report.StudentNumber,
                    FirstName = report.FirstName,
                    LastName = report.LastName,
                    CollegeID = report.College,
                    Formator = report.Formator,
                    Course = report.CourseYearSection,
                    Note = report.Description
                };

                return RedirectToAction(nameof(EncodeStudentViolation), studentInfo);
            }
            else
            {
                var studentInfo = new StudentRecordViewModel
                {
                    StudentNumber = report.StudentNumber,
                    FirstName = report.FirstName,
                    LastName = report.LastName,
                };

                TempData["FromPending"] = true;
                SetToastMessage("The student reported does not exist yet. Create their record first.");
                return RedirectToAction(nameof(CreateStudentRecord), studentInfo);
            }
        }

        [HttpGet]
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

        private void SetToastMessage(string message, string title = "", string cssClassName = "bg-white", ToastButton? button = null)
        {
            var toastMessage = new ToastViewModel
            {
                Title = title,
                Message = message,
                CssClassName = cssClassName,
                Button = button
            };
            TempData["Result"] = JsonSerializer.Serialize(toastMessage);
        }
    }
}
