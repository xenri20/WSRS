using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.ViewModels;

namespace WSRS_SWAFO.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "LogOn");
            }

            var accountVM = new AccountViewModel { Email = user!.Email, Name = user!.Name };
            return View(accountVM);
        }

        public async Task<IActionResult> UpdatePasswordAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "LogOn");
            }

            bool hasPassword = await _userManager.HasPasswordAsync(user);

            return View(new UpdatePasswordViewModel { HasPassword = hasPassword });
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePasswordAsync(UpdatePasswordViewModel updatePasswordVM)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["Error"] = errorList;

                return View(updatePasswordVM);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found");
                return RedirectToAction("Index", "LogOn");
            }

            // Dynamically updates the user password if they have or don't have one yet
            IdentityResult result;
            if (updatePasswordVM.HasPassword)
            {
                result = await _userManager.ChangePasswordAsync(user, updatePasswordVM.CurrentPassword, updatePasswordVM.NewPassword);
            }
            else
            {
                result = await _userManager.AddPasswordAsync(user, updatePasswordVM.NewPassword);
            }

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                var errorList = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                TempData["Error"] = errorList;
                return View(updatePasswordVM);
            }

            await _userManager.UpdateSecurityStampAsync(user);

            // TODO Add modal to show change successful and user is not being logged out

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "LogOn");
        }
    }
}
