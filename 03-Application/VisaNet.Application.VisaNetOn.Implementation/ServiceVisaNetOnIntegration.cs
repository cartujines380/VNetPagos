using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Application.VisaNetOn.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.VisaNetOn.Implementation
{
    public class ServiceVisaNetOnIntegration : IServiceVisaNetOnIntegration
    {
        private readonly IServiceVisaNetOnFactory _serviceVisaNetOnFactory;
        private readonly IServicePaymentTicket _servicePaymentTicket;
        private readonly IServicePayment _servicePayment;

        public ServiceVisaNetOnIntegration(IServiceVisaNetOnFactory serviceVisaNetOnFactory, IServicePaymentTicket servicePaymentTicket, IServicePayment servicePayment)
        {
            _serviceVisaNetOnFactory = serviceVisaNetOnFactory;
            _servicePaymentTicket = servicePaymentTicket;
            _servicePayment = servicePayment;
        }

        public ResultDto ProcessOperation(IDictionary<string, string> formData, RedirectEnums action)
        {
            var serviceVisaNetOn = _serviceVisaNetOnFactory.GetVisaNetOnService(action);
            return serviceVisaNetOn.ProcessOperation(formData);
        }

        public void GeneratePaymentTicket(string transactionNumber, Guid userId, out byte[] renderedBytes, out string mimeType)
        {
            try
            {
                _servicePaymentTicket.GeneratePaymentTicket(transactionNumber, userId, out renderedBytes, out mimeType);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnIntegration - GeneratePaymentTicket - Excepcion. transactionNumber {0}", transactionNumber));
                NLogLogger.LogEvent(exception);

                renderedBytes = new byte[] { };
                mimeType = null;
            }
        }

        public PaymentDto GetPaymentDto(string transactionNumber, string idApp)
        {
            var paymentDto = _servicePayment.AllNoTracking(null, x => x.TransactionNumber.Equals(transactionNumber) &&
                                                                   (x.Service.UrlName.Equals(idApp) ||
                                                                    (x.Service.ServiceContainer != null &&
                                                                     x.Service.ServiceContainer.UrlName.Equals(idApp))),
                                                                     x => x.Service, x => x.Service.ServiceContainer, x => x.AnonymousUser, x => x.RegisteredUser,
                                                                     x => x.Bills, x => x.DiscountObj, x => x.Card).FirstOrDefault();
            return paymentDto;
        }

        public void SendPaymentTicketByEmail(string transactionNumber, Guid userId)
        {
            try
            {
                var paymentList = _servicePayment.AllNoTracking(null, x => x.TransactionNumber.Equals(transactionNumber));
                var payment = paymentList.FirstOrDefault();

                if (userId != Guid.Empty)
                {
                    userId = payment.RegisteredUserId.HasValue
                        ? payment.RegisteredUserId.Value
                        : payment.AnonymousUserId.Value;
                }

                _servicePayment.SendPaymentTicketByEmail(payment.Id, transactionNumber, userId);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnIntegration - SendPaymentTicketByEmail - Excepcion. transactionNumber {0}", transactionNumber));
                NLogLogger.LogEvent(exception);
                throw;
            }
        }

        public bool CancelPayment(string trnsNumber)
        {
            var csOperationData = _servicePayment.CancelPaymentDone(Guid.Empty, trnsNumber, false);
            if (csOperationData.VoidData != null)
            {
                if (csOperationData.VoidData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                {
                    return true;
                }
                if (csOperationData.VoidData.PaymentResponseCode != (int)CybersourceMsg.Accepted)
                {
                    //SI NO SE HIZO EL VOID, SE INTENTA EL REFUND
                    if (csOperationData.RefundData.PaymentResponseCode == (int)CybersourceMsg.Accepted)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
        
    }
}