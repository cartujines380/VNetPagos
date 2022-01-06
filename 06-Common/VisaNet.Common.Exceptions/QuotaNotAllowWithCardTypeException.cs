using System;

namespace VisaNet.Common.Exceptions
{
    public class QuotaNotAllowWithCardTypeException : BusinessException
    {
        public readonly string ErrorCode = "70";

        public QuotaNotAllowWithCardTypeException(string code, params string[] overrides) : base(code, overrides)
        {
            
        }
        
    }
}
