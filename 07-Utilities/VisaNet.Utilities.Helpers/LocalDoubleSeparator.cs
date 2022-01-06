
namespace VisaNet.Utilities.Helpers
{
    public static class LocalDoubleSeparator
    {
        public static string ConvertLocal(this string value)
        {
            return value.Replace('.', ',');
        }
    }
}
