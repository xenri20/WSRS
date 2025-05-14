using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using WSRS_Student.Models;
using WSRS_Student.ViewModels;

namespace WSRS_Student.Controllers;

[Authorize]
[Route("student/GMC")]
public class GMCController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DashboardController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public GMCController(IHttpClientFactory httpClientFactory, ILogger<DashboardController> logger, UserManager<ApplicationUser> userManager)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _userManager = userManager;
    }

    [HttpGet("")]
    public IActionResult GMCRequest()
    {
        var referer = Request.Headers["Referer"].ToString();
        if (string.IsNullOrEmpty(referer))
        {
            return Content("<script>alert('External links are disabled. Use the in-app interface to proceed.'); window.history.back();</script>", "text/html");
        }
        return View();
    }

    [HttpGet("error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GMCRequest(GoodMoralRequestViewModel model)
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

        var client = _httpClientFactory.CreateClient("WSRS_Api");

        var response = await client.PostAsJsonAsync("Gmc", model);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<GoodMoralRequestViewModel>();

            SetToastMessage("Your report has been submitted, thank you.", title: "Success");
            return RedirectToAction("Index", "Dashboard");
        }

        SetToastMessage("Something went wrong submitting your request.", title: "Error", cssClassName: "bg-danger text-white");
        var errorContent = await response.Content.ReadAsStringAsync();
        _logger.LogError("API Response: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);

        return RedirectToAction("Index", "Dashboard");
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

