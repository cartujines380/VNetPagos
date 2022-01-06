using System;
using System.ComponentModel;

namespace VisaNet.Utilities.JsonResponse
{
    public static class EnumExtensionMethods
    {
        public static string ToDescription(this Enum val)
        {
            string enumName = val.ToString();
            var attributes =
                (DescriptionAttribute[])val.GetType()
                .GetField(enumName)
                .GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? attributes[0].Description : enumName;
        }
    }
}
