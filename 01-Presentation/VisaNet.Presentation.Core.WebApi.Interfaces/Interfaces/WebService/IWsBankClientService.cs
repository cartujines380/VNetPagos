using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.WebService
{
    public interface IWsBankClientService
    {
        Task<ICollection<ServiceDto>> AllServices();
        Task<WsBankBillsResultDto> GetBills(WsBankBillsInputDto dto);
        Task<WsBankPaymentResultDto> Payment(WsBankPaymentInputDto dto);
        Task<WsBankReverseResultDto> ReversePayment(WsBankReverseInputDto dto);
        Task<WsBankBillsResultDto> PreprocessPayment(WsBankPreprocessPaymentInputDto dto);
        Task<List<CyberSourceExtraDataDto>> CalculateDiscount(WsBankPreprocessPaymentInputDto dto);
        Task<ICollection<PaymentDto>> GetPayments(WsBankSearchPaymentsInputDto dto);
    }
}
