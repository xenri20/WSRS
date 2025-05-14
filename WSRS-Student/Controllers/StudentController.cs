using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WSRS_Student.Dtos;

namespace WSRS_Student.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<StudentController> _logger;

        public StudentController(ILogger<StudentController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Violations()
        {
            var client = _httpClientFactory.CreateClient("WSRS_Api");

            var studentNumberClaim = User.FindFirst("StudentNumber")?.Value;

            if (string.IsNullOrEmpty(studentNumberClaim) || !int.TryParse(studentNumberClaim, out int studentNumber))
                return Unauthorized();

            try
            {
                var response = await client.GetAsync($"violations/{studentNumber}");

                if (response.IsSuccessStatusCode)
                {
                    var violations = await response.Content.ReadFromJsonAsync<AllReportsDto>();
                    return View(violations);
                }
                else 
                {
                    _logger.LogInformation($"Error Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong getting the requested data. {ex.Message}");
            }

            return View(new AllReportsDto
            {
                Violations = new List<ReportEncodedDto>(),
                TrafficViolations = new List<TrafficReportEncodedDto>()
            });
        }
    }
}
