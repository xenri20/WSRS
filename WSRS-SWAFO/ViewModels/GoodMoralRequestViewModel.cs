using System.ComponentModel.DataAnnotations;
using WSRS_SWAFO.Data.Enum;

namespace WSRS_SWAFO.ViewModels
{
    public class GoodMoralRequestViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Application Date")]
        public DateOnly ApplicationDate { get; set; }
        public string Purpose { get; set; }
        public string Copies { get; set; }
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        public string Course { get; set; }
        [Display(Name = "Student Number")]
        public int StudentNumber { get; set; }
        public Gender Gender { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public bool IsApproved { get; set; }
    }
}
