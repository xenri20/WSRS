using Microsoft.EntityFrameworkCore;
using WSRS_Api.Models;

namespace WSRS_Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<Offense> Offenses { get; set; }
    public DbSet<ReportEncoded> ReportsEncoded { get; set; }
    public DbSet<TrafficReportEncoded> TrafficReportsEncoded { get; set; }
    public DbSet<ReportsPending> ReportsPending { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}