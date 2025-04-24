using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WSRS_Formators.Dtos;
using WSRS_Formators.Models;
using WSRS_Formators.ViewModels;

namespace WSRS_Formators.Controllers
{
    public class ReportViolationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<FormatorUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public ReportViolationController(IHttpClientFactory httpClientFactory, UserManager<FormatorUser> userManager, ILogger<HomeController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult> Index(
            [FromQuery] int? studentNumber)
        {
            if (studentNumber == null)
            {
                return View(new StudentViolationsViewModel());
            }

            var client = _httpClientFactory.CreateClient("WSRS_Api");

            try
            {
                var response = await client.GetAsync($"violations/{studentNumber}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<AllReportsDto>();

                    if (data.Student == null) return View(new StudentViolationsViewModel
                    {
                        StudentNumber = studentNumber
                    });

                    var violations = new StudentViolationsViewModel
                    {
                        StudentNumber = data.Student.StudentNumber,
                        FirstName = data.Student.FirstName,
                        LastName = data.Student.LastName,
                        Violations = data.Violations,
                        TrafficViolations = data.TrafficViolations
                    };
                    return View(violations);
                }

                _logger.LogInformation($"Error Status: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong getting the requested data. {ex.Message}");
            }

            // Something went wrong, send an empty output
            return View(new StudentViolationsViewModel());
        }

        [HttpGet]
        public IActionResult Create([FromRoute] int? studentNumber)
        {
            if (studentNumber != null) ViewBag.StudentNumber = studentNumber;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] ReportPendingDto reportDto)
        {
            /*
            Intercept the value of the model before validating
             - add the user's employee id to reportDto as FormatorId
             - add the user's name to reportDto as Formator
            */
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to retrieve info of current user.");
                return View(reportDto);
            }
            reportDto.FormatorId = currentUser.EmployeeId;
            reportDto.Formator = currentUser.FullName;

            if (!ModelState.IsValid)
            {
                return View(reportDto);
            }

            var client = _httpClientFactory.CreateClient("WSRS_Api");

            var response = await client.PostAsJsonAsync("report", reportDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ReportPendingDto>();
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
