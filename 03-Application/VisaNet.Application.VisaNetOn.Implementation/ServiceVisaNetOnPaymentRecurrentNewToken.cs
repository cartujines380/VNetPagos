using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.VisaNetOn.Implementation
{
    public class ServiceVisaNetOnPaymentRecurrentNewToken : ServiceVisaNetOn
    {
        public ServiceVisaNetOnPaymentRecurrentNewToken(IServicePayment servicePayment, IServiceServiceAssosiate serviceServiceAssosiate,
            IServiceVonData serviceVonData, IServiceService serviceService, IServiceExternalNotification serviceExternalNotification, IServiceWebhookRegistration serviceWebhookRegistration)
            : base(servicePayment, serviceServiceAssosiate, serviceVonData, serviceService, serviceExternalNotification, serviceWebhookRegistration)
        {
        }

        public override ResultDto ProcessOperation(IDictionary<string, string> formData)
        {
            //PROCESAR PAGO USUARIO RECURRENTE CON NUEVA TARJETA

            string idOperation;
            string idApp;
            GetIdAppAndIdOperation(formData, out idApp, out idOperation);

            NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentRecurrentNewToken - ProcessOperation - " +
                "Procesando pago usuario recurrente con tarjeta nueva. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));

            try
            {
                //Analizar respuesta de CS
                var cybersourceResponse = ServicePayment.CybersourceAnalyze(formData);
                if (cybersourceResponse.PaymentDto != null)
                {
                    cybersourceResponse.PaymentDto.IdOperation = idOperation;

                    VonDataDto vonUserData = null;
                    if (cybersourceResponse.PaymentData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                    {
                        //Agregar tarjeta a usuario recurrente
                        vonUserData = AddCardToVONUser(cybersourceResponse);
                    }

                    //Generar pago
                    var paymentResult = GeneratePayment(cybersourceResponse);

                    if (vonUserData != null)
                    {
                        //Notificar comercio
                        var notificationOk = NotifyCommerce(paymentResult, cybersourceResponse, vonUserData, idApp, idOperation);
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
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentRecurrentNewToken - ProcessOperation - " +
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
            VonDataDto vonUserData, string idApp, string idOperation)
        {
            var notificationOk = false;
            try
            {
                if (paymentResult.NewPaymentDto != null)
                {
                    //Si se realizo el pago se notifica el pago + asociacion
                    const bool withAssociation = true;
                    paymentResult.NewPaymentDto.IdOperation = idOperation;
                    notificationOk = ServiceExternalNotification.NotifyExternalSourceNewPayment(paymentResult.NewPaymentDto, vonUserData.UserExternalId,
                        vonUserData.CardExternalId, withAssociation);
                }
                else
                {
                    //Sino solamente se notifica la asociacion
                    cybersourceResponse.PaymentDto.IdOperation = idOperation;
                    notificationOk = ServiceExternalNotification.NotifyExternalSourceNewCard(cybersourceResponse.PaymentDto, vonUserData.UserExternalId,
                        vonUserData.CardExternalId);
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentRecurrentNewToken - NotifyCommerce - " +
                    "Ha ocurrido una excepcion. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));
                NLogLogger.LogEvent(e);
            }
            return notificationOk;
        }

    }
}