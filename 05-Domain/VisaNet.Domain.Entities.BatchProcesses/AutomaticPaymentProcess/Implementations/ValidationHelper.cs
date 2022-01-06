using System;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations
{
    public class ValidationHelper : IValidationHelper
    {
        private readonly ILoggerHelper _loggerHelper;
        private readonly IServiceBin _serviceBin;

        public ValidationHelper(ILoggerHelper loggerHelper, IServiceBin serviceBin)
        {
            _loggerHelper = loggerHelper;
            _serviceBin = serviceBin;
        }

        public PaymentResultTypeDto ValidateService(ServiceAssociatedDto serviceAssociatedDto)
        {
            PaymentResultTypeDto result;

            //Primero se verifica si tiene pago programado
            if (!IsAutomaticPaymentEnabled(serviceAssociatedDto))
            {
                result = PaymentResultTypeDto.AutomaticPaymentDisabled;
            }
            else
            {
                result = IsValidCard(serviceAssociatedDto);
                if (result == PaymentResultTypeDto.Success)
                {
                    if (!ServiceAllowsAutomaticPayment(serviceAssociatedDto))
                    {
                        result = PaymentResultTypeDto.ServiceNotAllowsAutomaticPayment;
                    }
                    else if (!IsValidPaymentsCount(serviceAssociatedDto))
                    {
                        result = PaymentResultTypeDto.ExceededPaymentsCount;
                    }
                }
            }
            return result;
        }

        private PaymentResultTypeDto IsValidCard(ServiceAssociatedDto serviceAssociatedDto)
        {
            var cardResult = PaymentResultTypeDto.Success;
            if (!ValidCardToken(serviceAssociatedDto))
            {
                cardResult = PaymentResultTypeDto.InvalidCardToken;
            }
            else if (!ValidCardBin(serviceAssociatedDto))
            {
                cardResult = PaymentResultTypeDto.InvalidCardBin;
            }
            else if (!ValidCardDueDate(serviceAssociatedDto))
            {
                cardResult = PaymentResultTypeDto.InvalidCardDueDate;
            }
            return cardResult;
        }

        private bool ValidCardToken(ServiceAssociatedDto serviceAssociatedDto)
        {
            if (serviceAssociatedDto.DefaultCard == null || (serviceAssociatedDto.DefaultCard != null && string.IsNullOrEmpty(serviceAssociatedDto.DefaultCard.PaymentToken)))
            {
                _loggerHelper.LogInvalidCardToken(serviceAssociatedDto);
                return false;
            }
            return true;
        }

        private bool ValidCardBin(ServiceAssociatedDto serviceAssociatedDto)
        {
            if (serviceAssociatedDto.DefaultCard != null)
            {
                var binNumber = 0;
                if (Int32.TryParse(serviceAssociatedDto.DefaultCard.MaskedNumber.Substring(0, 6), out binNumber) == false)
                {
                    _loggerHelper.LogInvalidCardBin(serviceAssociatedDto);
                    return false;
                }
                var bin = _serviceBin.Find(binNumber);
                if (bin != null && !bin.Active)
                {
                    _loggerHelper.LogInvalidCardBin(serviceAssociatedDto);
                    return false;
                }
            }

            return true;
        }

        private bool ValidCardDueDate(ServiceAssociatedDto serviceAssociatedDto)
        {
            if (serviceAssociatedDto.DefaultCard != null && serviceAssociatedDto.DefaultCard.DueDate < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
            {
                _loggerHelper.LogInvalidCardDueDate(serviceAssociatedDto);
                return false;
            }
            return true;
        }

        private bool IsAutomaticPaymentEnabled(ServiceAssociatedDto serviceAssociatedDto)
        {
            if (!serviceAssociatedDto.AutomaticPaymentDtoId.HasValue ||
                serviceAssociatedDto.AutomaticPaymentDtoId.Value.Equals(Guid.Empty))
            {
                _loggerHelper.LogAutomaticPaymentDisabled(serviceAssociatedDto);
                return false;
            }
            return true;
        }

        private bool ServiceAllowsAutomaticPayment(ServiceAssociatedDto serviceAssociatedDto)
        {
            if (!serviceAssociatedDto.ServiceDto.EnableAutomaticPayment)
            {
                _loggerHelper.LogServiceNotAllowsAutomaticPayment(serviceAssociatedDto);
                return false;
            }
            return true;
        }

        private bool IsValidPaymentsCount(ServiceAssociatedDto serviceAssociatedDto)
        {
            if (!serviceAssociatedDto.AutomaticPaymentDto.UnlimitedQuotas)
            {
                if (serviceAssociatedDto.AutomaticPaymentDto.QuotasDone > serviceAssociatedDto.AutomaticPaymentDto.Quotas)
                {
                    _loggerHelper.LogInvalidPaymentsCount(serviceAssociatedDto);
                    return false;
                }
                return true;
            }
            return true;
        }

    }
}
