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
}

