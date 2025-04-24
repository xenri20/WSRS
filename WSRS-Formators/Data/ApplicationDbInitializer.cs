using Microsoft.AspNetCore.Identity;
using WSRS_Formators.Models;

namespace WSRS_Formators.Data
{
    public class ApplicationDbInitializer
    {
        private readonly UserManager<FormatorUser> _userManager;

        public ApplicationDbInitializer(UserManager<FormatorUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            long employeeId = 20250000001;
            string email = "formator@dlsud.edu.ph";
            string password = "P@ssw0rd!";

            // Check if the user already exists
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new FormatorUser
                {
                    UserName = email, // Changed from employeeId to email
                    Email = email,
                    FullName = "John Formator",
                    EmployeeId = employeeId
                };

                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
