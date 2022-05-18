using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Xamariners.Mvvm.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class CreditCardAttribute : ValidationAttribute
    {
        private CreditCardAttribute.CardType _cardTypes;

        public CreditCardAttribute.CardType AcceptedCardTypes
        {
            get => this._cardTypes;
            set => this._cardTypes = value;
        }

        public CreditCardAttribute() => this._cardTypes = CreditCardAttribute.CardType.All;

        public CreditCardAttribute(CreditCardAttribute.CardType AcceptedCardTypes) => this._cardTypes = AcceptedCardTypes;

        protected override ValidationResult IsValid(
          object value,
          ValidationContext validationContext)
        {
            string str = Convert.ToString(value);
            return string.IsNullOrEmpty(str) || this.IsValidType(str, this._cardTypes) && this.IsValidNumber(str) ? ValidationResult.Success : new ValidationResult(this.ErrorMessage);
        }

        private bool IsValidType(string cardNumber, CreditCardAttribute.CardType cardType)
        {
            if (Regex.IsMatch(cardNumber, "^(4)") && (cardType & CreditCardAttribute.CardType.Visa) != (CreditCardAttribute.CardType)0)
                return cardNumber.Length == 13 || cardNumber.Length == 16;
            if (Regex.IsMatch(cardNumber, "^(51|52|53|54|55)") && (cardType & CreditCardAttribute.CardType.MasterCard) != (CreditCardAttribute.CardType)0)
                return cardNumber.Length == 16;
            if (Regex.IsMatch(cardNumber, "^(34|37)") && (cardType & CreditCardAttribute.CardType.AMEX) != (CreditCardAttribute.CardType)0)
                return cardNumber.Length == 15;
            if (Regex.IsMatch(cardNumber, "^(300|301|302|303|304|305|36|38)") && (cardType & CreditCardAttribute.CardType.Diners) != (CreditCardAttribute.CardType)0)
                return cardNumber.Length == 14;
            return (cardType & CreditCardAttribute.CardType.Unknown) != (CreditCardAttribute.CardType)0;
        }

        private bool IsValidNumber(string number)
        {
            int[] numArray = new int[10]
            {
        0,
        1,
        2,
        3,
        4,
        -4,
        -3,
        -2,
        -1,
        0
            };
            int num = 0;
            char[] charArray = number.ToCharArray();
            for (int index1 = charArray.Length - 1; index1 > -1; --index1)
            {
                int index2 = (int)charArray[index1] - 48;
                num += index2;
                if ((index1 - charArray.Length) % 2 == 0)
                    num += numArray[index2];
            }
            return num % 10 == 0;
        }

        [DefaultValue(CreditCardAttribute.CardType.Unknown)]
        public enum CardType
        {
            Unknown = 1,
            Visa = 2,
            MasterCard = 4,
            AMEX = 8,
            Diners = 16, // 0x00000010
            All = 30, // 0x0000001E
            AllOrUnknown = 31, // 0x0000001F
        }
    }
}
