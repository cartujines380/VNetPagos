using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.VisaNetOn.Implementation
{
    public class ServiceVisaNetOnPaymentRegisteredWithToken : ServiceVisaNetOn
    {
        private readonly IServiceCard _serviceCard;

        public ServiceVisaNetOnPaymentRegisteredWithToken(IServicePayment servicePayment, IServiceServiceAssosiate serviceServiceAssosiate,
            IServiceVonData serviceVonData, IServiceService serviceService, IServiceExternalNotification serviceExternalNotification, IServiceCard serviceCard, IServiceWebhookRegistration serviceWebhookRegistration)
            : base(servicePayment, serviceServiceAssosiate, serviceVonData, serviceService, serviceExternalNotification, serviceWebhookRegistration)
        {
            _serviceCard = serviceCard;
        }

        public override ResultDto ProcessOperation(IDictionary<string, string> formData)
        {
            //PROCESAR PAGO USUARIO REGISTRADO CON TARJETA EXISTENTE

            string idOperation;
            string idApp;
            GetIdAppAndIdOperation(formData, out idApp, out idOperation);

            NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentRegisteredWithToken - ProcessOperation - " +
                "Procesando pago usuario registrado con tarjeta existente. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));

            try
            {
                //Analizar respuesta de CS
                var cybersourceResponse = ServicePayment.CybersourceAnalyze(formData);
                if (cybersourceResponse.PaymentDto != null)
                {
                    cybersourceResponse.PaymentDto.IdOperation = idOperation;

                    var integrationService = GetServiceForIntegration(cybersourceResponse.PaymentDto.ServiceId);

                    //Generar pago
                    var paymentResult = GeneratePayment(cybersourceResponse);

                    if (cybersourceResponse.PaymentData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                    {
                        //Notificar comercio
                        var notificationOk = NotifyCommerce(paymentResult, cybersourceResponse, integrationService.UrlName, idOperation);
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
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentRegisteredWithToken - ProcessOperation - " +
                    "Ha ocurrido una excepcion. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));
                NLogLogger.LogEvent(e);
            }
            return new ResultDto
            {
                ResultCode = "1",
                ResultDescription = "Error al registrar el pago."
            };
        }

        private bool NotifyCommerce(CybersourceCreatePaymentDto paymentResult, CybersourceTransactionsDataDto cybersourceResponse, string idApp, string idOperation)
        {
            var notificationOk = false;
            try
            {
                var serviceAssociate = ServiceServiceAssosiate.GetById(paymentResult.NewPaymentDto.ServiceAssociatedId.Value, x => x.Cards);
                var userExternalId = serviceAssociate.IdUserExternal.Value.ToString();
                string cardExternalId;

                bool withAssociation;
                if (cybersourceResponse.PaymentDto.Card.ExternalId == null)
                {
                    //Si la tarjeta no tiene ExternalId debo generar uno y se notifica asociacion de tarjeta
                    var updatedCard = _serviceCard.GenerateExternalId(cybersourceResponse.PaymentDto.Card.Id);
                    cardExternalId = updatedCard.ExternalId.ToString();
                    withAssociation = true;
                }
                else
                {
                    //Si ya tiene ExternalId se verifica si ya se notifico o no
                    cardExternalId = cybersourceResponse.PaymentDto.Card.ExternalId.ToString();
                    var alreadyNotifiedCard = ServiceExternalNotification.AlreadyNotifiedExternalCard(idApp, userExternalId, cardExternalId);
                    withAssociation = !alreadyNotifiedCard;
                }

                paymentResult.NewPaymentDto.IdOperation = idOperation;
                notificationOk = ServiceExternalNotification.NotifyExternalSourceNewPayment(paymentResult.NewPaymentDto, userExternalId, cardExternalId, withAssociation);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnPaymentRegisteredWithToken - NotifyCommerce - " +
                    "Ha ocurrido una excepcion. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));
                NLogLogger.LogEvent(e);
            }
            return notificationOk;
        }

    }
}