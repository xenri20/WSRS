using Microsoft.EntityFrameworkCore;
using WSRS_Api.Data;
using WSRS_Api.Interfaces;
using WSRS_Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    var connectionString = builder.Configuration.GetConnectionString("Azure") ?? throw new InvalidOperationException("Connection string 'AzureSQL' not found.");
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
}
else if (builder.Environment.IsDevelopment())
{
    // var connectionString = builder.Configuration.GetConnectionString("Local") ?? throw new InvalidOperationException("Connection string 'Local' not found.");
    var connectionString = builder.Configuration.GetConnectionString("Azure") ?? throw new InvalidOperationException("Connection string 'AzureSQL' not found.");
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
}

// Add services to the container.
builder.Services.AddScoped<IViolationRepository, ViolationRepository>();
// builder.Services.AddScoped<IViolationRepository, MockViolationRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "WSRSOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("WSRSOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();
