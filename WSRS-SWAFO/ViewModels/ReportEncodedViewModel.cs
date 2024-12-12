using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WSRS_SWAFO.Models;

namespace WSRS_SWAFO.ViewModels
{
    public class ReportEncodedViewModel
    {
        public int Id {  get; set; }
        [ForeignKey("Offense")]
        public int OffenseId { get; set; }
        public Offense Offense { get; set; } // Navigation Property

        [ForeignKey("Colleges")]
        public string CollegeID { get; set; }
        public College College { get; set; }

        [ForeignKey("Student")]
        public int StudentNumber { get; set; }
        public Student Student { get; set; } // Navigation Property

        [ForeignKey("ApplicationUser")]
        public string? FormatorId { get; set; } // Nullable if needed
        public ApplicationUser Formator { get; set; }

        public DateOnly CommissionDate { get; set; }
        public string Course { get; set; }
        public DateOnly HearingDate { get; set; }
        public string Sanction { get; set; }
        public string Description { get; set; }
        public string StatusOfSanction { get; set; }
    }
}

//[Required(ErrorMessage = "Student ID is required")]
//[Remote(action: "CheckStudentID", controller: "Encode", ErrorMessage = "Student ID is already taken.")]
//[Display(Name = "Student Number")]
//public int StudentNumber { get; set; }

//[Required(ErrorMessage = "Student's First Name is required")]
//[Display(Name = "First Name")]
//public string FirstName { get; set; }

//[Required(ErrorMessage = "Student's Last Name is required")]
//[Display(Name = "Last Name")]
//public string LastName { get; set; }