using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.VisaNetOn.Implementation
{
    public class ServiceVisaNetOnPaymentNewUser : ServiceVisaNetOn
    {
        public ServiceVisaNetOnPaymentNewUser(IServicePayment servicePayment, IServiceServiceAssosiate serviceServiceAssosiate,
            IServiceVonData serviceVonData, IServiceService serviceService, IServiceExternalNotification serviceExternalNotification, IServiceWebhookRegistration serviceWebhookRegistration)
            : base(servicePayment, serviceServiceAssosiate, serviceVonData, serviceService, serviceExternalNotification, serviceWebhookRegistration)
        {
        }

        public override ResultDto ProcessOperation(IDictionary<string, string> formData)
        {
            //PROCESAR PAGO USUARIO NUEVO QUE DESEA SER RECURRENTE

            string idOperation;
            string idApp;
            GetIdAppAndIdOperation(formData, out idApp, out idOperation);

            NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentNewUser - ProcessOperation - " +
                "Procesando pago nuevo usuario recurrente. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));

            var result = new ResultDto
            {
                ResultCode = "1",
                ResultDescription = "Error al registrar el pago."
            };

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
                        //Registrar usuario recurrente
                        vonUserData = RegisterVONUser(cybersourceResponse);
                        cybersourceResponse.PaymentDto.IdUserExternal = vonUserData.UserExternalId;
                    }

                    //Generar pago
                    var paymentResult = GeneratePayment(cybersourceResponse);

                    if (vonUserData != null)
                    {
                        //Notificar comercio
                        var notificationOk = NotifyCommerce(paymentResult, cybersourceResponse, vonUserData, idApp, idOperation);
                    }
                    //Procesar resultado
                    result = ProcessPaymentResult(paymentResult, idApp, idOperation);
                    return result;
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
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentNewUser - ProcessOperation - " +
                    "Ha ocurrido una excepcion. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));
                NLogLogger.LogEvent(e);
                result.ResultCode = "1";
                result.ResultDescription = "Error al registrar el pago.";
            }
            return result;
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
                    notificationOk = ServiceExternalNotification.NotifyExternalSourceNewAssociation(cybersourceResponse.PaymentDto, vonUserData.UserExternalId,
                        vonUserData.CardExternalId);
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentNewUser - NotifyCommerce - " +
                    "Ha ocurrido una excepcion. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));
                NLogLogger.LogEvent(e);
            }
            return notificationOk;
        }

    }
}