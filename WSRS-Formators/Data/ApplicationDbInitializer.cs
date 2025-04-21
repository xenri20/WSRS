using Microsoft.AspNetCore.Identity;
using WSRS_Formators.Models;

namespace WSRS_Formators.Data
{
    public class ApplicationDbInitializer
    {
        private readonly UserManager<FormatorUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationDbInitializer(UserManager<FormatorUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            string email = "admin@dlsud.edu.ph";
            string password = "Admin@123";
            string employeeId = "20250000001";

            // Ensure default roles exist
            string[] roles = { "Admin", "Manager", "Member" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }  

            // Check if the user already exists
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new FormatorUser
                {
                    UserName = email,
                    Email = email,
                    FullName = "System Admin",
                    EmployeeId = employeeId
                };

                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }

                await _userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
