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
    }
    
}
