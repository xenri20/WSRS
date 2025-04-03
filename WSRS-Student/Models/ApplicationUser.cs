using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WSRS_Student.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string FullName { get; set; } 
        public string StudentNumber { get; set; }
    }
}
