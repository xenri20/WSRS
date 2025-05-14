using System.ComponentModel.DataAnnotations;
using WSRS_Student.Data.Enum;

namespace WSRS_Student.ViewModels
{
    public class GoodMoralRequestViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Application Date")]
        public DateOnly ApplicationDate { get; set; }
        public string Purpose { get; set; }
        public string Copies { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        public string Course { get; set; }

        [Required]
        [Display(Name = "Student Number")]
        public int StudentNumber { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public bool IsApproved { get; set; }
    }
}
