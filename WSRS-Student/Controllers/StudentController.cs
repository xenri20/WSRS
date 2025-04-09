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
            // 
        }

    }
}
