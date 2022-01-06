namespace VisaNet.Common.Exceptions
{
    public class CybersourceException : FatalException
    {
        public CybersourceException(string code, params string[] overrides)
            : base(code, overrides)
        {
        }
    }
}
