using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WSRS_Formators.Models;

namespace WSRS_Formators.Data
{
    public class AuthDbContext : IdentityDbContext<FormatorUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Offense> Offenses { get; set; }
        public DbSet<FormatorUser> FormatorUser { get; set; }
        public DbSet<College> Colleges { get; set; }
        public DbSet<ReportEncoded> ReportsEncoded { get; set; }
        public DbSet<TrafficReportsEncoded> TrafficReportsEncoded { get; set; }
        public DbSet<ReportPending> ReportsPending { get; set; }
    }
}
