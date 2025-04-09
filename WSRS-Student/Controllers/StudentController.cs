using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WSRS_Student.Data;

namespace WSRS_Student.Controllers
{
    [Authorize]
    [Route("student/student")]
    public class StudentController : Controller
    {

        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public IActionResult Violations()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var student = _context.Students
                .Include(s => s.ReportsEncoded)
                .Include(s => s.TrafficReportsEncoded)
                .FirstOrDefaultAsync(s => s.IdentityUserId == userId);

            if (student == null)
            {
                return NotFound(); // Or redirect to an error page
            }

            return View(student);
        }
    }
}
