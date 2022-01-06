using System;

namespace VisaNet.Common.Exceptions
{
    public class QuotaNotAllowInBankException : BusinessException
    {
        public readonly string ErrorCode = "68";

        public QuotaNotAllowInBankException(string code, params string[] overrides)
            : base(code, overrides)
        {
            
        }
        
    }
}
