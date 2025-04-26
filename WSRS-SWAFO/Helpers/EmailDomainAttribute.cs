using System.ComponentModel.DataAnnotations;

namespace WSRS_SWAFO.Helpers
{
    public class EmailDomainAttribute : ValidationAttribute
    {
        private readonly string _allowedDomain;

        public EmailDomainAttribute(string allowedDomain)
        {
            _allowedDomain = allowedDomain; 
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string email && email.EndsWith(_allowedDomain, StringComparison.OrdinalIgnoreCase))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"Email must be using the proper domain ({_allowedDomain})");
        }
    }
}
