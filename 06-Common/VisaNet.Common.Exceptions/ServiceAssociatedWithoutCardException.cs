using System;

namespace VisaNet.Common.Exceptions
{
    public class ServiceAssociatedWithoutCardException : BusinessException
    {
        public readonly string ErrorCode = "51";

        public ServiceAssociatedWithoutCardException(string code, params string[] overrides)
            : base(code, overrides)
        {
            
        }
        
    }
}
