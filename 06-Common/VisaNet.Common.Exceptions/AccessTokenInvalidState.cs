namespace VisaNet.Common.Exceptions
{
    public class AccessTokenInvalidState : BusinessException
    {
        public readonly string ErrorCode = "18";

        public AccessTokenInvalidState(string code, params string[] overrides)
            : base(code, overrides)
        {
        }

    }
}