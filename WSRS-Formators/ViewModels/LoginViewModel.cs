using System.ComponentModel.DataAnnotations;

namespace WSRS_Formators.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email Address is required")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
