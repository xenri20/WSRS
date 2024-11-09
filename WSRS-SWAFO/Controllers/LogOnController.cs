using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WSRS_SWAFO.Controllers
{
    public class LogOnController : Controller
    {
        public async Task<IActionResult> IndexAsync()
        {
            bool isOnline = await NetworkHelper.IsOnline();
            ViewData["isOnline"] = isOnline;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult SignInWithMicrosoft()
        {
            var redirectUrl = Url.Action("Index", "Dashboard");
            return Challenge(new AuthenticationProperties { RedirectUri = redirectUrl }, "AzureWSRSLogin");
        }
    }
}
