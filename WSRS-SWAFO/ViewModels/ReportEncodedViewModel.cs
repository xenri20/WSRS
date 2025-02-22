using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WSRS_SWAFO.ViewModels
{
    public class ReportEncodedViewModel
    {
        public int Id {  get; set; }

        [Display(Name = "Student Number")]
        [ForeignKey("Student")]
        public int StudentNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required(ErrorMessage = "Select an Offense first!")]
        [Display(Name = "Offense")]
        [ForeignKey("Offense")]
        public int OffenseId { get; set; }

        [Required(ErrorMessage = "College is required")]
        [Display(Name = "College")]
        [ForeignKey("College")]
        public string CollegeID { get; set; }

        //[ForeignKey(nameof(Formator)] // Add formator table soon
        public string? Formator { get; set; } // Nullable if needed

        [Required(ErrorMessage = "Commision Date is required")]
        [Display(Name = "Commision Date")]
        public DateOnly CommissionDate { get; set; }

        [Required(ErrorMessage = "CYS is required")]
        [Display(Name = "CYS")]
        public string Course { get; set; }

        [Display(Name = "Hearing Date")]
        public DateOnly? HearingDate { get; set; }

        [Required(ErrorMessage = "Sanction is required")]
        [Display(Name = "Sanction")]
        public string Sanction { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Display(Name = "Status of Sanction")]
        public string StatusOfSanction { get; set; }
    }
}