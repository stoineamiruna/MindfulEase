using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MindfulEase.Validation
{
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            PropertyInfo? comparisonPropertyInfo = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (comparisonPropertyInfo == null)
                return new ValidationResult($"Unknown property: {_comparisonProperty}");

            var comparisonValue = comparisonPropertyInfo.GetValue(validationContext.ObjectInstance);

            if (value == null || comparisonValue == null)
                return ValidationResult.Success;

            if (value is DateTime currentValue && comparisonValue is DateTime comparisonDate)
            {
                if (currentValue <= comparisonDate)
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be greater than {_comparisonProperty}.");
            }
            else
            {
                return new ValidationResult("Invalid data types");
            }

            return ValidationResult.Success;
        }
    }
}
