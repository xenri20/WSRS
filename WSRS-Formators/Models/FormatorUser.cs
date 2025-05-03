using Microsoft.AspNetCore.Identity;

namespace WSRS_Formators.Models
{
    public class FormatorUser : IdentityUser
    {
        public long EmployeeId { get; set; }
        public string FullName { get; set; }
    }
}