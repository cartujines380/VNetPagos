namespace VisaNet.Common.Exceptions
{
    public class BillAlreadyPaid : BusinessException
    {
        public readonly string ErrorCode = "8";

        public BillAlreadyPaid(string code, params string[] overrides)
            : base(code, overrides)
        {
        }

    }
}