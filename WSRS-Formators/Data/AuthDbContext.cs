using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WSRS_Formators.Models;

namespace WSRS_Formators.Data
{
    public class AuthDbContext : IdentityDbContext<FormatorUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var schema = "Formator";

            builder.Entity<FormatorUser>().ToTable("AspNetUsers", schema);
            builder.Entity<IdentityRole>().ToTable("AspNetRoles", schema);
            builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles", schema);
            builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims", schema);
            builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins", schema);
            builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims", schema);
            builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens", schema);
        }
    }
}
