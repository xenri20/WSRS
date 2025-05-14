using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WSRS_Api.Dtos;
using WSRS_Api.Models;
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

        [HttpGet]
        public IActionResult GetAllStudentReports()
        {
            var reports = _repository.GetAll();
            return Ok(reports);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetStudentReports(int id)
        {
            var report = _repository.GetById(id);
            return Ok(report);
        }

        // POST api/<ReportController>
        [HttpPost]
        public IActionResult PostStudentViolation(ReportPendingDto reportDto)
        {
            var postedStudentViolation = _repository.PostStudentViolation(reportDto);

            if (postedStudentViolation != null) return Ok(postedStudentViolation);

            return BadRequest("An error occurred while processing the report.");
        }

        // PATCH api/<ReportController>
        [HttpPatch("{id:int}")]
        [Consumes("application/json-patch+json")]
        public async Task<IActionResult> PatchStudentReport(int id, [FromBody] JsonPatchDocument<ReportsPending> pending)
        {
            if (pending == null)
            {
                return BadRequest("Patch document cannot be null.");
            }

            await _repository.UpdateStudentReportPatch(id, pending);
            return Ok();
        }
    }
}
