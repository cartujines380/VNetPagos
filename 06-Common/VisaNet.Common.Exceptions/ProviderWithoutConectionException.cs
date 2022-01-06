namespace VisaNet.Common.Exceptions
{
    public class ProviderWithoutConectionException : FatalException
    {
        public ProviderWithoutConectionException(string code, params string[] overrides)
            : base(code, overrides)
        {
        }
    }
}
