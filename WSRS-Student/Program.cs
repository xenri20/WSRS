using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WSRS_Student.Data;
using WSRS_Student.Models;
using WSRS_Student.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionStringLocal = builder.Configuration.GetConnectionString("Local");
var connectionStringAzure = builder.Configuration.GetConnectionString("Azure");

if (string.IsNullOrEmpty(connectionStringLocal) || string.IsNullOrEmpty(connectionStringAzure))
{
    throw new InvalidOperationException("Connection strings for Local or Azure not found");
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionStringLocal));

builder.Services.AddDbContext<AzureDbContext>(options =>
    options.UseSqlServer(connectionStringAzure));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});

// Add services to the container.
builder.Services.AddHttpClient("WSRS_Api", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("API_BASE_URL").Value
        ?? throw new InvalidOperationException("API_BASE_URL is not configured."));
});

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IApplicationDbInitializer, ApplicationDbInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");


using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IApplicationDbInitializer>();
    await initializer.CreateUserAsync();
}

app.Run();
