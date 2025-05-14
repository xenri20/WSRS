using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WSRS_Student.Models;

namespace WSRS_Student.Controllers;

[Authorize]
[Route("student/GMC")]
public class GMCController : Controller
{
    private readonly ILogger<DashboardController> _logger;

    public GMCController(ILogger<DashboardController> logger)
    {
        _logger = logger;
    }

    [HttpGet("")]
    public IActionResult GMCRequest()
    {
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
    public async Task<IActionResult> PostGMCRequest(GoodMoralRequestViewModel model)
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
            return RedirectToAction(nameof(Index));
        }

        SetToastMessage("Something went wrong submitting your request.", title: "Error", cssClassName: "bg-danger text-white");
        var errorContent = await response.Content.ReadAsStringAsync();
        _logger.LogError("API Response: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);

        return RedirectToAction(nameof(Index));
    }
}

