using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.VisaNetOn.Implementation
{
    public class ServiceVisaNetOnPaymentRegisteredNewToken : ServiceVisaNetOn
    {
        public ServiceVisaNetOnPaymentRegisteredNewToken(IServicePayment servicePayment, IServiceServiceAssosiate serviceServiceAssosiate,
            IServiceVonData serviceVonData, IServiceService serviceService, IServiceExternalNotification serviceExternalNotification, IServiceWebhookRegistration serviceWebhookRegistration)
            : base(servicePayment, serviceServiceAssosiate, serviceVonData, serviceService, serviceExternalNotification, serviceWebhookRegistration)
        {
        }

        public override ResultDto ProcessOperation(IDictionary<string, string> formData)
        {
            //PROCESAR PAGO USUARIO REGISTRADO CON NUEVA TARJETA

            string idOperation;
            string idApp;
            GetIdAppAndIdOperation(formData, out idApp, out idOperation);

            NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentRegisteredNewToken - ProcessOperation - " +
                "Procesando pago usuario registrado con tarjeta nueva. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));

            try
            {
                //Analizar respuesta de CS
                var cybersourceResponse = ServicePayment.CybersourceAnalyze(formData);
                if (cybersourceResponse.PaymentDto != null)
                {
                    cybersourceResponse.PaymentDto.IdOperation = idOperation;

                    //Generar pago
                    var paymentResult = GeneratePayment(cybersourceResponse);

                    if (cybersourceResponse.PaymentData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                    {
                        //Notificar comercio
                        var notificationOk = NotifyCommerce(paymentResult, cybersourceResponse, idApp, idOperation);
                    }

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
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentRegisteredNewToken - ProcessOperation - " +
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
            string idApp, string idOperation)
        {
            var notificationOk = false;
            try
            {
                var serviceAssociate = ServiceServiceAssosiate.GetById(paymentResult.NewPaymentDto.ServiceAssociatedId.Value);
                var userExternalId = serviceAssociate.IdUserExternal.Value.ToString();
                var cardExternalId = cybersourceResponse.PaymentDto.Card.ExternalId.ToString();

                if (paymentResult.NewPaymentDto != null)
                {
                    //Si se realizo el pago se notifica el pago + asociacion
                    const bool withAssociation = true;
                    paymentResult.NewPaymentDto.IdOperation = idOperation;
                    notificationOk = ServiceExternalNotification.NotifyExternalSourceNewPayment(paymentResult.NewPaymentDto, userExternalId,
                        cardExternalId, withAssociation);
                }
                else if (!string.IsNullOrEmpty(cybersourceResponse.PaymentDto.Card.PaymentToken))
                {
                    //Sino solamente se notifica la asociacion
                    cybersourceResponse.PaymentDto.IdOperation = idOperation;
                    notificationOk = ServiceExternalNotification.NotifyExternalSourceNewCard(cybersourceResponse.PaymentDto, userExternalId,
                        cardExternalId);
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentRegisteredNewToken - NotifyCommerce - " +
                    "Ha ocurrido una excepcion. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));
                NLogLogger.LogEvent(e);
            }
            return notificationOk;
        }

    }
}