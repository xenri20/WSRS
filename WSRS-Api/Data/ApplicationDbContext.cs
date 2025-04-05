using Microsoft.EntityFrameworkCore;
using WSRS_Api.Models;

namespace WSRS_Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<ReportEncoded> ReportsEncoded { get; }
}
