using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations
{
    public abstract class RunState
    {
        public abstract Guid ProcessHistoryId { get; set; }
        public abstract List<PendingAutomaticPaymentDto> ServicesToRetry { get; set; }
        public abstract AutomaticPaymentStatisticsDto ProcessStatistics { get; set; }
        public abstract bool ShouldPay { get; set; }
        public abstract bool ShouldNotifyUser { get; set; }
        public abstract bool ShouldNotifySystem { get; set; }

        public abstract void StartProcess();
        public abstract List<ServiceAssociatedDto> GetServices();
        public abstract void UpdateProcessHistory(bool fatalError = false);
        public abstract void SetNotificationFlag(Dictionary<Guid, PaymentResultTypeDto> billResultDictionary);

        protected abstract void SuccessResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult);
        protected abstract void ControlledErrorResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult);
        protected abstract void DeleteAutomaticPaymentControlledErrorResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult);
        protected abstract void RetryErrorResult(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult);

        public void InterpretResult(PaymentResultTypeDto serviceResult,
            ServiceAssociatedDto serviceAssociatedDto)
        {
            switch (serviceResult)
            {
                //Success 
                case PaymentResultTypeDto.Success:
                case PaymentResultTypeDto.NoBills:
                    SuccessResult(serviceAssociatedDto, serviceResult);
                    break;

                //Error controlado (no se reintenta)
                case PaymentResultTypeDto.ServiceErrorControlled:
                case PaymentResultTypeDto.AutomaticPaymentDisabled:
                case PaymentResultTypeDto.InvalidModel:
                case PaymentResultTypeDto.DiscountCalculationError:
                case PaymentResultTypeDto.BankDontAllowQuota:
                case PaymentResultTypeDto.ExceededPaymentsAmount:
                    ControlledErrorResult(serviceAssociatedDto, serviceResult);
                    break;

                //Error controlado (no se reintenta) y se elimina pago programado
                case PaymentResultTypeDto.ServiceErrorDeleteAutomaticPayment:
                case PaymentResultTypeDto.ServiceNotAllowsAutomaticPayment:
                case PaymentResultTypeDto.InvalidCardToken:
                case PaymentResultTypeDto.InvalidCardBin:
                case PaymentResultTypeDto.InvalidCardDueDate:
                case PaymentResultTypeDto.ExceededPaymentsCount:
                case PaymentResultTypeDto.BinNotValidForService:
                case PaymentResultTypeDto.CsExpiredCard:
                case PaymentResultTypeDto.CsStolenCard:
                case PaymentResultTypeDto.CsInactiveCard:
                case PaymentResultTypeDto.CsInvalidAccountNumber:
                case PaymentResultTypeDto.CsCardTypeNotAccepted:
                    DeleteAutomaticPaymentControlledErrorResult(serviceAssociatedDto, serviceResult);
                    break;

                //Error no controlado (se reintenta)
                case PaymentResultTypeDto.ServiceErrorRetry:
                case PaymentResultTypeDto.UnhandledException:
                case PaymentResultTypeDto.GetBillsException:
                case PaymentResultTypeDto.AmountIsZeroError:
                case PaymentResultTypeDto.GatewayNotificationError:
                case PaymentResultTypeDto.PaymentGeneralError:
                case PaymentResultTypeDto.CsAccountFrozen:
                case PaymentResultTypeDto.CsAccountInformationProblem:
                case PaymentResultTypeDto.CsAddressVerificationFailure:
                case PaymentResultTypeDto.CsCardCreditLimitReached:
                case PaymentResultTypeDto.CsCardGeneralDeclined:
                case PaymentResultTypeDto.CsCardInvalidCVN:
                case PaymentResultTypeDto.CsCardInvalidCVNByCybersource:
                case PaymentResultTypeDto.CsCardTypeInvalid:
                case PaymentResultTypeDto.CsCardholderAuthenticationNeeded:
                case PaymentResultTypeDto.CsCustomerDeclined:
                case PaymentResultTypeDto.CsDecisionManagerError:
                case PaymentResultTypeDto.CsGeneralDecline:
                case PaymentResultTypeDto.CsGeneralSystemFailure:
                case PaymentResultTypeDto.CsInsufficientFunds:
                case PaymentResultTypeDto.CsInvalidAccessKey:
                case PaymentResultTypeDto.CsInvalidData:
                case PaymentResultTypeDto.CsIssuiungBankRequestDeclined:
                case PaymentResultTypeDto.CsIssuiungBankUnavailable:
                case PaymentResultTypeDto.CsPartialAmount:
                case PaymentResultTypeDto.CsPayerAuthenticationFail:
                case PaymentResultTypeDto.CsProcessorFailure:
                case PaymentResultTypeDto.CsServerTimeout:
                case PaymentResultTypeDto.CsServiceTimeout:
                    RetryErrorResult(serviceAssociatedDto, serviceResult);
                    break;
            }
        }

        public static List<PaymentResultTypeDto> SuccessCodes()
        {
            var list = new List<PaymentResultTypeDto>
            {
                PaymentResultTypeDto.Success,
                PaymentResultTypeDto.NoBills,
                PaymentResultTypeDto.BillOk
            };
            return list;
        }

        public static List<PaymentResultTypeDto> RetryErrorCodes()
        {
            var list = new List<PaymentResultTypeDto>
            {
                PaymentResultTypeDto.ServiceErrorRetry,
                PaymentResultTypeDto.UnhandledException,
                PaymentResultTypeDto.AmountIsZeroError,
                PaymentResultTypeDto.GetBillsException,
                PaymentResultTypeDto.GatewayNotificationError,
                PaymentResultTypeDto.PaymentGeneralError,
                PaymentResultTypeDto.CsAccountFrozen,
                PaymentResultTypeDto.CsAccountInformationProblem,
                PaymentResultTypeDto.CsAddressVerificationFailure,
                PaymentResultTypeDto.CsCardCreditLimitReached,
                PaymentResultTypeDto.CsCardGeneralDeclined,
                PaymentResultTypeDto.CsCardInvalidCVN,
                PaymentResultTypeDto.CsCardInvalidCVNByCybersource,
                PaymentResultTypeDto.CsCardTypeInvalid,
                PaymentResultTypeDto.CsCardholderAuthenticationNeeded,
                PaymentResultTypeDto.CsCustomerDeclined,
                PaymentResultTypeDto.CsDecisionManagerError,
                PaymentResultTypeDto.CsGeneralDecline,
                PaymentResultTypeDto.CsGeneralSystemFailure,
                PaymentResultTypeDto.CsInsufficientFunds,
                PaymentResultTypeDto.CsInvalidAccessKey,
                PaymentResultTypeDto.CsInvalidData,
                PaymentResultTypeDto.CsIssuiungBankRequestDeclined,
                PaymentResultTypeDto.CsIssuiungBankUnavailable,
                PaymentResultTypeDto.CsPartialAmount,
                PaymentResultTypeDto.CsPayerAuthenticationFail,
                PaymentResultTypeDto.CsProcessorFailure,
                PaymentResultTypeDto.CsServerTimeout,
                PaymentResultTypeDto.CsServiceTimeout,
            };
            return list;
        }

        public static List<PaymentResultTypeDto> ControlledErrorCodes()
        {
            var list = new List<PaymentResultTypeDto>
            {
                PaymentResultTypeDto.ServiceErrorControlled,
                PaymentResultTypeDto.InvalidModel,
                PaymentResultTypeDto.DiscountCalculationError,
                PaymentResultTypeDto.BankDontAllowQuota,
                PaymentResultTypeDto.BinNotValidForService,
                PaymentResultTypeDto.ExceededPaymentsCount,
                PaymentResultTypeDto.ExceededPaymentsAmount,
                PaymentResultTypeDto.BillExpired,
                PaymentResultTypeDto.ServiceNotAllowsAutomaticPayment,
                PaymentResultTypeDto.AutomaticPaymentDisabled,
                PaymentResultTypeDto.InvalidCardToken,
                PaymentResultTypeDto.InvalidCardBin,
                PaymentResultTypeDto.InvalidCardDueDate,
                PaymentResultTypeDto.CsExpiredCard,
                PaymentResultTypeDto.CsStolenCard,
                PaymentResultTypeDto.CsInactiveCard,
                PaymentResultTypeDto.CsInvalidAccountNumber,
                PaymentResultTypeDto.CsCardTypeNotAccepted
            };
            return list;
        }

        public static List<PaymentResultTypeDto> DeleteAutomaticPaymentErrorCodes()
        {
            var list = new List<PaymentResultTypeDto>
            {
                PaymentResultTypeDto.ServiceErrorDeleteAutomaticPayment,
                PaymentResultTypeDto.ServiceNotAllowsAutomaticPayment,
                PaymentResultTypeDto.InvalidCardToken,
                PaymentResultTypeDto.InvalidCardBin,
                PaymentResultTypeDto.InvalidCardDueDate,
                PaymentResultTypeDto.ExceededPaymentsCount,
                PaymentResultTypeDto.BinNotValidForService,
                PaymentResultTypeDto.CsExpiredCard,
                PaymentResultTypeDto.CsStolenCard,
                PaymentResultTypeDto.CsInactiveCard,
                PaymentResultTypeDto.CsInvalidAccountNumber,
                PaymentResultTypeDto.CsCardTypeNotAccepted
            };
            return list;
        }

        public static List<PaymentResultTypeDto> CybersourceErrorCodes()
        {
            var list = new List<PaymentResultTypeDto>
            {
                PaymentResultTypeDto.CsExpiredCard,
                PaymentResultTypeDto.CsStolenCard,
                PaymentResultTypeDto.CsInactiveCard,
                PaymentResultTypeDto.CsInvalidAccountNumber,
                PaymentResultTypeDto.CsCardTypeNotAccepted,
                PaymentResultTypeDto.CsAccountFrozen,
                PaymentResultTypeDto.CsAccountInformationProblem,
                PaymentResultTypeDto.CsAddressVerificationFailure,
                PaymentResultTypeDto.CsCardCreditLimitReached,
                PaymentResultTypeDto.CsCardGeneralDeclined,
                PaymentResultTypeDto.CsCardInvalidCVN,
                PaymentResultTypeDto.CsCardInvalidCVNByCybersource,
                PaymentResultTypeDto.CsCardTypeInvalid,
                PaymentResultTypeDto.CsCardholderAuthenticationNeeded,
                PaymentResultTypeDto.CsCustomerDeclined,
                PaymentResultTypeDto.CsDecisionManagerError,
                PaymentResultTypeDto.CsGeneralDecline,
                PaymentResultTypeDto.CsGeneralSystemFailure,
                PaymentResultTypeDto.CsInsufficientFunds,
                PaymentResultTypeDto.CsInvalidAccessKey,
                PaymentResultTypeDto.CsInvalidData,
                PaymentResultTypeDto.CsIssuiungBankRequestDeclined,
                PaymentResultTypeDto.CsIssuiungBankUnavailable,
                PaymentResultTypeDto.CsPartialAmount,
                PaymentResultTypeDto.CsPayerAuthenticationFail,
                PaymentResultTypeDto.CsProcessorFailure,
                PaymentResultTypeDto.CsServerTimeout,
                PaymentResultTypeDto.CsServiceTimeout
            };
            return list;
        }

    }
}