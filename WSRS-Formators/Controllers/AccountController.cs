using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSRS_Formators.Models;
using WSRS_Formators.ViewModels;

namespace WSRS_Formators.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<FormatorUser> _userManager;
        private readonly SignInManager<FormatorUser> _signInManager;

        public AccountController(UserManager<FormatorUser> userManager, SignInManager<FormatorUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Find user by EmployeeId instead of UserName (email)
            var user = await _userManager.Users.FirstOrDefaultAsync(f => f.EmployeeId == model.UserName);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                if (result.Succeeded)
                    return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Invalid login credentials. Please try again.";
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        [Authorize]
        public IActionResult Dashboard()
        {
            // Protected content
            return View();
        }

    }
}
