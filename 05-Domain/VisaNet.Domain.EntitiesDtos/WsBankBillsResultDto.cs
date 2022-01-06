using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class WsBankBillsResultDto
    {
        public ErrorCodeDto ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public string MerchantId { get; set; }

        public string ServiceType { get; set; }

        public bool MultipleBillsAllowed { get; set; }

        public List<BillDto> Bills { get; set; }

        public WsBankBillsResultDto(ErrorCodeDto errorCode)
        {
            ErrorCode = errorCode;
        }
    }
}
