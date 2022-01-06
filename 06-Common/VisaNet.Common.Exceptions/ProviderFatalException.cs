namespace VisaNet.Common.Exceptions
{
    public class ProviderFatalException : FatalException
    {
        public ProviderFatalException(string code, params string[] overrides)
            : base(code, overrides)
        {
        }
    }
}
