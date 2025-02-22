using System.ComponentModel.DataAnnotations;

namespace WSRS_SWAFO.ViewModels
{
    public class UpdatePasswordViewModel
    {
        public bool HasPassword { get; set; }

        [Required]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        // TODO Change password requirements soon
        [Required]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
