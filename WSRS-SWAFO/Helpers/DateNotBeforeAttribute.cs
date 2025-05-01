using System.ComponentModel.DataAnnotations;
using System.Reflection;

public class DateNotBeforeAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public DateNotBeforeAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var currentValue = value as DateOnly?;
        var comparisonProperty = validationContext.ObjectType.GetProperty(_comparisonProperty);

        if (comparisonProperty == null)
            throw new ArgumentException($"Property '{_comparisonProperty}' not found.");

        var comparisonValue = comparisonProperty.GetValue(validationContext.ObjectInstance) as DateOnly?;

        if (currentValue == null || comparisonValue == null)
            return ValidationResult.Success;

        if (currentValue < comparisonValue)
        {
            // Get display names for both properties
            var currentDisplayName = validationContext.DisplayName;

            var comparisonDisplayAttr = comparisonProperty
                .GetCustomAttribute<DisplayAttribute>();
            var comparisonDisplayName = comparisonDisplayAttr?.GetName() ?? _comparisonProperty;

            var errorMessage = $"{currentDisplayName} cannot be before the {comparisonDisplayName}.";
            return new ValidationResult(errorMessage);
        }

        return ValidationResult.Success;
    }
}
