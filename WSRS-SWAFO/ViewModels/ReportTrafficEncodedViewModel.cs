using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WSRS_SWAFO.Models;

namespace WSRS_SWAFO.ViewModels
{
    public class ReportTrafficEncodedViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Select an Offense first!")]
        [Display(Name = "Offense")]
        [ForeignKey("Offense")]
        public int OffenseId { get; set; }
        public Offense Offense { get; set; } // Navigation Property

        [Required(ErrorMessage = "Student ID is required")]
        [Display(Name = "Student Number")]
        [ForeignKey("Student")]
        public int StudentNumber { get; set; }
        public Student Student { get; set; } // Navigation Property

        [Required(ErrorMessage = "College is required")]
        [Display(Name = "College")]
        [ForeignKey("College")]
        public string CollegeID { get; set; }
        public College College { get; set; }
        [Required(ErrorMessage = "Plate Number is required!")]
        [Display(Name = "Plate Number")]
        public string PlateNumber { get; set; }
        [Required(ErrorMessage = "Commision Date is required")]
        [Display(Name = "Commision Date")]
        public DateTime CommissionDatetime { get; set; }
        [Required(ErrorMessage = "Input Place first!")]
        [Display(Name = "Place")]
        public string Place { get; set; }
        [Required(ErrorMessage = "Input your Remarks!")]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
        [Required(ErrorMessage = "Input Place first!")]
        [Display(Name = "Place")]
        public int? ORNumber { get; set; }
        public DateOnly? DatePaid { get; set; }
    }
}