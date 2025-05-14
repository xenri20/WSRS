using System.ComponentModel.DataAnnotations;

namespace WSRS_Student.Data.Enum
{
    public enum Gender
    {
        Male,
        Female,
        [Display(Name = "Prefer Not To Say")]
        PreferNotToSay
    }
}