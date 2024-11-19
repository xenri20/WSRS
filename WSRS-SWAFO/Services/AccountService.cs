using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using WSRS_SWAFO.Models;

namespace WSRS_SWAFO.Services
{
    public class AccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountService> _logger;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task HandleUserSignInAsync(ClaimsPrincipal principal)
        {
            var claimsIdentity = principal.Identity as ClaimsIdentity;

            var email = claimsIdentity.FindFirst("preferred_username")?.Value;

            // Find the user based on their Azure AD unique identifier
            var user = await _userManager.FindByEmailAsync(claimsIdentity.FindFirst("preferred_username")?.Value);
            // Or create this user
            if (user == null)
            {
                user = new ApplicationUser
                {
                    // Mapping online user claims to ApplicationUser properties
                    Email = email,
                    UserName = email,
                    Name = claimsIdentity.FindFirst("name")?.Value,
                };
                var result = await _userManager.CreateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("A user has just been registered to the database");
                }

                // TODO Add role as well
            }

            // Sign in the user with ASP.NET Identity, syncing the session with Azure AD login
            await _signInManager.SignInAsync(user, isPersistent: false);
        }
    }
}
