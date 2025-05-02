using System.ComponentModel.DataAnnotations;

namespace WSRS_Formators.ViewModels
{
    public class CreateReportViewModel
    {
        [Required]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Report Date")]
        public DateOnly ReportDate { get; set; }
        [Required]
        [Display(Name = "Student Number")]
        public int StudentNumber { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public string College { get; set; }
        [Required]
        [Display(Name = "CYS")]
        public string CourseYearSection { get; set; }
    }
}
