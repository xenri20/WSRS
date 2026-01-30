using Microsoft.AspNetCore.Mvc;
using WSRS_Api.Dtos;
using WSRS_Api.Interfaces;

namespace WSRS_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ViolationsController : ControllerBase
    {
        private readonly ILogger<ViolationsController> _logger;
        private readonly IViolationRepository _repository;

        public ViolationsController(ILogger<ViolationsController> logger, IViolationRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet("{studentNumber}")]
        public async Task<ActionResult<ReportEncodedDto>> GetViolationsByStudentNumber(int studentNumber)
        {
            var violations = await _repository.GetByIdAsync(studentNumber);

            // Something actually went wrong if this happened
            if (violations == null) return NotFound();

            return Ok(violations);
        }

        [HttpGet("is-clear/{studentNumber}")]
        public async Task<ActionResult<bool>> IsStudentClear(int studentNumber)
        {
            var result = await _repository.IsStudentClear(studentNumber);
            return Ok(result);
        }
    }
}
