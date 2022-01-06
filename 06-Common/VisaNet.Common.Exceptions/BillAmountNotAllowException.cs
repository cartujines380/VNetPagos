using System;

namespace VisaNet.Common.Exceptions
{
    public class BillAmountNotAllowException : BusinessException
    {
        public readonly string ErrorCode = "71";

        public BillAmountNotAllowException(string code, params string[] overrides)
            : base(code, overrides)
        {
            
        }
        
    }
}
