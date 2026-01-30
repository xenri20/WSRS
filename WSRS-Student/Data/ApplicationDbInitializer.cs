using Microsoft.AspNetCore.Identity;
using WSRS_Student.Interfaces;
using WSRS_Student.Models;

namespace WSRS_Student.Data
{
    public class ApplicationDbInitializer : IApplicationDbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationDbInitializer(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task CreateUserAsync()
        {
            string email = "student@dlsud.edu.ph";
            string password = "P@ssw0rd!";
            string studentNumber = "202100001";

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser()
                {
                    FullName = "SUBJECT, TEST",
                    UserName = email,
                    Email = email,
                    StudentNumber = studentNumber
                };

                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                    throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                //await userManager.AddToRoleAsync(user,"User");
            }
        }
    }
}
