using Microsoft.EntityFrameworkCore;
using WSRS_Api.Models;

namespace WSRS_Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<ReportEncoded> ReportsEncoded { get; set; }
    public DbSet<TrafficReportEncoded> TrafficReportsEncoded { get; set; }
    public DbSet<ReportsPending> ReportsPending { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Student>()
            .ToView(null); // prevents EF from trying to generate migrations for this read-only table

        builder.Entity<ReportEncoded>()
            .ToView(null); // prevents EF from trying to generate migrations for this read-only table

        builder.Entity<TrafficReportEncoded>()
            .ToView(null); // prevents EF from trying to generate migrations for this read-only table
    }
}