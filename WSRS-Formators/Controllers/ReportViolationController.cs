using Microsoft.AspNetCore.Mvc;
using WSRS_Formators.Dtos;
using WSRS_Formators.Models;
using WSRS_Formators.ViewModels;

namespace WSRS_Formators.Controllers
{
    public class ReportViolationController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public ReportViolationController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> Index(
            [FromRoute] int? studentNumber)
        {
            var client = _httpClientFactory.CreateClient("WSRS_Api");

            if (studentNumber == null) 
            {
                return View();
            }

            try
            {
                var response = await client.GetAsync($"violations/{studentNumber}");

                if (response.IsSuccessStatusCode)
                {
                    var violations = await response.Content.ReadFromJsonAsync<StudentViolationsViewModel>();
                    return View(violations);
                }

                _logger.LogInformation($"Error Status: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong getting the requested data. {ex.Message}");
            }

            // Something went wrong, send an empty output
            return View(new StudentViolationsViewModel
            {
                Student = new Student(),
                Violations = new List<ReportEncodedDto>(),
                TrafficViolations = new List<TrafficReportEncodedDto>()
            });
        }

        [HttpGet]
        public IActionResult Create([FromRoute] int? studentNumber)
        {
            if (studentNumber != null) ViewBag.StudentNumber = studentNumber;
            return View();
        }
    }
}
