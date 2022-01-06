using System;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Testing.AutomaticPayment
{
    public class TestLoggerHelper : ILoggerHelper
    {
        public void LogProcessFinished(int runNumber)
        {
        }

        public void LogServicesCount(int count)
        {
        }

        public void LogActualServiceBeingProcessed(int actualServiceNumber)
        {
        }

        public void LogFinishedProcessingServices()
        {
        }

        public void LogProcessExecutedSuccessfuly()
        {
        }

        public void LogProcessReachedMaxTries(int maxTries)
        {
        }

        public void LogProcessRunNumber(int timesExecuted)
        {
        }

        public void LogNotificationsSingleProcess()
        {
        }

        public void LogAutomaticPaymentsSingleProcess(int timesExecuted)
        {
        }

        public void LogServiceProcessStarted(ServiceAssociatedDto serviceAssociatedDto)
        {
        }

        public void LogServiceProcessFinished(ServiceAssociatedDto serviceAssociatedDto)
        {
        }

        public void LogInvalidCardToken(ServiceAssociatedDto serviceAssociatedDto)
        {
        }

        public void LogInvalidCardBin(ServiceAssociatedDto serviceAssociatedDto)
        {
        }

        public void LogInvalidCardDueDate(ServiceAssociatedDto serviceAssociatedDto)
        {
        }

        public void LogServiceNotAllowsAutomaticPayment(ServiceAssociatedDto serviceAssociatedDto)
        {
        }

        public void LogAutomaticPaymentDisabled(ServiceAssociatedDto serviceAssociatedDto)
        {
        }

        public void LogInvalidPaymentsCount(ServiceAssociatedDto serviceAssociatedDto)
        {
        }

        public void LogFilteredBillsForServiceAssociate(int filteredBillsCount, Guid serviceAssociatedDtoId)
        {
        }

        public void LogIdPadronForServiceAssociate(Guid serviceAssociatedDtoId, int idPadron)
        {
        }

        public void LogBillExceedsQuotas(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
        }

        public void LogBillExceedsAmount(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
        }

        public void LogCalculatingDiscount(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
        }

        public void LogCallingCybersource(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
        }

        public void LogCallCybersourceSuccess(PaymentDto payment, CyberSourceDataDto paymentData)
        {
        }

        public void LogPayBillSuccess(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
        }

        public void LogPayBillInvalidModel(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
        }

        public void LogPayBillBankDontAllowQuota(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
        }

        public void LogPayBillBinNotValidForService(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto)
        {
        }

        public void LogGatewayNotificationError(Guid serviceAssociatedDtoId)
        {
        }

        public void LogGetBillsHandledException(Exception e, Guid serviceAssociatedDtoId)
        {
        }

        public void LogGetBillsException(Exception e, Guid serviceAssociatedDtoId)
        {
        }

        public void LogDiscountCalculationException(Exception e, ServiceAssociatedDto serviceAssociatedDto)
        {
            throw new NotImplementedException();
        }

        public void LogException(Exception e, Guid? serviceAssociatedDtoId = null)
        {
        }

    }
}
