using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WSRS_SWAFO.Helpers;
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
        public async Task<IActionResult> SignOutPartial()
        {
            // ASP.NET Core Identity Sign out
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "LogOn");
        }

        [HttpPost]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();

            // Azure AD Signs out. Sign out completely from the application
            bool isOnline = await NetworkHelper.IsOnline();
            if (isOnline)
            {
                var idToken = User.FindFirst("login_hint")?.Value;
                var postLogoutRedirectUri = Url.Action("Index", "LogOn", null, Request.Scheme);

                string logoutUrl = $"https://login.microsoftonline.com/common/oauth2/v2.0/logout?post_logout_redirect_uri={postLogoutRedirectUri}";

                if (!string.IsNullOrEmpty(idToken))
                {
                    logoutUrl += $"&logout_hint={idToken}";
                }

                return Redirect(logoutUrl);
            }

            return RedirectToAction("Index", "LogOn");
        }
    }
}
