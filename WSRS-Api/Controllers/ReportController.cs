using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WSRS_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        // POST api/<ReportController>
        [HttpPost]
        public IActionResult PostStudentViolation()
        {
        }
    }
}
