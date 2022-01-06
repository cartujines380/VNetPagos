using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.VisaNetOn.Implementation
{
    public class ServiceVisaNetOnTokenizationRegistered : ServiceVisaNetOn
    {
        public ServiceVisaNetOnTokenizationRegistered(IServicePayment servicePayment, IServiceServiceAssosiate serviceServiceAssosiate,
            IServiceVonData serviceVonData, IServiceService serviceService, IServiceExternalNotification serviceExternalNotification, IServiceWebhookRegistration serviceWebhookRegistration)
            : base(servicePayment, serviceServiceAssosiate, serviceVonData, serviceService, serviceExternalNotification, serviceWebhookRegistration)
        {
        }

        public override ResultDto ProcessOperation(IDictionary<string, string> formData)
        {
            //PROCESAR ASOCIACION DE USUARIO REGISTRADO EXISTENTE CON TARJETA NUEVA

            string idOperation;
            string idApp;
            GetIdAppAndIdOperation(formData, out idApp, out idOperation);

            NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnTokenizationRegistered - ProcessOperation - " +
                "Procesando asociacion de usuario registrado. IdApp: {0}, IdOperation: {1}.", idApp, idOperation));

            try
            {
                const bool isRegisteredUser = true;

                //Genera asociacion y notifica (metodo viejo que utilizaba Apps)
                var associationResponse = ServiceServiceAssosiate.ProccesDataFromCybersourceForApps(formData);

                //Procesar resultado
                return ProcessAssociationResult(associationResponse, isRegisteredUser, idApp, idOperation);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceVisaNetOnTokenizationRegistered - ProcessOperation - " +
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