using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WSRS_SWAFO.Data;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.ViewModels;

namespace WSRS_SWAFO.Controllers
{
    [Authorize(Roles = "AppRole.Admin, AppRole.Member")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync()
        {
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return Content("<script>alert('External links are disabled. Use the in-app interface to proceed.'); window.history.back();</script>", "text/html");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Index", "LogOn");
            }

            if (TempData["LogOut"] is true)
            {
                var message = new ToastViewModel
                {
                    Title = "Success",
                    Message = "Offline password has been set. You are now being logged out",
                    CssClassName = "bg-white"
                };

                TempData["LogOutMessage"] = JsonSerializer.Serialize(message);
            }
            var accountVM = new AccountViewModel
            {
                Email = user.Email!,
                Name = user.FirstName + " " + user.Surname
            };

            return View(accountVM);
        }

        [HttpGet]
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

            TempData["LogOut"] = true;
            return RedirectToAction(nameof(Index));
        }
    }
}
