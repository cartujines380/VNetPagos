using VisaNet.Utilities.ExtensionMethods.Resources;

namespace VisaNet.Utilities.ExtensionMethods
{
    public static class BoolExtensionMethods
    {
        public static string CustomToString(this bool data)
        {
            return data ? HelpersStrings.True : HelpersStrings.False;
        }
    }
}
