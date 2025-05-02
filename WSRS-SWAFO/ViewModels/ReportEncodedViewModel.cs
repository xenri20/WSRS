using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WSRS_SWAFO.Helpers;
using WSRS_SWAFO.Models;

namespace WSRS_SWAFO.ViewModels
{
    public class ReportEncodedViewModel
    {
        public int Id {  get; set; }

        [Display(Name = "Student Number")]
        [ForeignKey(nameof(Student))]
        public int StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "Select an Offense first!")]
        [Display(Name = "Offense")]
        [ForeignKey(nameof(Offense))]
        public int OffenseId { get; set; }

        [Required(ErrorMessage = "College is required")]
        [Display(Name = "College")]
        [ForeignKey(nameof(College))]
        public string CollegeID { get; set; }

        public string? Formator { get; set; } // Nullable if needed

        [Required(ErrorMessage = "Commission Date is required")]
        [Display(Name = "Commission Date")]
        [DateNotInPast(ErrorMessage = "The date must be today or later.")]
        public DateOnly CommissionDate { get; set; }

        [Required(ErrorMessage = "CYS is required")]
        [Display(Name = "CYS")]
        public string Course { get; set; }

        [Required(ErrorMessage = "Sanction is required")]
        [Display(Name = "Sanction")]
        public string Sanction { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status")]
        public string StatusOfSanction { get; set; }
    }
}