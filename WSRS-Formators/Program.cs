using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WSRS_Formators.Data;
using WSRS_Formators.Models;
using WSRS_Student.Data;

var builder = WebApplication.CreateBuilder(args);

// Load connection strings
var connectionStringLocal = builder.Configuration.GetConnectionString("Local");
var connectionStringAzure = builder.Configuration.GetConnectionString("AzureFormator");

if (string.IsNullOrEmpty(connectionStringLocal) || string.IsNullOrEmpty(connectionStringAzure))
{
    throw new InvalidOperationException("Connection strings for Local or Azure not found");
}

// Add DB contexts
builder.Services.AddDbContext<AzureDbContext>(options =>
    options.UseSqlServer(connectionStringLocal));

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(connectionStringLocal));

// Configure Identity
builder.Services.AddIdentity<FormatorUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});

builder.Services.AddHttpClient("WSRS_Api", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("API_BASE_URL").Value 
        ?? throw new InvalidOperationException("API_BASE_URL is not configured."));
});

// MVC setup
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

// Seed roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var initializer = new ApplicationDbInitializer(
        services.GetRequiredService<UserManager<FormatorUser>>(),
        services.GetRequiredService<RoleManager<IdentityRole>>()
    );
    await initializer.SeedAsync();
}

app.Run();
