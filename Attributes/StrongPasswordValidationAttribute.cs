using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Xamariners.Mvvm.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class StrongPasswordValidationAttribute : ValidationAttribute
    {
        private const string STRONG_PASSWORD_REGEX_PATTERN = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d.*)(?=.*\\W.*)[a-zA-Z0-9\\S]{8,15}$";

        public string RequiredPasswordErrorMessage { get; set; }

        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            string input = value as string;
            if (string.IsNullOrEmpty(input))
                return string.IsNullOrEmpty(this.RequiredPasswordErrorMessage) ? new ValidationResult(validationContext.DisplayName + " is required and cannot be blank.") : new ValidationResult(this.RequiredPasswordErrorMessage);
            if (input.Length > 128)
                return new ValidationResult("Password length must be less then 128.");
            if (Regex.Match(input, "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d.*)(?=.*\\W.*)[a-zA-Z0-9\\S]{8,15}$", RegexOptions.ExplicitCapture).Success)
                return ValidationResult.Success;
            return string.IsNullOrEmpty(this.ErrorMessage) ? new ValidationResult("Your password does not fit to the password requirements, please try again.") : new ValidationResult(this.ErrorMessage);
        }
    }
}
