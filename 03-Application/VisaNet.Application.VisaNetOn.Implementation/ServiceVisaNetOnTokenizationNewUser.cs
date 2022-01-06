using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.VisaNetOn.Implementation
{
    public class ServiceVisaNetOnTokenizationNewUser : ServiceVisaNetOn
    {
        public ServiceVisaNetOnTokenizationNewUser(IServicePayment servicePayment, IServiceServiceAssosiate serviceServiceAssosiate,
            IServiceVonData serviceVonData, IServiceService serviceService, IServiceExternalNotification serviceExternalNotification, IServiceWebhookRegistration serviceWebhookRegistration)
            : base(servicePayment, serviceServiceAssosiate, serviceVonData, serviceService, serviceExternalNotification, serviceWebhookRegistration)
        {
        }

        public override ResultDto ProcessOperation(IDictionary<string, string> formData)
        {
            //PROCESAR ASOCIACION DE NUEVO USUARIO RECURRENTE Y TARJETA NUEVA

            string idOperation;
            string idApp;
            GetIdAppAndIdOperation(formData, out idApp, out idOperation);

            NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnTokenizationNewUser - ProcessOperation - " +
                "Procesando asociacion de nuevo usuario recurrente. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));

            try
            {
                //Analizar respuesta de CS
                var cybersourceResponse = ServicePayment.CybersourceAnalyze(formData);
                if (cybersourceResponse.PaymentDto != null)
                {
                    const bool isNewUser = true;
                    const bool isRegisteredUser = false;
                    cybersourceResponse.PaymentDto.IdOperation = idOperation;

                    //Generar asociacion
                    var associationTuple = GenerateAssociationForRecurrentUser(cybersourceResponse, isNewUser);
                    var associationResponse = associationTuple.Item1;
                    var vonUserData = associationTuple.Item2;

                    //Notificar comercio
                    var notificationOk = ServiceExternalNotification.NotifyExternalSourceNewAssociation(cybersourceResponse.PaymentDto, vonUserData.UserExternalId,
                        vonUserData.CardExternalId);

                    //Procesar resultado
                    return ProcessAssociationResult(associationResponse, isRegisteredUser, idApp, idOperation);
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnTokenizationNewUser - ProcessOperation - " +
                    "Ha ocurrido una excepcion. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));
                NLogLogger.LogEvent(e);
            }
            return new ResultDto
            {
                ResultCode = "1",
                ResultDescription = "Error general."
            };
        }

    }
}