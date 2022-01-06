using System;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces
{
    public interface ILoggerHelper
    {
        //Process
        void LogProcessFinished(int runNumber);
        void LogServicesCount(int count);
        void LogActualServiceBeingProcessed(int actualServiceNumber);
        void LogFinishedProcessingServices();

        //Process runs
        void LogProcessExecutedSuccessfuly();
        void LogProcessReachedMaxTries(int maxTries);
        void LogProcessRunNumber(int timesExecuted);
        void LogNotificationsSingleProcess();
        void LogAutomaticPaymentsSingleProcess(int timesExecuted);

        //Service asociated
        void LogServiceProcessStarted(ServiceAssociatedDto serviceAssociatedDto);
        void LogServiceProcessFinished(ServiceAssociatedDto serviceAssociatedDto);
        void LogInvalidCardToken(ServiceAssociatedDto serviceAssociatedDto);
        void LogInvalidCardBin(ServiceAssociatedDto serviceAssociatedDto);
        void LogInvalidCardDueDate(ServiceAssociatedDto serviceAssociatedDto);
        void LogServiceNotAllowsAutomaticPayment(ServiceAssociatedDto serviceAssociatedDto);
        void LogAutomaticPaymentDisabled(ServiceAssociatedDto serviceAssociatedDto);
        void LogInvalidPaymentsCount(ServiceAssociatedDto serviceAssociatedDto);
        void LogFilteredBillsForServiceAssociate(int filteredBillsCount, Guid serviceAssociatedDtoId);
        void LogIdPadronForServiceAssociate(Guid serviceAssociatedDtoId, int idPadron);

        //Bill
        void LogBillExceedsQuotas(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto);
        void LogBillExceedsAmount(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto);
        void LogCalculatingDiscount(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto);
        void LogCallingCybersource(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto);
        void LogCallCybersourceSuccess(PaymentDto payment, CyberSourceDataDto paymentData);
        void LogPayBillSuccess(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto);
        void LogPayBillInvalidModel(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto);
        void LogPayBillBankDontAllowQuota(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto);
        void LogPayBillBinNotValidForService(ServiceAssociatedDto serviceAssociatedDto, BillDto billDto);
        void LogGatewayNotificationError(Guid serviceAssociatedDtoId);

        //Exceptions
        void LogGetBillsHandledException(Exception e, Guid serviceAssociatedDtoId);
        void LogGetBillsException(Exception e, Guid serviceAssociatedDtoId);
        void LogDiscountCalculationException(Exception e, ServiceAssociatedDto serviceAssociatedDto);
        void LogException(Exception e, Guid? serviceAssociatedDtoId = null);

    }
}
