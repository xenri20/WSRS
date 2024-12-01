using System.ComponentModel.DataAnnotations;

namespace WSRS_SWAFO.ViewModels
{
    public class StudentsRecordModel
    {
        [Display(Name = "Student Number")]
        //[Required(ErrorMessage = "Email Address is required")]
        public string StudentNumber { get; set; }

        [Display(Name = "First Name")]
        //[Required(ErrorMessage = "Password is required")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "CYS")]
        public string Course { get; set; }
    }
}
