using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WSRS_Student.Data;
using WSRS_Student.Models;
using WSRS_Student.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.IsDevelopment() ? "Local" : "Azure";
var connectionString = builder.Configuration.GetConnectionString(environment)
    ?? throw new InvalidOperationException($"Connection string {environment} not found");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});

// Add services to the container.
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
