using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WSRS_Api.Dtos;

namespace WSRS_Student.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public StudentController(IHttpClientFactory httpClientFactory)
        {
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
            }
            catch (Exception ex)
            {
                // TODO add logger for exception here
            }

            return View(new AllReportsDto
            {
                Violations = new List<ReportEncodedDto>(),
                TrafficViolations = new List<TrafficReportEncodedDto>()
            });
        }
    }
}
