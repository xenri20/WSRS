using Microsoft.EntityFrameworkCore;
using WSRS_Formators.Models;

namespace WSRS_Formators.Data
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions<EmployeeDbContext> options) : base(options)
        {
        }   
        public DbSet<ReportViolation> ReportsViolation { get; set; }
     
        public DbSet<Student> Students { get; set; }
        public DbSet<Offense> Offenses { get; set; }
        public DbSet<College> Colleges { get; set; }
        public DbSet<ReportEncoded> ReportsEncoded { get; set; }
        public DbSet<TrafficReportsEncoded> TrafficReportsEncoded { get; set; }
        public DbSet<ReportPending> ReportsPending { get; set; }

        // Optional: Fluent API overrides can go here if needed.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example configuration (customize per model)
            // modelBuilder.Entity<Student>().HasKey(s => s.StudentNumber);
        }
    }
}
