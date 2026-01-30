using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WSRS_SWAFO.Models;

namespace WSRS_SWAFO.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Offense> Offenses { get; set; }
        public DbSet<College> College { get; set; }
        public DbSet<HearingSchedules> HearingSchedules { get; set; }
        public DbSet<ReportEncoded> ReportsEncoded { get; set; }
        public DbSet<TrafficReportsEncoded> TrafficReportsEncoded { get; set; }
    }
}

