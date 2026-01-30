using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WSRS_Formators.Dtos;
using WSRS_Formators.Models;
using WSRS_Formators.ViewModels;

namespace WSRS_Formators.Controllers
{
    [Authorize]
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
        public IActionResult Create(
            [FromQuery] int? studentNumber,
            [FromQuery] string? firstName,
            [FromQuery] string? lastName)
        {
            if (studentNumber != null || !string.IsNullOrEmpty(firstName) || !string.IsNullOrEmpty(lastName))
            {
                ViewData["StudentNumber"] = studentNumber;
                ViewData["FirstName"] = firstName;
                ViewData["LastName"] = lastName;

                return View();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateReportViewModel model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                ModelState.AddModelError(string.Empty, "Unable to retrieve info of current user.");
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var reportDto = new ReportPendingDto
            {
                FormatorId = currentUser.EmployeeId,
                Formator = currentUser.FullName,
                ReportDate = model.ReportDate,
                StudentNumber = model.StudentNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                College = model.College,
                CourseYearSection = model.CourseYearSection,
                Description = model.Description
            };

            var client = _httpClientFactory.CreateClient("WSRS_Api");

            var response = await client.PostAsJsonAsync("report", reportDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ReportPendingDto>();

                SetToastMessage("Your report has been submitted, thank you.", title: "Success");
                return RedirectToAction(nameof(Index));
            }

            SetToastMessage("Something went wrong submitting your report.", title: "Error", cssClassName: "bg-danger text-white");
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("API Response: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);

            return RedirectToAction(nameof(Index));
        }

        private void SetToastMessage(string message, string title = "", string cssClassName = "bg-white")
        {
            var toastMessage = new ToastViewModel
            {
                Title = title,
                Message = message,
                CssClassName = cssClassName
            };
            TempData["Result"] = JsonSerializer.Serialize(toastMessage);
        }
    }
}
