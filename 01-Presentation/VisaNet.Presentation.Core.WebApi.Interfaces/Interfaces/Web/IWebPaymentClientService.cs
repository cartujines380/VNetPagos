using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebPaymentClientService
    {
        Task<ICollection<PaymentDto>> FindAll(BaseFilter filtersDto);
        Task<PaymentDto> Create(PaymentDto serviceCategory);
        Task<byte[]> DownloadTicket(Guid id, string transactionNumber, Guid userId);
        Task SendTicketByEmail(Guid id, string transactionNumber, Guid userId);
        Task CancelPaymentCybersource(CancelPayment payment);
        Task<bool> IsPaymentDoneWithServiceAssosiated(Guid id);
        Task<int> CountPaymentsDone(Guid registredUserId, Guid anonymousUserId, Guid serviceId);
        Task ReversePayment(RefundPayment reverse);
        Task NotifyError(string data);
        Task<CybersourceCreatePaymentDto> NotifyPayment(IDictionary<string, string> csDictionaryData);
        Task<ICollection<PaymentDto>> GetDataForFromList(BaseFilter filtersDto);
        Task<bool> NotifyExternalSourceNewPayment(PaymentDto paymentDto);       
    }
}