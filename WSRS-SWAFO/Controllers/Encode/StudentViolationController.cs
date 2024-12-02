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
    }
}
