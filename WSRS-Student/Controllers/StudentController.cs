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
        private readonly ApplicationDbContext _localContext;
        private readonly AzureDbContext _azureContext;
        private int _userStudentNumber;

        public StudentController(ApplicationDbContext localContext, AzureDbContext azureContext)
        {
            _localContext = localContext;
            _azureContext = azureContext;
        }

        public async Task<IActionResult> Violations()
        {
            var studentNumberClaim = User.FindFirst("StudentNumber")?.Value;

            if (string.IsNullOrEmpty(studentNumberClaim) || !int.TryParse(studentNumberClaim, out var studentNumber))
                return Unauthorized();

            var student = await _context.Students
                .Include(s => s.ReportsEncoded)
                .Include(s => s.TrafficReportsEncoded)
                .FirstOrDefaultAsync(s => s.StudentNumber == studentNumber);

            return View(student);
        }
    }
}
