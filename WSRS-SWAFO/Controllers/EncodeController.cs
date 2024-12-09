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

        public async Task<IActionResult> Index()
        {
            var students = await _student.Students
                .Select(s => new StudentsRecordModel
                {
                    StudentNumber = s.StudentNumber,
                    LastName = s.LastName,
                    FirstName = s.FirstName
                })
                .ToListAsync();
            var viewModel = new CreateStudentViewModel
            {
                ExistingStudents = students.AsQueryable()
            };

            return View(viewModel);
        }


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
            return View("Index", existingDataViewModel.ExistingStudents);
        }

        [HttpPost]
        public IActionResult CreateNewStudent(CreateStudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            //if (model.NewStudent == null)
            //{
            //    return View("Error");
            //}

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
    }
}
