using System.ComponentModel.DataAnnotations;

namespace TrainVault.CustomValidation
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value != null)
            {
                DateTime date;
                if (value is DateTime dateTimeValue)
                {
                    date = dateTimeValue;
                }
                else if (DateTime.TryParse(value.ToString(), out date))
                {
                    // Handles DateOnly or string cases
                }
                else
                {
                    return new ValidationResult("Invalid date format.");
                }

                if (date >= DateTime.Today)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("The date must be today or in the future.");
                }
            }

            return new ValidationResult("Date is required.");
        }
    }
}
