using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.ViewModels;

namespace WSRS_SWAFO.Controllers
{
    public class EncodeController : Controller
    {
        private readonly ApplicationDbContext _student;
        private StudentsRecordModel _studentData = new StudentsRecordModel();

        public IActionResult SetStudentData(int StudentNumber, string FirstName, string LastName)
        {
            TempData["StudentNumber"] = StudentNumber;
            TempData["FirstName"] = FirstName;
            TempData["LastName"] = LastName;
            return NoContent();
        }

        public StudentsRecordModel GetStudentData() {  return _studentData; }

        public EncodeController(ApplicationDbContext student)
        {
            _student = student;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _student.Students
                .Select(s => new StudentsRecordModel
                {
                    StudentNumber = s.StudentNumber,
                    LastName = s.LastName,
                    FirstName = s.FirstName
                }).Take(5).ToListAsync();
            var viewModel = new CreateStudentViewModel
            {
                ExistingStudents = students.AsQueryable()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Search(string searchStudent)
        {
                var existingDataViewModel = new CreateStudentViewModel();
            if (!string.IsNullOrEmpty(searchStudent))
            {
                var studentsQuery = _student.Students.AsQueryable();
                if (int.TryParse(searchStudent, out int studentNumber))
                {
                    studentsQuery = studentsQuery.Where(s => s.StudentNumber == studentNumber);
                }
                else
                {
                    studentsQuery = studentsQuery.Where(s => s.LastName.Contains(searchStudent) || s.FirstName.Contains(searchStudent));
                }
                existingDataViewModel.ExistingStudents = studentsQuery.Select(s => new StudentsRecordModel
                {
                    StudentNumber = s.StudentNumber,
                    LastName = s.LastName,
                    FirstName = s.FirstName
                });
            }
            return View("Index", existingDataViewModel);
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> CheckStudentID(CreateStudentViewModel model)
        {
            var exists = await _student.Students.AnyAsync(student => student.StudentNumber == model.NewStudent.StudentNumber);
            return Json(!exists); 
        }


        [HttpPost]
        public IActionResult CreateNewStudent(CreateStudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var student = new Student
            {
                StudentNumber = model.NewStudent.StudentNumber,
                FirstName = model.NewStudent.FirstName,
                LastName = model.NewStudent.LastName
            };

            _student.Students.Add(student);
            _student.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Pending()
        {
            return View();
        }

        public IActionResult TrafficViolation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateCollege()
        {
            var colleges = _student.College.ToList(); // Fetch all colleges
            ViewBag.College = colleges; // Pass the list of colleges to the view
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateCollege(string collegeName)
        {
            if (ModelState.IsValid)
            {
                // Check if the collegeName already exists to prevent duplicate entries
                var existingCollege = _student.College.FirstOrDefault(c => c.CollegeID == collegeName);
                if (existingCollege == null)
                {
                    // Create new college with the given name and set the CollegeID to the same value
                    var newCollege = new College
                    {
                        CollegeID = collegeName // Use collegeName as CollegeID
                    };

                    _student.College.Add(newCollege);
                    _student.SaveChanges(); // Save changes to the database

                    TempData["SuccessMessage"] = "College added into the database successfully."; // Success message
                }
                else
                {
                    TempData["ErrorMessage"] = "College ID already exists."; // Error message for duplicate CollegeID
                }

                return RedirectToAction("CreateCollege"); // Redirect to refresh the page and display success/error
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCollege(string collegeID)
        {
            var college = _student.College.FirstOrDefault(c => c.CollegeID == collegeID);
            if (college != null)
            {
                _student.College.Remove(college); // Remove the college
                _student.SaveChanges();           // Save changes
                TempData["SuccessMessage"] = "College deleted successfully.";
            }
            return RedirectToAction("CreateCollege"); // Redirect to refresh the page
        }
    }
}
