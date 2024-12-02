using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.ViewModels;

namespace WSRS_SWAFO.Controllers.Encode
{
    public class StudentViolationController : Controller
    {

        private readonly ApplicationDbContext _student;

        public StudentViolationController(ApplicationDbContext student)
        {
            _student = student;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _student.Students.ToListAsync();
            return View("~/Views/Encode/StudentViolation/Index.cshtml", students);
        }

        public IActionResult Search(string searchStudent)
        {
            var studentsQuery = _student.Students.AsQueryable();

            if (!string.IsNullOrEmpty(searchStudent))
            {
                if (int.TryParse(searchStudent, out int studentNumber))
                {
                    studentsQuery = studentsQuery.Where(s => s.StudentNumber == studentNumber);
                }
                else
                {
                    studentsQuery = studentsQuery.Where(s => s.LastName.Contains(searchStudent) || s.FirstName.Contains(searchStudent));
                }
            }

            // Map the query to StudentsRecordModel
            var result = studentsQuery.Select(s => new StudentsRecordModel
            {
                StudentNumber = s.StudentNumber, // Assuming the view model requires it as a string
                LastName = s.LastName,
                FirstName = s.FirstName
            });

            return View(result); // Pass the mapped IQueryable to the view
        }
    }
}
