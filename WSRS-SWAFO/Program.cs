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

builder.Services.AddDefaultIdentity<ApplicationUser>(options => 
    {
        // Temporary definition of password requirements for testing
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// https://dotnettutorials.net/lesson/difference-between-addmvc-and-addmvccore-method/
// Adds features support for MVC and Pages
builder.Services.AddMvc();

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

//await AppDbInitializer.CreateRoles(app);
//await AppDbInitializer.CreateAdmin(app);

app.Run();
