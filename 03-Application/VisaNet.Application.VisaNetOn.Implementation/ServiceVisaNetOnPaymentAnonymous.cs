using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.VisaNetOn.Implementation
{
    public class ServiceVisaNetOnPaymentAnonymous : ServiceVisaNetOn
    {
        public ServiceVisaNetOnPaymentAnonymous(IServicePayment servicePayment, IServiceServiceAssosiate serviceServiceAssosiate,
            IServiceVonData serviceVonData, IServiceService serviceService, IServiceExternalNotification serviceExternalNotification, IServiceWebhookRegistration serviceWebhookRegistration)
            : base(servicePayment, serviceServiceAssosiate, serviceVonData, serviceService, serviceExternalNotification, serviceWebhookRegistration)
        {
        }

        public override ResultDto ProcessOperation(IDictionary<string, string> formData)
        {
            //PROCESAR PAGO ANONIMO

            string idOperation;
            string idApp;
            GetIdAppAndIdOperation(formData, out idApp, out idOperation);

            NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentAnonymous - ProcessOperation - " +
                "Procesando pago anonimo. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));

            try
            {
                var paymentResult = ServicePayment.NotifyGateways(formData);

                //Se controla que se haya realizado el pago para notificar
                if (paymentResult.NewPaymentDto != null && paymentResult.CyberSourceOperationData.PaymentData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                {
                    paymentResult.NewPaymentDto.IdOperation = idOperation;
                    var notificationOk = ServiceExternalNotification.NotifyExternalSourceNewPaymentAnonymous(paymentResult.NewPaymentDto);
                }
                
                return ProcessPaymentResult(paymentResult, idApp, idOperation);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentAnonymous - ProcessOperation - " +
                    "Ha ocurrido una excepcion. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));
                NLogLogger.LogEvent(e);
                return new ResultDto
                {
                    ResultCode = "1",
                    ResultDescription = "Error al registrar el pago."
                };
            }
        }

    }
}