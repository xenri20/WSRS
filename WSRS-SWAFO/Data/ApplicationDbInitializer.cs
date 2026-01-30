using Microsoft.AspNetCore.Identity;
using WSRS_SWAFO.Models;

namespace WSRS_SWAFO.Data
{
    public class ApplicationDbInitializer
    {
        public async Task SeedRoles(IApplicationBuilder applicationBuilder)
        {
            await CreateRoles(applicationBuilder);
        }

        private static async Task CreateRoles(IApplicationBuilder applicationBuilder)
        {
            using (var scope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roleNames = new[] { "AppRole.Admin", "AppRole.Member" };

                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);

                    if (!roleExist)
                    {
                        //create the roles and seed them to the database
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }
        }

        private static async Task CreateAdmin(IApplicationBuilder applicationBuilder)
        {
            using (var scope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                string adminEmail = "admin@test.com";
                string adminPassword = "admin123";

                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var user = new ApplicationUser();

                    user.Surname = "N/A";
                    user.FirstName = "N/A";
                    user.UserName = adminEmail;
                    user.Email = adminEmail;

                    await userManager.CreateAsync(user, adminPassword);

                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
