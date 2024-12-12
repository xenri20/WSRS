using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WSRS_SWAFO.ViewModels
{
    public class StudentRecordViewModel
    {
        [Required(ErrorMessage = "Student ID is required")]
        [Remote(action: "CheckStudentID", controller: "Encode", ErrorMessage = "Student ID is already taken.")]
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
