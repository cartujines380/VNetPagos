namespace VisaNet.Common.Exceptions
{
    public class MissingServiceReferenceParamsException : BusinessException
    {
        public MissingServiceReferenceParamsException(string code, params string[] overrides)
            : base(code, overrides)
        {
        }
    }
}