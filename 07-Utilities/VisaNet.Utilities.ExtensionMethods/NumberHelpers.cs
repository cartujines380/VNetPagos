using System;

namespace VisaNet.Utilities.ExtensionMethods
{
    public static class NumberHelpers
    {
        public static decimal SignificantDigits(this decimal value, int digits)
        {
            return decimal.Round(value, digits, MidpointRounding.AwayFromZero);
        }

        public static double SignificantDigits(this double value, int digits)
        {
            if (double.IsNaN(value)) return -1;
            
            return (double)decimal.Round((decimal)value, digits, MidpointRounding.AwayFromZero);
        }
        public static double SignificantDigits(this int value, int digits)
        {
            if (double.IsNaN(value)) return -1;

            return (double)decimal.Round(value, digits, MidpointRounding.AwayFromZero);
        }

        public static string ToStringComparable(this int number, int stringLength)
        {
            return number.ToString(new string('0', stringLength));
        }
        public static string ToStringComparable(this decimal number, int stringLength)
        {
            var numberString = number.ToString();

            var actualStringLength = numberString.Length;

            //si hay comas las cambio por 0
            if (numberString.Contains(","))
            {
                numberString = numberString.Replace(",", "");
                numberString = "0" + numberString;
                actualStringLength--;
            }
            var result = string.Empty;

            while (actualStringLength < stringLength)
            {
                result += "0";
                actualStringLength++;
            }

            result += numberString;

            return result;
        }

        public static string ToStringComparable(this double number, int stringLength)
        {
            var numberString = number.ToString();

            var actualStringLength = numberString.Length;

            //si hay comas las cambio por 0
            if (numberString.Contains(","))
            {
                numberString = numberString.Replace(",", "");
                numberString = "0" + numberString;
                actualStringLength--;
            }
            
            var result = string.Empty;

            while (actualStringLength < stringLength)
            {
                result += "0";
                actualStringLength++;
            }

            result += numberString;

            return result;
        }
    }
}
