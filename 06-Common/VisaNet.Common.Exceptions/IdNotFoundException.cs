namespace VisaNet.Common.Exceptions
{
    public class IdNotFoundException : BusinessException
    {
        public IdNotFoundException(string code, params string[] overrides)
            : base(code, overrides)
        {
        }
    }
}