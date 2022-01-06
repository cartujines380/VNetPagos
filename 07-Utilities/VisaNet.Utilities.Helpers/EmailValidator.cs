using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace VisaNet.Utilities.Helpers
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class EmailValidator : ValidationAttribute
    {
        private const string EmailRegex = @"^([\w\.\-\+]+)@([\w\-]+)((\.(\w){2,3})+)$";
        private const string InvalidCharacters = @"[áéíóúÁÉÍÓÚäëïöüÄËÏÖÜãÃ]{1,}";
        
        public override bool IsValid(object value)
        {
            if (value == null) return true;
            try
            {
                var aa = (String) value;
                if (string.IsNullOrEmpty(aa)) return true;
            }
            catch (Exception)
            {
                return true;
            }
            var regexForMail = new Regex(EmailRegex);
            var regexForCharacters = new Regex(InvalidCharacters);
            return regexForMail.IsMatch(value.ToString()) && !regexForCharacters.IsMatch(value.ToString());
        }

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
        }
    }
}
