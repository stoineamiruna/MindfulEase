using System;
using System.ComponentModel.DataAnnotations;

namespace MindfulEase.Validation
{
    public class DateNotInFutureAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dateValue)
            {
                return dateValue.Date <= DateTime.Today;
            }

            return true; 
        }
    }
}
