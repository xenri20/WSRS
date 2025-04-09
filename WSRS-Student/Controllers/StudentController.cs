using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSRS_Student.Data;

namespace WSRS_Student.Controllers
{
    [Authorize]
    [Route("student/student")]
    public class StudentController : Controller
    {
        private readonly AzureDbContext _azureContext;

        public StudentController(AzureDbContext azureContext)
        {
            _azureContext = azureContext;
        }

        public async Task<IActionResult> Violations()
        {
            var studentNumberClaim = User.FindFirst("StudentNumber")?.Value;

            if (string.IsNullOrEmpty(studentNumberClaim) || !int.TryParse(studentNumberClaim, out var studentNumber))
                return Unauthorized();

            /*
             This is assuming we directly query against the context, instead
             of having an external api
            */
            var student = await _azureContext.Students
                .Include(s => s.ReportsEncoded)
                .Include(s => s.TrafficReportsEncoded)
                .FirstOrDefaultAsync(s => s.StudentNumber == studentNumber);

            return View(student);
        }
    }
}
