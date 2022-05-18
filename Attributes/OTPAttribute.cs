using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Xamariners.Mvvm.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class OTPAttribute : ValidationAttribute
    {
        public OTPAttribute(int length) => this.Length = length;

        public int Length { get; }

        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            string text = value as string;
            if (string.IsNullOrEmpty(this.ErrorMessage))
                this.ErrorMessage = string.Format("OTP should not be blank and must be a number of {0} digits", (object)this.Length);
            return string.IsNullOrEmpty(text) || text.Length < 4 || text.Length > this.Length || !this.IsNumber(text) ? new ValidationResult(this.ErrorMessage) : ValidationResult.Success;
        }

        private bool IsNumber(string text) => double.TryParse(text, out double _);
    }
}
