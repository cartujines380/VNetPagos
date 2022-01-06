namespace VisaNet.Common.Exceptions
{
    public class ProviderBusinessException : BusinessException
    {
        public ProviderBusinessException(string code, params string[] overrides) : base(code, overrides)
        {
        }
    }
}
