using System.ComponentModel.DataAnnotations;
using WSRS_SWAFO.Helpers;

namespace WSRS_SWAFO.ViewModels
{
    public class TrafficReportEncodedViewModel
    {
        [Required(ErrorMessage = "Student Number is required")]
        [Display(Name = "Student Number")]
        public int StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "Select an Offense first!")]
        [Display(Name = "Offense")]
        public int OffenseId { get; set; }

        [Required(ErrorMessage = "College is required")]
        [Display(Name = "College")]
        public string CollegeID { get; set; }

        [Required(ErrorMessage = "Commission Date is required")]
        [Display(Name = "Commission Date")]
        [DateNotInPast(ErrorMessage = "The date must be today or later.")]
        public DateOnly CommissionDate { get; set; }

        [Required(ErrorMessage = "Plate Number is required!")]
        [Display(Name = "Plate Number")]
        public string PlateNumber { get; set; }

        [Required(ErrorMessage = "Input Place first!")]
        [Display(Name = "Place")]
        public string Place { get; set; }

        [Required(ErrorMessage = "Input your Remarks!")]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Date Paid")]
        public DateOnly? DatePaid { get; set; }

        [Display(Name = "OR Number")]
        public string? ORNumber { get; set; }
    }
}
