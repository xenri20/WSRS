using Microsoft.AspNetCore.Mvc;
using WSRS_Api.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WSRS_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ReportRepository _repository;

        public ReportController(ReportRepository repository)
        {
            _repository = repository;
        }

        // POST api/<ReportController>
        [HttpPost]
        public IActionResult PostStudentViolation(int FormatorId, string Description, int StudentNumber, string Formator)
        {
            var postedStudentViolation = _repository.PostStudentViolation(FormatorId, Description, StudentNumber, Formator);

            if (postedStudentViolation != null) return Ok(postedStudentViolation);

            return BadRequest("An error occurred while processing the report.");
        }
    }
}
