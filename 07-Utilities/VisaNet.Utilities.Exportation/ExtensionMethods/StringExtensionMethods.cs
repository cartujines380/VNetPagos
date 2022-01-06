using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace VisaNet.Utilities.Exportation.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            if (source == null)
                return false;

            source = source.Replace("Á", "A").Replace("á", "a").Replace("É", "E").Replace("é", "e").Replace("Í", "I")
                           .Replace("í", "i").Replace("Ó", "O").Replace("ó", "o").Replace("Ú", "U").Replace("ú", "u");

            if (toCheck != null)
            {
                toCheck = toCheck.Replace("Á", "A").Replace("á", "a").Replace("É", "E").Replace("é", "e").Replace("Í", "I")
                                 .Replace("í", "i").Replace("Ó", "O").Replace("ó", "o").Replace("Ú", "U").Replace("ú", "u");
            }

            return source.IndexOf(toCheck, comp) >= 0;
        }

        public static string Short(this string value, int maxAllowed)
        {
            if (value == null)
                return null;

            if (value.Length > maxAllowed)
                return value.Substring(0, maxAllowed - 3) + "...";

            return value;
        }

        public static string ToSHA256(this string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hashstring = new SHA256Managed();
            var hash = hashstring.ComputeHash(bytes);
            return hash.Aggregate(string.Empty, (current, x) => current + String.Format("{0:x2}", x));
        }

        public static bool IsValidEmail(this string inputEmail)
        {
            const string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                                    @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                                    @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";

            return ((new Regex(strRegex)).IsMatch(inputEmail));
        }
    }
}
