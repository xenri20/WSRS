using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WSRS_Api.Models;
using WSRS_Api.Repositories;

namespace WSRS_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GmcController : ControllerBase
    {
        private readonly GmcRepository _repository;

        public GmcController(GmcRepository repository)
        {
            _repository = repository;
        }

        // GET api/<GmcController>
        [HttpGet]
        public IActionResult GetAllRequests()
        {
            var requests = _repository.GetAll();
            return Ok(requests);
        }

        // GET api/<GmcController>/id
        [HttpGet]
        public IActionResult GetRequest(int id)
        {
            var request = _repository.GetById(id);
            return Ok(request);
        }

        // POST api/<GmcController>
        [HttpPost]
        public IActionResult PostStudentViolation(GoodMoralRequest request)
        {
            var postedRequest = _repository.PostGoodMoralRequest(request);

            if (postedRequest != null) return Ok(postedRequest);

            return BadRequest("An error occurred while processing the good moral request.");
        }

        // PATCH api/<GmcController>
        [HttpPatch("{id:int}")]
        [Consumes("application/json-patch+json")]
        public async Task<IActionResult> PatchStudentReport(int id, [FromBody] JsonPatchDocument<GoodMoralRequest> request)
        {
            if (request == null)
            {
                return BadRequest("Patch document cannot be null.");
            }

            await _repository.UpdateGoodMoralRequestPatch(id, request);
            return Ok();
        }
    }
}
