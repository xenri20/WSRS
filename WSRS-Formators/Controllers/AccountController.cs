using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

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
                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.Password);
                if (passwordCheck)
                {
                    var claims = new List<Claim>
                    {
                        new Claim("EmployeeId", user.EmployeeId.ToString()),
                        new Claim("FullName", user.FullName)
                    };

                    await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, additionalClaims: claims);

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            TempData["Error"] = "Invalid login credentials. Please try again.";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
