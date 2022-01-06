using System;

namespace VisaNet.Utilities.Exportation.ExtensionMethods
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
    }
}
