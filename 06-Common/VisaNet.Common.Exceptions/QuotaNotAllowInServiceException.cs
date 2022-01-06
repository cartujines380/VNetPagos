using System;

namespace VisaNet.Common.Exceptions
{
    public class QuotaNotAllowInServiceException : BusinessException
    {
        public readonly string ErrorCode = "69";

        public QuotaNotAllowInServiceException(string code, params string[] overrides)
            : base(code, overrides)
        {
            
        }
        
    }
}
