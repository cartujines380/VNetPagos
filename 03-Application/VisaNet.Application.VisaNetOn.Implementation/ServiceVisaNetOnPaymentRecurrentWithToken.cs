using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.VisaNetOn.Implementation
{
    public class ServiceVisaNetOnPaymentRecurrentWithToken : ServiceVisaNetOn
    {
        public ServiceVisaNetOnPaymentRecurrentWithToken(IServicePayment servicePayment, IServiceServiceAssosiate serviceServiceAssosiate,
            IServiceVonData serviceVonData, IServiceService serviceService, IServiceExternalNotification serviceExternalNotification, IServiceWebhookRegistration serviceWebhookRegistration)
            : base(servicePayment, serviceServiceAssosiate, serviceVonData, serviceService, serviceExternalNotification, serviceWebhookRegistration)
        {
        }

        public override ResultDto ProcessOperation(IDictionary<string, string> formData)
        {
            //PROCESAR PAGO USUARIO RECURRENTE CON TARJETA EXISTENTE

            string idOperation;
            string idApp;
            GetIdAppAndIdOperation(formData, out idApp, out idOperation);

            NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentRecurrentWithToken - ProcessOperation - " +
                "Procesando pago usuario recurrente con tarjeta existente. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));

            try
            {
                //Analizar respuesta de CS
                var cybersourceResponse = ServicePayment.CybersourceAnalyze(formData);
                if (cybersourceResponse.PaymentDto != null)
                {
                    cybersourceResponse.PaymentDto.IdOperation = idOperation;

                    //Generar pago
                    var paymentResult = GeneratePayment(cybersourceResponse);

                    //Notificar comercio
                    var notificationOk = NotifyCommerce(paymentResult, cybersourceResponse, formData, idApp, idOperation);

                    //Procesar resultado
                    return ProcessPaymentResult(paymentResult, idApp, idOperation);
                }
                if (cybersourceResponse.PaymentData != null)
                {
                    return new ResultDto
                    {
                        ResultCode = cybersourceResponse.PaymentData.PaymentResponseCode.ToString(),
                        ResultDescription = cybersourceResponse.PaymentData.PaymentResponseMsg,
                    };
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentRecurrentWithToken - ProcessOperation - " +
                    "Ha ocurrido una excepcion. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));
                NLogLogger.LogEvent(e);
            }
            return new ResultDto
            {
                ResultCode = "1",
                ResultDescription = "Error al registrar el pago."
            };
        }

        private bool NotifyCommerce(CybersourceCreatePaymentDto paymentResult, CybersourceTransactionsDataDto cybersourceResponse,
            IDictionary<string, string> formData, string idApp, string idOperation)
        {
            var notificationOk = false;
            try
            {
                if (paymentResult.NewPaymentDto != null)
                {
                    //Si se realizo el pago se notifica
                    const bool withAssociation = false;
                    var cardExternalId = formData["req_merchant_defined_data32"];
                    var vonUserData = ServiceVonData.Find(idApp, cybersourceResponse.PaymentDto.AnonymousUserId.Value, cardExternalId);
                    paymentResult.NewPaymentDto.IdOperation = idOperation;

                    notificationOk = ServiceExternalNotification.NotifyExternalSourceNewPayment(paymentResult.NewPaymentDto, vonUserData.UserExternalId,
                        vonUserData.CardExternalId, withAssociation);
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentRecurrentWithToken - NotifyCommerce - " +
                    "Ha ocurrido una excepcion. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));
                NLogLogger.LogEvent(e);
            }
            return notificationOk;
        }

    }
}