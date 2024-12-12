using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.ViewModels;

namespace WSRS_SWAFO.Controllers
{
    public class EncodeController : Controller
    {
        private readonly ApplicationDbContext _student;

        public EncodeController(ApplicationDbContext student)
        {
            _student = student;
        }

        // Page One - Student Violation Controller
        // Checks Student Record Database upon page load - Index
        public async Task<IActionResult> StudentRecordViolation()
        {
            // Checks student data in the Database
            var students = await _student.Students
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
                var studentsQuery = _student.Students.AsQueryable();
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
            if (ModelState.IsValid) {
                return View("CreateStudentRecord", model);
            }

            var student = new Student
            {
                StudentNumber = model.StudentNumber,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            _student.Students.Add(student);
            _student.SaveChanges();

            return RedirectToAction("EncodeStudentViolation");
        }

        // Checks whether studentID is existing or not - POST, GET Function
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> CheckStudentID(StudentRecordViewModel model)
        {
            var exists = await _student.Students.AnyAsync(student => student.StudentNumber == model.StudentNumber);
            return Json(!exists); 
        }

        // Page

        /*
        public IActionResult SetStudentSession(int StudentNumber, string FirstName, string LastName)
        {
            //var students = await _student.Students
            //    .Select(s => new StudentsRecordModel
            //    {
            //        StudentNumber = s.StudentNumber,
            //        LastName = s.LastName,
            //        FirstName = s.FirstName
            //    })
            //    .Take(5)
            //    .ToListAsync();
            //var viewModel = new CreateStudentViewModel
            //{
            //    ExistingStudents = students.AsQueryable()
            //};
            //HttpContext.Session.SetInt32("_studentID", StudentNumber);
            //HttpContext.Session.SetString("_firstName", FirstName);
            //HttpContext.Session.SetString("_lastName", LastName);
            //return View("index", viewModel);
            return View("index"); 
            //Json(new { StudentNumber, FirstName, LastName });
        }
        public IActionResult GetStudent()
        {
            var studentNumber = HttpContext.Session.GetInt32("_studentID");
            var firstName = HttpContext.Session.GetString("_firstName");
            var lastName = HttpContext.Session.GetString("_lastName");

            if (studentNumber == null || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return BadRequest(new { message = "Student data not found in session." });
            }

            return Json(new { studentNumber, firstName, lastName });
        }

        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }


        



        */

        public IActionResult Pending()
        {
            return View();
        }

        public IActionResult TrafficViolation()
        {
            return View();
        }

        //[HttpGet]
        //public IActionResult CreateCollege()
        //{
        //    var colleges = _student.College.ToList(); // Fetch all colleges
        //    ViewBag.College = colleges; 
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult CreateCollege(string collegeName)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Check if the collegeName already exists to prevent duplicate entries
        //        var existingCollege = _student.College.FirstOrDefault(c => c.CollegeID == collegeName);
        //        if (existingCollege == null)
        //        {
        //            // Create new college with the given name and set the CollegeID to the same value
        //            var newCollege = new College
        //            {
        //                CollegeID = collegeName // Use collegeName as CollegeID
        //            };

        //            _student.College.Add(newCollege);
        //            _student.SaveChanges(); // Save changes to the database

        //            TempData["SuccessMessage"] = "College added into the database successfully."; // Success message
        //        }
        //        else
        //        {
        //            TempData["ErrorMessage"] = "College ID already exists."; // Error message for duplicate CollegeID
        //        }

        //        return RedirectToAction("CreateCollege"); // Redirect to refresh the page and display success/error
        //    }

        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteCollege(string collegeID)
        //{
        //    var college = _student.College.FirstOrDefault(c => c.CollegeID == collegeID);
        //    if (college != null)
        //    {
        //        _student.College.Remove(college); // Remove the college
        //        _student.SaveChanges();           // Save changes
        //        TempData["SuccessMessage"] = "College deleted successfully.";
        //    }
        //    return RedirectToAction("CreateCollege"); // Redirect to refresh the page
        //}
    }
}
