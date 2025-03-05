using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<Student>> GetStudentAsync(int studentNumber)
        {
            var student = await _context.Students.FindAsync(studentNumber);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }
    }
}
