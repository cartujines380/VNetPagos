using System;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServicePaymentTicket
    {
        void GeneratePaymentTicket(string transactionNumber, Guid userId, out byte[] renderedBytes, out string mimeType);
    }
}