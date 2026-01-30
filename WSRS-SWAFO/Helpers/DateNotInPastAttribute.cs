using System.ComponentModel.DataAnnotations;

namespace WSRS_SWAFO.Helpers
{
    public class DateNotInPastAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value) => value switch
        {
            DateOnly date => date >= DateOnly.FromDateTime(DateTime.Now),
            _ => true
        };

        public override string FormatErrorMessage(string name) => $"{name} cannot be a past date.";
    }
}
