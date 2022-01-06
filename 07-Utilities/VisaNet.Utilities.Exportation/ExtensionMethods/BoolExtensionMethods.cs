namespace VisaNet.Utilities.Exportation.ExtensionMethods
{
    public static class BoolExtensionMethods
    {
        public static string CustomToString(this bool data)
        {
            return data ? "Si" : "No";
        }
    }
}
