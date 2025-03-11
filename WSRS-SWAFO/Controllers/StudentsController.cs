using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSRS_SWAFO.Data;
using WSRS_SWAFO.Models;

namespace WSRS_SWAFO.Controllers
{

    public class StudentsController : Controller
    {
        private readonly ILogger<StudentsController> _logger;
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context, ILogger<StudentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Route("api/[controller]")]
        [HttpGet("studentNumber")]
        [Produces("application/json")]
        public async Task<ActionResult<Student>> GetStudentViolationsAsync(int studentNumber)
        {
            var student = await _context.Students
                .Include(s => s.ReportsEncoded)
                    .ThenInclude(r => r.Offense)
                .Include(s => s.TrafficReportsEncoded)
                    .ThenInclude(tr => tr.Offense)
                .FirstOrDefaultAsync(s => s.StudentNumber == studentNumber);
            
            if (student == null)
            {
                return NotFound(new { message = "Student not found" });
            }

            return student;
        }
    }
}
