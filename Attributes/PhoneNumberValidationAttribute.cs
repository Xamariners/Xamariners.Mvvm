using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Xamariners.Mvvm.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PhoneNumberValidationAttribute : ValidationAttribute
    {
        private readonly int _length;

        public PhoneNumberValidationAttribute(int length)
        {
            this.ErrorMessage = "Invalid phone number";
            this._length = length;
        }

        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            return value == null || value is string number && this.IsPhoneNumber(number, this._length) ? ValidationResult.Success : new ValidationResult(this.ErrorMessage);
        }

        private bool IsPhoneNumber(string number, int length) => Regex.Match(number, string.Format("^([0-9]{0})$", (object)length)).Success;
    }
}
