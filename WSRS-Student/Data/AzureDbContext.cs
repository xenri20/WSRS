using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WSRS_Student.Models;

namespace WSRS_Student.Data
{
    public class AzureDbContext : IdentityDbContext<ApplicationUser>
    {
        public AzureDbContext(DbContextOptions<AzureDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Offense> Offenses { get; set; }
        public DbSet<College> Colleges { get; set; }
        public DbSet<ReportEncoded> ReportsEncoded { get; set; }
        public DbSet<TrafficReportsEncoded> TrafficReportsEncoded { get; set; }
        public DbSet<ReportPending> ReportsPending { get; set; }
    }
}
