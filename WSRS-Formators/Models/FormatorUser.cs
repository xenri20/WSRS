using Microsoft.AspNetCore.Identity;

namespace WSRS_Formators.Models
{
    public class FormatorUser : IdentityUser
    {
        public string FullName { get; set; }
        public string EmployeeId { get; set; }
    }
}
   