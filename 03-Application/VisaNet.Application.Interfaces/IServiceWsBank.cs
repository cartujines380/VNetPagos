using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceWsBank
    {
        IEnumerable<ServiceDto> AllServices();
        WsBankBillsResultDto GetBills(WsBankBillsInputDto dto);
        WsBankPaymentResultDto Payment(WsBankPaymentInputDto dto);
        WsBankReverseResultDto ReversePayment(WsBankReverseInputDto dto);
        WsBankBillsResultDto PreprocessPayment(WsBankPreprocessPaymentInputDto dto);
        List<CyberSourceExtraDataDto> CalculateDiscount(WsBankPreprocessPaymentInputDto dto);
        IEnumerable<PaymentDto> GetPayments(WsBankSearchPaymentsInputDto dto);
    }
}
