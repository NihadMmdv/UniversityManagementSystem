using System;
using System.ComponentModel.DataAnnotations;

namespace UMS.Service.Validators
{
    public class NotInFutureAttribute : ValidationAttribute
    {
        public NotInFutureAttribute()
            : base("The date cannot be in the future.")
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateOnly dateValue)
            {
                if (dateValue > DateOnly.FromDateTime(DateTime.Today))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
