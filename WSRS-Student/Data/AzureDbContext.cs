using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WSRS_Student.Models;

namespace WSRS_Student.Data
{
    public class AzureDbContext : IdentityDbContext<ApplicationUser>
    {
        public AzureDbContext(DbContextOptions<AzureDbContext> options) : base(options) { }
    }
}
