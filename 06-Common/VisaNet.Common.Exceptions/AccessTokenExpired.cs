namespace VisaNet.Common.Exceptions
{
    public class AccessTokenExpired : BusinessException
    {
        public readonly string ErrorCode = "19";

        public AccessTokenExpired(string code, params string[] overrides)
            : base(code, overrides)
        {
        }

    }
}