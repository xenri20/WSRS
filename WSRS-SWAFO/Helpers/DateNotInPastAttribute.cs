using System.ComponentModel.DataAnnotations;

namespace WSRS_SWAFO.Helpers
{
    public class DateNotInPastAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateOnly date)
            {
                return date >= DateOnly.FromDateTime(DateTime.Now);
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} cannot be a past date.";
        }
    }
}
