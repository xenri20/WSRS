using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.ViewModels;

namespace WSRS_SWAFO.Controllers
{
    [AllowAnonymous]
    public class LogOnController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogOnController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            bool isOnline = await NetworkHelper.IsOnline();
            ViewData["isOnline"] = isOnline;

            return View();
        }

        public async Task<IActionResult> SignInWithMicrosoft()
        {    
            bool isOnline = await NetworkHelper.IsOnline();
            if (!isOnline)
            { 
                // Automatically refreshes page if there is no internet connection
                return RedirectToAction("Index");
            }

            var redirectUrl = Url.Action("Index", "Dashboard");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, "AzureWSRSLogin");
        }

        [HttpPost]
        public async Task<IActionResult> SignInLocally(LoginViewModel loginViewModel)
        {
            bool isOnline = await NetworkHelper.IsOnline();
            if (isOnline)
            {
                // Automatically refreshes page if there is internet connection
                RedirectToAction("Index");
            }

            if (!ModelState.IsValid) return View("Index", loginViewModel);

            var result = await _signInManager.PasswordSignInAsync(loginViewModel.EmailAddress, loginViewModel.Password, false, lockoutOnFailure: false);
 
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            TempData["Error"] = "Invalid offline login credentials. Please try again";
            return View("Index", loginViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            // ASP.NET Core Identity Sign out
            await _signInManager.SignOutAsync();

            // Azure AD Sign out
            bool isOnline = await NetworkHelper.IsOnline();
            if (isOnline)
            {
                // Avoids error when trying to sign out from Azure AD when offline
                await HttpContext.SignOutAsync("AzureWSRSLogin");
            }

            // Cookie Authentication Scheme Sign out
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index");
        }
    }
}
