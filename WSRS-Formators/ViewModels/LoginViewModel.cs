using System.ComponentModel.DataAnnotations;

namespace WSRS_Formators.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Employee Id")]
        [Required(ErrorMessage = "Employee Id is required")]
        public long UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
