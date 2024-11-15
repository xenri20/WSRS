using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WSRS_SWAFO.Models;

namespace WSRS_SWAFO.Services
{
    public class AccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task HandleUserSignInAsync(ClaimsPrincipal principal)
        {
            var claimsIdentity = principal.Identity as ClaimsIdentity;

            // Find the user based on their Azure AD unique identifier
            var user = await _userManager.FindByEmailAsync(claimsIdentity.FindFirst("preferred_username")?.Value);
            // Or create this user
            if (user == null)
            {
                user = new ApplicationUser
                {
                    // Mapping online user claims to ApplicationUser properties
                    Email = claimsIdentity.FindFirst("preferred_username")?.Value,
                    UserName = claimsIdentity.FindFirst("preferred_username")?.Value,
                    Name = claimsIdentity.FindFirst("name")?.Value,
                };
                await _userManager.CreateAsync(user);

                // TODO Add role as well
            }

            // Sign in the user with ASP.NET Identity, syncing the session with Azure AD login
            await _signInManager.SignInAsync(user, isPersistent: false);
        }
    }
}
