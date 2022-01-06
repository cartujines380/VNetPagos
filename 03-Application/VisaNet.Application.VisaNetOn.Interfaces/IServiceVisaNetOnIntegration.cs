using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.VisaNetOn.Interfaces
{
    public interface IServiceVisaNetOnIntegration
    {
        ResultDto ProcessOperation(IDictionary<string, string> formData, RedirectEnums action);
        void GeneratePaymentTicket(string transactionNumber, Guid userId, out byte[] renderedBytes, out string mimeType);
        PaymentDto GetPaymentDto(string transactionNumber, string idApp);
        void SendPaymentTicketByEmail(string transactionNumber, Guid userId);
        bool CancelPayment(string trnsNumber);
    }
}