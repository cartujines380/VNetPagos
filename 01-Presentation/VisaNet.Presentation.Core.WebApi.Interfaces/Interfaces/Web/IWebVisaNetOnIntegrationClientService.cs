using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebVisaNetOnIntegrationClientService
    {
        Task<ResultDto> ProcessOperation(ProcessOperationDto processOperationDto);
        Task<VonDataDto> FindVonData(string idApp, string idUserExternal, string idCardExternal);
        Task<VonDataDto> FindVonData(string idApp, Guid anonymousUserId, string idCardExternal);
        Task<ICollection<VonDataDto>> FindVonData(string idApp, string idUserExternal);
        Task<ICollection<VonDataDto>> FindVonData(string idApp, Guid anonymousUserId);

        Task<byte[]> DownloadTicket(string transactionNumber, Guid userId);
        Task<PaymentDto> GetPaymentDto(string transactionNumber, string idApp);
        Task SendPaymentTicketByEmail(string transactionNumber, Guid userId);
        Task<bool> CancelPayment(string transactionNumber);
    }
}