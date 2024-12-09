using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace WSRS_SWAFO.ViewModels
{
    public class StudentsRecordModel
    {
        [Required(ErrorMessage = "Student ID is required")]
        [Display(Name = "Student Number")]
        public int StudentNumber { get; set; }

        [Required(ErrorMessage = "Student's First Name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Student's Last Name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}
