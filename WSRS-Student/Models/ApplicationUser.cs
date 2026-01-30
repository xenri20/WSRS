using Microsoft.AspNetCore.Identity;

namespace WSRS_Student.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string StudentNumber { get; set; }
    }
}
