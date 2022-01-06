namespace VisaNet.Common.Exceptions
{
    public class BillExpired : BusinessException
    {
        public readonly string ErrorCode = "20";

        public BillExpired(string code, params string[] overrides)
            : base(code, overrides)
        {
        }

    }
}