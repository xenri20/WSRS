using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WSRS_SWAFO.Models;
using WSRS_SWAFO.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using WSRS_SWAFO.Services;
using WSRS_SWAFO.Interfaces;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<AccountService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "AzureWSRSLogin";
})
.AddOpenIdConnect("AzureWSRSLogin", options =>
{
    builder.Configuration.Bind("AzureWSRSLogin", options);
    options.Authority = options.Authority;
    options.ClientSecret = options.ClientSecret;
    options.ClientId = options.ClientId;
    options.CallbackPath = options.CallbackPath;
    options.SignedOutRedirectUri = options.SignedOutRedirectUri;
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.UseTokenLifetime = true;

    // Map OIDC claims to ASP.NET Identity
    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = async context =>
        {
            var accountService = context.HttpContext.RequestServices.GetRequiredService<AccountService>();
            await accountService.HandleUserSignInAsync(context.Principal);
        }
    };
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailSender, EmailService>();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    /* PASSWORD REQUIREMENTS (based on https://learn.microsoft.com/en-us/microsoft-365/admin/misc/password-policy-recommendations?view=o365-worldwide)
        - Minimum 12 characters 
        - No strict requirements for digits, lowercase, uppercase, or non-alphanumeric characters
     */
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 12;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".WSRSAuth";
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.IsEssential = true;
});

// https://dotnettutorials.net/lesson/difference-between-addmvc-and-addmvccore-method/
// Adds features support for MVC and Pages
builder.Services.AddMvc();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Dashboard/Error");
    app.UseHsts();
}

// Data Seeding
// Note: make sure you have a valid file running studentsWithReports command
if (args.Length > 0)
{
    if (args[0] == "seed")
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();

        if (args.Length != 2)
        {
            Console.WriteLine("Usage: 'seed [ studentsWithReports | college ]'");
            return;
        }

        switch (args[1])
        {
            case "studentsWithReports":
                DataSeeder.SeedStudentReports("./Data/mock-data.xlsx", context);
                return;
            case "college":
                DataSeeder.SeedCollege(context);
                return;
            default:
                Console.WriteLine("Accepts: '[ studentsWithReports | college ]'");
                return;
        }
    }
    else
    {
        Console.WriteLine("Error: Command not found");
        return;
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

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
