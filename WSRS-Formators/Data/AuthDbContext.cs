using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WSRS_Formators.Models;

namespace WSRS_Formators.Data
{
    public class AuthDbContext : IdentityDbContext<FormatorUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    }
}
