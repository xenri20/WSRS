using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using WSRS_SWAFO.Data.Enum;

namespace WSRS_SWAFO.ViewModels
{
    public class EditTrafficRecordViewModel
    {
        // Values to be displayed
        public int Id { get; set; }
        public int StudentNumber { get; set; }

        [ValidateNever]
        public string LastName { get; set; }

        [ValidateNever]
        public string FirstName { get; set; }
        public string College { get; set; }
        public int OffenseId { get; set; }

        [ValidateNever]
        public string Nature { get; set; }

        [ValidateNever]
        public OffenseClassification Classification { get; set; }

        // Values that can be edited

        [Required(ErrorMessage = "Please enter a date")]
        [Display(Name = "Commission Date")]
        public DateOnly CommissionDate { get; set; }

        [Required(ErrorMessage = "Please enter the plate number")]
        [Display(Name = "Plate number")]
        public string PlateNumber { get; set; }

        [Required(ErrorMessage = "Please enter a place")]
        [Display(Name = "Place")]
        public string Place { get; set; }

        [Required(ErrorMessage = "Please enter your remarks")]
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [ValidateNever]
        [Display(Name = "Date Paid")]
        public DateOnly? DatePaid { get; set; }

        [ValidateNever]
        [Display(Name = "OR Number")]
        public string? ORNumber { get; set; }
    }
}
