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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null)
            {
                var userStudentNumber = await _localContext.Users
                    .Where(u => u.Id == userId)
                    .Select(u => u.StudentNumber)
                    .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(userStudentNumber) || !int.TryParse(userStudentNumber, out _userStudentNumber))
                return Unauthorized();
            }

            var student = await _azureContext.Students
                .Include(s => s.ReportsEncoded)
                .Include(s => s.TrafficReportsEncoded)
                .FirstOrDefaultAsync(s => s.StudentNumber == _userStudentNumber); 


            return View(student);
        }
    }
}
