using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Xamariners.Mvvm.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class EmailValidationAttribute : ValidationAttribute
    {
        private const string EMAIL_REGEX_PATTERN = "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`{}|~\\w])*)(?<=[0-9a-z])@))(?([)([(\\d{1,3}.){3}\\d{1,3}])|(([0-9a-z][-0-9a-z]*[0-9a-z]*.)+[a-z0-9][-a-z0-9]{0,22}[a-z0-9]))$";

        public string RequiredEmailErrorMessage { get; set; }

        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            string input = value as string;
            if (string.IsNullOrEmpty(input))
                return string.IsNullOrEmpty(this.RequiredEmailErrorMessage) ? new ValidationResult("Email address is required.") : new ValidationResult(this.RequiredEmailErrorMessage);
            if (input.Length > 256)
                return new ValidationResult("Email address length must be less then 256.");
            if (Regex.Match(input, "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`{}|~\\w])*)(?<=[0-9a-z])@))(?([)([(\\d{1,3}.){3}\\d{1,3}])|(([0-9a-z][-0-9a-z]*[0-9a-z]*.)+[a-z0-9][-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture).Success)
                return ValidationResult.Success;
            return string.IsNullOrEmpty(this.ErrorMessage) ? new ValidationResult("Invalid email address.") : new ValidationResult(this.ErrorMessage);
        }

        public override bool IsValid(object value) => this.IsValid(value, (ValidationContext)null) == null;
    }
}
