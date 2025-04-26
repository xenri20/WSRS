using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WSRS_SWAFO.Helpers;

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

        [Required(ErrorMessage = "Student's Email is required")]
        [EmailDomain("dlsud.edu.ph", ErrorMessage = "Email must use dlsud.edu.ph")]
        [Display(Name = "Email Address")]
        public string Email { get; set; }
    }
}
