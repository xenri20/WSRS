using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using WSRS_SWAFO.Data.Enum;

namespace WSRS_SWAFO.ViewModels
{
    public class EditRecordViewModel
    {
        // Values to be displayed
        public int Id { get; set; }

        [ValidateNever]
        public int StudentNumber { get; set; }

        [ValidateNever]
        public string LastName { get; set; }

        [ValidateNever]
        public string FirstName { get; set; }

        [ValidateNever]
        public string College { get; set; }
        public int OffenseId { get; set; }

        [ValidateNever]
        public string Nature { get; set; }

        [ValidateNever]
        public OffenseClassification Classification { get; set; }

        // Values that can be edited
        [Required(ErrorMessage = "Please enter student's Course, Year and Section")]
        [Display(Name = "CYS")]
        public string Course { get; set; }

        public string? Formator { get; set; }

        [Required]
        [Display(Name = "previous date")]
        public DateOnly? OriginalDate { get; set; }

        // Commission date should not be later than today's date
        [Required(ErrorMessage = "Please enter a date")]
        [Display(Name = "Commission Date")]
        [DateNotBefore(nameof(OriginalDate))]
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
