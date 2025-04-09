using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WSRS_Student.Data;

namespace WSRS_Student.Controllers
{
    [Authorize]
    [Route("student/student")]
    public class StudentController : Controller
    {
        public StudentController(ApplicationDbContext context)
        {
            // 
        }

    }
}
