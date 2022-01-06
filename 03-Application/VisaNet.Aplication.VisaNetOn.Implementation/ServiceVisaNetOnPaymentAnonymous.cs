using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Aplication.VisaNetOn.Implementation
{
    public class ServiceVisaNetOnPaymentAnonymous : ServiceVisaNetOn
    {
        private readonly IServicePayment _servicePayment;

        public ServiceVisaNetOnPaymentAnonymous(IServicePayment servicePayment)
        {
            _servicePayment = servicePayment;
        }

        public override void ProcessOperation(IDictionary<string, string> formData)
        {
            //Procesar pago anonimo
            NLogLogger.LogEvent(NLogType.Info, "ServiceVisaNetOnPaymentAnonymous - ProcessOperation - Procesando pago anonimo.");
            var paymentResult = _servicePayment.NotifyGateways(formData);
            if (paymentResult.CyberSourceOperationData.PaymentData != null)
            {
                if (paymentResult.CyberSourceOperationData.PaymentData.PaymentResponseCode == (int)ErrorCodeDto.CYBERSOURCE_OK)
                {
                    if (paymentResult.NewPaymentDto == null)
                    {
                        //Ocurrio un error
                        NLogLogger.LogEvent(NLogType.Info, "ServiceVisaNetOnPaymentAnonymous - ProcessOperation - " +
                            "Ocurrio un error al registrar el pago (NewPaymentDto null).");
                        throw new Exception("Ocurrió un error al registrar el pago.");
                    }
                }
                else
                {
                    //Ocurrio un error
                    NLogLogger.LogEvent(NLogType.Info, "ServiceVisaNetOnPaymentAnonymous - ProcessOperation - " +
                        "Ocurrio un error al registrar el pago (PaymentResponseCode: " + paymentResult.CyberSourceOperationData.PaymentData.PaymentResponseCode + ").");
                    throw new Exception("Ocurrió un error al registrar el pago.");
                }
            }
            else
            {
                //Ocurrio un error
                NLogLogger.LogEvent(NLogType.Info, "ServiceVisaNetOnPaymentAnonymous - ProcessOperation - " +
                    "Ocurrio un error al registrar el pago (PaymentData null).");
                throw new Exception("Ocurrió un error al registrar el pago.");
            }
        }

    }
}