using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SystemRezerwacjiKortow.Attributes
{
    public class FloatingAttributes:ValidationAttribute
    {
        public FloatingAttributes():base("{0} ma niepoprawny format") { }
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value==null) return ValidationResult.Success;
            decimal floatValue;
            if(Decimal.TryParse(value.ToString(),out floatValue))
            return ValidationResult.Success;
            var errorMessage = FormatErrorMessage((validationContext.DisplayName));
            return new ValidationResult(errorMessage);
        }
    }
}