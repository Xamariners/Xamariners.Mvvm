using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Xamariners.Mvvm.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PersonNameAttribute : ValidationAttribute
    {
        private const string PERSON_NAME_REGEX_PATTERN = "^[\\p{L}'][ \\p{L}\\p{IsKatakana}\\p{IsHiragana}'-]*[\\p{L}]$";

        public string RequiredNameErrorMessage { get; set; }

        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            string str = value as string;
            if (string.IsNullOrEmpty(str))
                return string.IsNullOrEmpty(this.RequiredNameErrorMessage) ? new ValidationResult("Person name is required.") : new ValidationResult(this.RequiredNameErrorMessage);
            if (Regex.Match(str.Trim(), "^[\\p{L}'][ \\p{L}\\p{IsKatakana}\\p{IsHiragana}'-]*[\\p{L}]$").Success)
                return ValidationResult.Success;
            return string.IsNullOrEmpty(this.ErrorMessage) ? new ValidationResult("Person name is required and must be alphabetical characters only.") : new ValidationResult(this.ErrorMessage);
        }

        public override bool IsValid(object value) => this.IsValid(value, (ValidationContext)null) == null;
    }
}
