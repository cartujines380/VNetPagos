using System;

namespace VisaNet.Common.Exceptions
{
    public class BillTaxedAmountNotAllow : BusinessException
    {
        public readonly string ErrorCode = "72";

        public BillTaxedAmountNotAllow(string code, params string[] overrides)
            : base(code, overrides)
        {
            
        }
        
    }
}
