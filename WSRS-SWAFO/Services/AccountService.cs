using System.Security.Claims;
using System.Security.Principal;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
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

            var email = claimsIdentity?.FindFirst("preferred_username")?.Value;
            var surname = claimsIdentity?.FindFirst(ClaimTypes.Surname)?.Value;
            var firstName = claimsIdentity?.FindFirst(ClaimTypes.GivenName)?.Value;

            // Find the user based on their Azure AD unique identifier
            var user = await _userManager.FindByEmailAsync(claimsIdentity!.FindFirst("preferred_username")?.Value!);
            // Or create this user
            if (user == null)
            {
                user = new ApplicationUser
                {
                    // Mapping online user claims to ApplicationUser properties
                    Email = email,
                    UserName = email,
                    Surname = surname!,
                    FirstName = firstName!,
                };

                var role = claimsIdentity?.FindAll(ClaimTypes.Role)
                    .Select(r => r.Value)
                    .ToList();

                var result = await _userManager.CreateAsync(user);

                if (role != null)
                {
                    await _userManager.AddToRolesAsync(user, role);
                } else
                {
                    await _userManager.AddToRoleAsync(user, "AppRole.Member");
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation("A user has just been registered to the database");
                }

                // TODO Add role as well
            }
            
            // For promptless sign out purposes (e.g. no more choosing which user to sign out as)
            var loginHint = claimsIdentity!.FindFirst("login_hint")?.Value;

            if (!string.IsNullOrEmpty(loginHint))
            {
                // Sync Azure AD claims with ASP.NET Identity claims
                var additionalClaims = new List<Claim>
                {
                    new Claim("login_hint", loginHint),
                    new Claim(ClaimTypes.Surname, surname!),
                    new Claim(ClaimTypes.GivenName, firstName!),
                };

                await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, additionalClaims: additionalClaims);
            }
            else
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
            }
        }
    }
}
