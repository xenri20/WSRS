using Microsoft.EntityFrameworkCore;
using WSRS_Api.Data;
using WSRS_Api.Interfaces;
using WSRS_Api.Repositories;

var builder = WebApplication.CreateBuilder(args);
// var connectionString = builder.Configuration.GetConnectionString("AzureSQL") ?? throw new InvalidOperationException("Connection string 'AzureSQL' not found.");
var connectionString = builder.Configuration.GetConnectionString("Local") ?? throw new InvalidOperationException("Connection string 'Local' not found.");

// Add services to the container.
builder.Services.AddScoped<IViolationRepository, ViolationRepository>();
// builder.Services.AddScoped<IViolationRepository, MockViolationRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
