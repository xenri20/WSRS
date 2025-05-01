using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using WSRS_SWAFO.Models;

namespace WSRS_SWAFO.ViewModels
{
    public class EditRecordViewModel
    {
        // Values to be displayed
        public int Id { get; set; }
        public int StudentNumber { get; set; }
        public Student? Student { get; set; }
        public string College { get; set; }
        public int OffenseId { get; set; }
        public Offense? Offense { get; set; }

        // Values that can be edited
        [Required(ErrorMessage = "Please enter student's Course, Year and Section")]
        [Display(Name = "CYS")]
        public string Course { get; set; }

        public string? Formator { get; set; }
        
        // TODO Add validation logic when choosing dates
        //   - Commission date should not be later than today's date
        //   - Hearing date cannot be set before commission date
        [Required(ErrorMessage = "Please enter a date")]
        [Display(Name = "Commission Date")]
        public DateOnly CommissionDate { get; set; }

        [Required(ErrorMessage = "Please enter a sanction")]
        [Display(Name = "Sanction")]
        public string Sanction { get; set; }

        [Required(ErrorMessage = "Please enter the sanction's status")]
        [Display(Name = "Sanction Status")]
        public string StatusOfSanction { get; set; }

        [ValidateNever]
        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}
