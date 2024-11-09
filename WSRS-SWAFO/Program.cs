using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WSRS_SWAFO.Models;
using Microsoft.Identity.Web.UI;
using WSRS_SWAFO.Data;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

public class Program
{
    public static async Task Main(String[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = "AzureWSRSLogin";
        })
        .AddCookie()
        .AddOpenIdConnect("AzureWSRSLogin", options =>
        {
            builder.Configuration.Bind("AzureWSRSLogin", options);
            options.Authority = options.Authority;
            options.ClientSecret = options.ClientSecret;
            options.ClientId = options.ClientId;
            options.CallbackPath = options.CallbackPath;
            options.ResponseType = "code";
            options.SaveTokens = true;
            options.UseTokenLifetime = true;
        });
        // builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
        //     {
        //         // Temporary definition of password requirements for testing
        //         options.SignIn.RequireConfirmedAccount = false;
        //         options.Password.RequireDigit = true;
        //         options.Password.RequireLowercase = true;
        //         options.Password.RequireUppercase = false;
        //         options.Password.RequireNonAlphanumeric = false;
        //         options.Password.RequiredLength = 6;
        //         options.Password.RequiredUniqueChars = 1;
        //     })
        //     .AddRoles<IdentityRole>()
        //     .AddEntityFrameworkStores<ApplicationDbContext>();
        // builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        builder.Services.AddMemoryCache();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Dashboard/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=LogOn}/{action=Index}/{id?}");
        app.MapRazorPages();

        // await CreateRoles(app.Services);
        // await CreateAdmin(app.Services);

        app.Run();
    }

    // private static async Task CreateRoles(IServiceProvider serviceProvider)
    // {
    //     using (var scope = serviceProvider.CreateScope())
    //     {
    //         var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    //         var roleNames = new[] { "Admin", "Manager", "Member" };

    //         foreach (var roleName in roleNames)
    //         {
    //             var roleExist = await roleManager.RoleExistsAsync(roleName);

    //             if (!roleExist)
    //             {
    //                 //create the roles and seed them to the database
    //                 await roleManager.CreateAsync(new IdentityRole(roleName));
    //             }
    //         }
    //     }
    // }

    // private static async Task CreateAdmin(IServiceProvider serviceProvider)
    // {
    //     using (var scope = serviceProvider.CreateScope())
    //     {
    //         var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    //         string adminEmail = "admin@test.com";
    //         string adminPassword = "admin123";

    //         if (await userManager.FindByEmailAsync(adminEmail) == null)
    //         {
    //             var user = new ApplicationUser();

    //             user.FirstName = "N/A";
    //             user.LastName = "N/A";
    //             user.UserName = adminEmail;
    //             user.Email = adminEmail;

    //             await userManager.CreateAsync(user, adminPassword);

    //             await userManager.AddToRoleAsync(user,"Admin");
    //         }
    //     }
    // }
}

