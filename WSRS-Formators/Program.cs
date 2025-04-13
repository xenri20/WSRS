using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WSRS_Formators.Data;
using WSRS_Student.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionStringLocal = builder.Configuration.GetConnectionString("AzureStudents");
var connectionStringAzure = builder.Configuration.GetConnectionString("AzureFormator");

if (string.IsNullOrEmpty(connectionStringLocal) || string.IsNullOrEmpty(connectionStringAzure))
{
    throw new InvalidOperationException("Connection strings for Local or Azure not found");
}

builder.Services.AddDbContext<AzureDbContext>(options =>
    options.UseSqlServer(connectionStringLocal));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EmployeeDbContext>(options =>
options.UseSqlServer( builder.Configuration.GetConnectionString("DefaultConnection")) 
);

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
