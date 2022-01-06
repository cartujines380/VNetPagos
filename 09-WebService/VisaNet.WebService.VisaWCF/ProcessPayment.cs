using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.WebService;
using VisaNet.WebService.VisaWCF.EntitiesDto;
using VisaNet.WebService.VisaWCF.Mappers;

namespace VisaNet.WebService.VisaWCF
{
    public interface IProcessPayment
    {
        PaymentResponse Process(IWsBankClientService wsBankClientService, PaymentData data);
    }

    public class ProcessPayment : IProcessPayment
    {
        public PaymentResponse Process(IWsBankClientService wsBankClientService, PaymentData data)
        {
            PaymentResponse response;

            var payment = wsBankClientService.Payment(data.ToPaymentDto()).GetAwaiter().GetResult();

            if (payment.ErrorCode == ErrorCodeDto.OK)
            {
                response = new PaymentResponse(VisaNetAccessResponseCode.Ok) { Response = payment.ToVisaNetPaymentResult(data) };
            }
            else
            {
                response = new PaymentResponse(VisaNetAccessResponseCode.Error) { ResponseError = new ResponseError { ResponseText = payment.ErrorMessage, ResponseType = (int)payment.ErrorCode }, CyberSourceData = payment.CyberSourceOperationData.ToVisaNetCyberSourceData() };
            }

            return response;
        }
    }

    public class ProcessReverse : IProcessPayment
    {
        public PaymentResponse Process(IWsBankClientService wsBankClientService, PaymentData data)
        {
            var result = wsBankClientService.ReversePayment(data.ToReverseDto()).GetAwaiter().GetResult();

            return new PaymentResponse(VisaNetAccessResponseCode.Error)
            {
                ResponseError = new ResponseError { ResponseText = ProcessReverseMessage(result.CyberSourceOperationData.ReversalData), ResponseType = (int)ErrorCodeDto.DECISIONMANAGER },
                CyberSourceData = result.CyberSourceOperationData.ToVisaNetCyberSourceData()
            };
        }

        private string ProcessReverseMessage(CsResponseData reversalData)
        {
            string response = "Cyber Source devolvió un error 481. Se pudo hacer correctamente un reverso de la transacción.";
            if (reversalData == null)
            {
                response = "Cyber Source devolvió un error 481. Hubo una excepción al hacer un reverso de la transacción.";
            }
            else
            {
                if (reversalData.PaymentResponseCode != (int)ErrorCodeDto.CYBERSOURCE_OK)
                {
                    response = "Cyber Source devolvió un error 481. Cyber Source devolvió un error al hacer un reverso de la transacción";
                }
            }
            return response;
        }
    }

    public class ProcessError : IProcessPayment
    {
        public PaymentResponse Process(IWsBankClientService wsBankClientService, PaymentData data)
        {
            return new PaymentResponse(VisaNetAccessResponseCode.Error) { ResponseError = new ResponseError { ResponseText = "Error General de CyberSource.", ResponseType = Convert.ToInt32(data.CyberSourceData.ReasonCode) } };
        }
    }
}