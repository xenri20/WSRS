using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WSRS_Student.Models;

namespace WSRS_Student.Data
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var schema = "Student";

            builder.Entity<ApplicationUser>().ToTable("AspNetUsers", schema);
            builder.Entity<IdentityRole>().ToTable("AspNetRoles", schema);
            builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", schema);
            builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", schema);
            builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", schema);
            builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", schema);
            builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", schema);
        }
    }
}
