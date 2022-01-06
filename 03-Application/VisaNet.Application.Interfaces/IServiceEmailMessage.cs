using System;
using System.Collections.Generic;
using System.Net;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Security.Entities.Security;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceEmailMessage : IService<EmailMessage, EmailMessageDto>
    {
        void SendNewUserEmail(ApplicationUser user, MembershipUser membershipUser);
        void SendNewUserRequestPassword(ApplicationUser user, MembershipUser membershipUser, string url = null);
        void SendNewBill(ApplicationUserDto user, ServiceDto service, string billId, string billDate, string billAmount);
        void SendBillExpired(ApplicationUserDto user, ServiceDto service, BillDto bill, string billAmount);
        void SendBillAboutToExpired(ApplicationUserDto user, ServiceDto service, BillDto bill, string billAmount);
        void SendResetPassword(ApplicationUser user, MembershipUser membershipUser, string url = null);
        void SendContactFormEmail(Contact contact, Parameters parameter);
        void SendNewPayment(bool isAnonymousUser, string transactionNumber, AnonymousUserDto anoymousUser, ApplicationUserDto user, string serviceNameAndDescription, byte[] arrBytes, string mimeType);
        void SendCopyPayment(bool isAnonymousUser, string transactionNumber, AnonymousUserDto anoymousUser, ApplicationUserDto user, byte[] arrBytes, string mimeType);
        void SendServiceDeletedNotification(string serviceName, ApplicationUser user);
        void SendExpiringCard(ApplicationUserDto user, string maskedNumber, DateTime dueDate, string message);
        void SendPaymentCancellationError(string userEmail, Guid userId, LogUserType userType, Parameters parameter, string requestId, DateTime creationDateTime);
        void SendAutomaticPaymentNotification(AutomaticPaymentStatisticsDto processStatistics);
        void SendUserAutomaticPaymentNotification(ApplicationUserDto user, bool serviceHasAutomaticPayment, string serviceAssociatedName, IDictionary<Guid, ProcessedBillResultDto> processedBillResults, string validationMessage);
        void SendBinNotDefined(Parameters parameter, string bin);
        void SendBinFileProcessed(Parameters parameter, BinFileProcessResultDto result);
        void SendGeneralNotification(ApplicationUserDto user, string title, string message);
        void SendInternalErrorNotification(Parameters parameter, string title, object user, string message, string exceptionMessage, string stackTrace, Exception innerException);
        void SendInternalGeneralNotification(Parameters parameter, string title, string message);
        void SendCybersourceError(Parameters parameter, string title, string message);
        void SendPaymentDoneCancellation(string userEmail, string mailDescription, string originalTransaction, string date, string totalAmount, string cancellationTransaction, string dateCancellation, string totalAmountCancellation);
        int GetEmailsForTableCount(ReportsEmailsFilterDto filterDto);
        ICollection<EmailMessageDto> GetEmailsForTable(ReportsEmailsFilterDto filterDto);
        void ResendEmail(Guid id);
        void CancelEmail(Guid id);
        void GenerateAttachment(Guid emailId, out FileDto file);
        /// <summary>
        /// 
        /// </summary>
        /// <returns>0 = no se envio ni se guardo en BD, 1 = solo se envio, 2 = se envio y se persistio</returns>
        int SendExtract(EmailType type, string subject, string text, string to, string path = null, string fileName = null, string mimeType = null);
        string CreateRoute(string email);
        void DeleteRoute(string routeId);
        void CheckAllEmailStatus();
        int SendAllPendingEmails();
        void RegisterFailure(MailgunFailureDto model);
        void RegisterBounce(MailgunBounceDto model);
        void RegisterDelivery(MailgunDeliveryDto model);

        void SendHighwayEmailOk(HighwayEmailDataDto highwayEmailDataDto);
        void SendHighwayEmailErrors(HighwayEmailDataDto highwayEmailDataDto);
        void SendHighwayEmailRejected(HighwayEmailDataDto highwayEmailDataDto);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>0 = no se envio ni se guardo en BD, 1 = solo se envio, 2 = se envio y se persistio</returns>
        int SendhighwayTransacctionReports(HighwayEmailDataDto dto);

        void SendConciliationProcessResult(IDictionary<string, string> resultsDictionary);

        void SendCustomerSiteSystemUserCreationEmail(CustomerSiteSystemUserDto user);
        void SendCustomerSiteBillEmail(CustomerSiteGenerateAccessTokenDto dto, string paymentUrl);
        void SendCustomerSiteNewUserEmail(NewUserEmailDto dto);
        void SendCustomerSiteResetPasswordEmail(ResetPasswordEmailDto dto);

        void SendDebitSuscriptionNotification(DebitRequestEmailDto dto);
        void SendFileManualSynchronizationNotification(string email, string message);
        void SendFileManualSynchronizationNotificationError(string email, Exception e);
        void SendTc33SynchronizationNotification(string email, string message);
        void SendTc33SynchronizationNotificationError(string email, Exception e);

        void SendCustomerEliminateCard(DeleteCardRequestDto dto);
    }
}