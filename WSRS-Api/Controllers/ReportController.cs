using Microsoft.AspNetCore.Mvc;
using WSRS_Api.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WSRS_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private readonly ReportRepository _repository;

        public ReportController(ILogger<ReportController> logger, ReportRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        // POST api/<ReportController>
        [HttpPost]
        public IActionResult PostStudentViolation()
        {
        }
    }
}
