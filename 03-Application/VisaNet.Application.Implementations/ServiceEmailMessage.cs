using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RazorEngine.Templating;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Security.Entities.Security;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.ExtensionMethods;

namespace VisaNet.Application.Implementations
{
    public class ServiceEmailMessage : BaseService<EmailMessage, EmailMessageDto>, IServiceEmailMessage
    {
        private static readonly string[] RedirectToEmail = string.IsNullOrEmpty(ConfigurationManager.AppSettings["RedirectEmailToTesting"]) ?
            new string[0] : (ConfigurationManager.AppSettings["RedirectEmailToTesting"]).Split('|');

        private readonly IServiceMailgun _serviceMailgun;
        private readonly IServicePaymentTicket _servicePaymentTicket;
        private readonly IRepositoryParameters _repositoryParameters;

        public ServiceEmailMessage(IRepositoryEmailMessage repository, IServiceMailgun serviceMailgun,
            IServicePaymentTicket servicePaymentTicket, IRepositoryParameters repositoryParameters)
            : base(repository)
        {
            _serviceMailgun = serviceMailgun;
            _servicePaymentTicket = servicePaymentTicket;
            _repositoryParameters = repositoryParameters;
        }

        public override IQueryable<EmailMessage> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        private IQueryable<EmailMessage> GetDataForTable(ReportsEmailsFilterDto filterDto)
        {
            var query = Repository.AllNoTracking();

            DateTime from = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filterDto.DateFromString))
            {
                from = DateTime.Parse(filterDto.DateFromString, new CultureInfo("es-UY"));
            }

            DateTime to = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filterDto.DateToString))
            {
                to = DateTime.Parse(filterDto.DateToString, new CultureInfo("es-UY"));
            }

            if (!from.Equals(DateTime.MinValue))
            {
                query = query.Where(p => p.CreationDateTime.CompareTo(from) >= 0);
            }

            if (!to.Equals(DateTime.MinValue))
            {
                var dateTo = to.AddDays(1);
                query = query.Where(p => p.CreationDateTime.CompareTo(dateTo) <= 0);
            }

            if (filterDto.EmailType != -1)
                query = query.Where(x => (int)x.EmailType == filterDto.EmailType);


            if (filterDto.Status != -1)
                query = query.Where(x => (int)x.Status == filterDto.Status);

            if (!string.IsNullOrEmpty(filterDto.To))
                query = query.Where(x => x.To.Contains(filterDto.To));

            return query;
        }

        public ICollection<EmailMessageDto> GetEmailsForTable(ReportsEmailsFilterDto filterDto)
        {
            var query = GetDataForTable(filterDto);

            //ordeno, skip y take
            if (filterDto.OrderBy.Equals("1"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status);
            }
            else if (filterDto.OrderBy.Equals("2"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.EmailType) : query.OrderByDescending(x => x.EmailType);
            }
            else if (filterDto.OrderBy.Equals("3"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.To) : query.OrderByDescending(x => x.To);
            }
            else
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.CreationDateTime) : query.OrderByDescending(x => x.CreationDateTime);
            }

            query = query.Skip(filterDto.DisplayStart);
            if (filterDto.DisplayLength.HasValue)
                query = query.Take(filterDto.DisplayLength.Value);

            return query.Select(em => new EmailMessageDto
            {
                To = em.To,
                Status = (MailgunStatusDto)em.Status,
                EmailType = (EmailTypeDto)em.EmailType,
                DataByType = em.DataByType,
                MailgunDescription = em.MailgunDescription,
                ParentId = em.ParentId,
                Body = em.Body,
                Id = em.Id,
                CreationDateTime = em.CreationDateTime,
                SendIntents = em.SendIntents,
                SendDateTime = em.SendDateTime,
                LastSendIntentDateTime = em.LastSendIntentDateTime,
                ApplicationUserId = em.ApplicationUserId,
                MailgunErrorDescription = em.MailgunErrorDescription
            }).ToList();
        }

        public void CancelEmail(Guid id)
        {
            Repository.ContextTrackChanges = true;
            var email = Repository.GetById(id);
            if (email.Status != MailgunStatus.FailureReachingMg && email.Status != MailgunStatus.DroppedOld)
            {
                Repository.ContextTrackChanges = false;
                throw new BusinessException(CodeExceptions.EMAIL_STATUS_CANNOT_BE_CANCELED);
            }
            email.Status = MailgunStatus.Canceled;
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public int GetEmailsForTableCount(ReportsEmailsFilterDto filterDto)
        {
            var query = GetDataForTable(filterDto);

            return query.Count();
        }

        public override EmailMessageDto Converter(EmailMessage entity)
        {
            return new EmailMessageDto
            {
                Id = entity.Id,
                ApplicationUserId = entity.ApplicationUserId,
                Body = entity.Body,
                CreationDateTime = entity.CreationDateTime,
                DataByType = entity.DataByType,
                EmailType = (EmailTypeDto)entity.EmailType,
                LastSendIntentDateTime = entity.LastSendIntentDateTime,
                MailgunDescription = entity.MailgunDescription,
                ParentId = entity.ParentId,
                SendDateTime = entity.SendDateTime,
                SendIntents = entity.SendIntents,
                Status = (MailgunStatusDto)entity.Status,
                To = entity.To,
                MailgunErrorDescription = entity.MailgunErrorDescription
            };
        }

        public override EmailMessage Converter(EmailMessageDto entity)
        {
            return new EmailMessage
            {
                Id = entity.Id,
                ApplicationUserId = entity.ApplicationUserId,
                Body = entity.Body,
                CreationDateTime = entity.CreationDateTime,
                DataByType = entity.DataByType,
                EmailType = (EmailType)entity.EmailType,
                LastSendIntentDateTime = entity.LastSendIntentDateTime,
                MailgunDescription = entity.MailgunDescription,
                ParentId = entity.ParentId,
                SendDateTime = entity.SendDateTime,
                SendIntents = entity.SendIntents,
                Status = (MailgunStatus)entity.Status,
                To = entity.To,
                MailgunErrorDescription = entity.MailgunErrorDescription
            };
        }

        public void SendNewUserEmail(ApplicationUser user, MembershipUser membershipUser)
        {
            try
            {
                var baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
                var model = new
                {
                    user.Name,
                    user.Surname,
                    user.Email,
                    membershipUser.ConfirmationToken,
                    BaseUrl = baseUrl
                };
                var mail = GenerateMail(EmailType.NewUser, model, new[] { user.Email });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.NewUser, mail, mailgunResponse, model, null, user.Id);

            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendNewUserRequestPassword(ApplicationUser user, MembershipUser membershipUser, string url = null)
        {
            try
            {
                var baseUrl = string.IsNullOrEmpty(url) ? ConfigurationManager.AppSettings["BaseUrl"] : url;
                var model = new
                {
                    user.Email,
                    user.Name,
                    user.Surname,
                    membershipUser.ResetPasswordToken,
                    BaseUrl = baseUrl
                };
                var mail = GenerateMail(EmailType.NewUserRequestPassword, model, new[] { user.Email });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.NewUserRequestPassword, mail, mailgunResponse, model, null, user.Id);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
                //TODO: hacer throw? sino controller no se entera
            }
        }

        public void SendNewBill(ApplicationUserDto user, ServiceDto service, string billId, string billDate, string billAmount)
        {
            try
            {
                var emailAddress = user.Email;
                var model = new
                {
                    Title = "Factura emitida",
                    Message = "Hay una nueva factura para el servicio " + service.Name,
                    BillId = billId,
                    BillDate = billDate,
                    BillAmount = billAmount,
                };

                var mail = GenerateMail(EmailType.NewBill, model, new[] { emailAddress });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.NewBill, mail, mailgunResponse, model, null, user.Id);

            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendBillExpired(ApplicationUserDto user, ServiceDto service, BillDto bill, string billAmount)
        {
            try
            {
                var emailAddress = user.Email;
                var model = new
                {
                    Title = "Factura vencida",
                    Message = "Hay una factura vencida para el servicio " + service.Name + " - " + service.Description,
                    BillId = bill.BillExternalId,
                    BillDate = bill.ExpirationDate.ToString("dd/MM/yyyy"),
                    BillAmount = billAmount,
                };

                var mail = GenerateMail(EmailType.BillExpired, model, new[] { emailAddress });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.BillExpired, mail, mailgunResponse, model, null, user.Id);

            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendBillAboutToExpired(ApplicationUserDto user, ServiceDto service, BillDto bill, string billAmount)
        {
            try
            {
                var emailAddress = user.Email;
                var model = new
                {
                    Title = "Factura próxima a vencer",
                    Message =
                        "Hay una factura próxima a vencer para el servicio " + service.Name + " - " +
                        service.Description,
                    BillId = bill.BillExternalId,
                    BillDate = bill.ExpirationDate.ToString("dd/MM/yyyy"),
                    BillAmount = billAmount,
                };

                var mail = GenerateMail(EmailType.BillAboutToExpired, model, new[] { emailAddress });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.BillAboutToExpired, mail, mailgunResponse, model, null, user.Id);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendResetPassword(ApplicationUser user, MembershipUser membershipUser, string url = null)
        {
            try
            {
                var baseUrl = string.IsNullOrEmpty(url) ? ConfigurationManager.AppSettings["BaseUrl"] : url;
                var model = new { user.Email, membershipUser.ResetPasswordToken, BaseUrl = baseUrl };
                var mail = GenerateMail(EmailType.ResetPassword, model, new[] { user.Email });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.ResetPassword, mail, mailgunResponse, model, null, user.Id);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendBinFileProcessed(Parameters parameter, BinFileProcessResultDto result)
        {
            try
            {
                var email = parameter.ErrorNotification.EmailAddress;
                var model = new { result.LinesProcessed, result.Errors, result.Inserts, result.Updates, result.Deletes };
                var mail = GenerateMail(EmailType.BinFileProcessed, model, new[] { email });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.BinFileProcessed, mail, mailgunResponse, model);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendContactFormEmail(Contact contact, Parameters parameter)
        {
            try
            {
                var model = new
                {
                    contact.Name,
                    contact.Surname,
                    contact.Email,
                    QueryTypeStr =
                        EnumHelpers.GetName(typeof(QueryTypeDto), (int)contact.QueryType,
                            EnumsStrings.ResourceManager),
                    contact.Subject,
                    contact.Message,
                };

                var mail = GenerateMail(EmailType.ContactForm, model, new[] { parameter.Contact.EmailAddress });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.ContactForm, mail, mailgunResponse, model);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendNewPayment(bool isAnonymousUser, string transactionNumber, AnonymousUserDto anoymousUser, ApplicationUserDto user, string serviceNameAndDescription, byte[] arrBytes, string mimeType)
        {
            try
            {
                var email = isAnonymousUser ? anoymousUser.Email : user.Email;

                if (email.Contains("á") || email.Contains("é") || email.Contains("í") || email.Contains("ó") ||
                    email.Contains("ú") ||
                    email.Contains("Á") || email.Contains("É") || email.Contains("Í") || email.Contains("Ó") ||
                    email.Contains("Ú"))
                {
                    return;
                }

                var textModel = new
                {
                    ServiceName = serviceNameAndDescription
                };

                var subject = "Nuevo Pago " + serviceNameAndDescription;

                Stream stream = new MemoryStream(arrBytes);

                var mail = GenerateMail(EmailType.NewPayment, textModel, new[] { email }, null, null, subject);

                mail.Attachments.Add(new Attachment(stream, transactionNumber + ".pdf", mimeType));

                var model = new { IsAnonymousUser = isAnonymousUser, TransactionNumber = transactionNumber, AnoymousUser = anoymousUser, ApplicationUser = user };
                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                if (mailgunResponse != null && mailgunResponse.Code == HttpStatusCode.Accepted)
                {
                    NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Se envio mail de Pago Nuevo al usuario: {0}", email));
                }

                if (isAnonymousUser)
                {
                    Save(EmailType.NewPayment, mail, mailgunResponse, model);
                }
                else
                {
                    Save(EmailType.NewPayment, mail, mailgunResponse, model, null, user.Id);
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendCopyPayment(bool isAnonymousUser, string transactionNumber, AnonymousUserDto anoymousUser, ApplicationUserDto user, byte[] arrBytes, string mimeType)
        {
            try
            {
                var email = isAnonymousUser ? anoymousUser.Email : user.Email;

                Stream stream = new MemoryStream(arrBytes);

                var mail = GenerateMail(EmailType.CopyPayment, null, new[] { email });
                mail.Attachments.Add(new Attachment(stream, transactionNumber + ".pdf", mimeType));

                var model = new { IsAnonymousUser = isAnonymousUser, TransactionNumber = transactionNumber, AnoymousUser = anoymousUser, ApplicationUser = user };

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                if (isAnonymousUser)
                {
                    Save(EmailType.CopyPayment, mail, mailgunResponse, model);
                }
                else
                {
                    Save(EmailType.CopyPayment, mail, mailgunResponse, model, null, user.Id);
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendServiceDeletedNotification(string serviceName, ApplicationUser user)
        {
            try
            {
                var email = user.Email;
                var model = new { ServiceName = serviceName };
                var mail = GenerateMail(EmailType.ServiceDeletedNotification, model, new[] { email });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.ServiceDeletedNotification, mail, mailgunResponse, model, null, user.Id);

            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendExpiringCard(ApplicationUserDto user, string maskedNumber, DateTime dueDate, string message)
        {
            try
            {
                var emailAddress = user.Email;
                var model = new
                {
                    MaskedNumber = maskedNumber,
                    DueDate = dueDate.ToString("MM/yyyy"),
                    Message = message,
                };

                var mail = GenerateMail(EmailType.ExpiringCard, model, new[] { emailAddress });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.ExpiringCard, mail, mailgunResponse, model, null, user.Id);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendPaymentCancellationError(string userEmail, Guid userId, LogUserType userType, Parameters parameter, string requestId, DateTime creationDateTime)
        {
            try
            {

                var emailTo = parameter.ErrorNotification;
                var emailAddress = emailTo.EmailAddress;
                var model = new
                {
                    RequestId = requestId,
                    Date = creationDateTime,
                    UserEmail = userEmail
                };

                var mail = GenerateMail(EmailType.PaymentCancellationError, model, new[] { emailAddress });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                if (userType == LogUserType.Registered)
                {
                    Save(EmailType.PaymentCancellationError, mail, mailgunResponse, model, null, userId);
                }
                else
                {
                    Save(EmailType.PaymentCancellationError, mail, mailgunResponse, model);
                }

            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendAutomaticPaymentNotification(AutomaticPaymentStatisticsDto processStatistics)
        {
            try
            {
                var parameter = _repositoryParameters.AllNoTracking().First();
                var emailTo = parameter.ErrorNotification;
                var emailAddress = emailTo.EmailAddress;

                var message = "La corrida nº" + processStatistics.ProcessRunNumber + " del proceso de pago programado y notificaciones finalizó en la fecha " + DateTime.Now.ToString("dd/MM/yyyy HH:mm");

                var model = new
                {
                    Message = message,
                    TotalServices = processStatistics.ServicesCount,
                    TotalBills = processStatistics.BillsCount,
                    TotalBillsToPay = processStatistics.BillsToPayCount,
                    TotalBillsNotReadyToPay = processStatistics.BillsNotReadyToPayCount,
                    PaymentsDone = processStatistics.BillsPayedCount,
                    PaymentsNotDoneAmount = processStatistics.BillsFailedAmountCount,
                    PaymentsNotDoneQuotas = processStatistics.BillsFailedQuotasCount,
                    PaymentsNotDoneCybersource = processStatistics.BillsFailedCybersourceCount,
                    PaymentsNotDoneGateway = processStatistics.BillsFailedGatewayCount,
                    PaymentsNotDoneInvalidCard = processStatistics.BillsFailedCardCount,
                    PaymentsNotDoneServiceValidation = processStatistics.BillsFailedServiceValidationCount,
                    PaymentsNotDoneDiscountCalculation = processStatistics.BillsFailedDiscountCalculationCount,
                    PaymentsNotDoneBinNotValidForService = processStatistics.BillsFailedBinNotValidForServiceCount,
                    PaymentsNotDoneExpired = processStatistics.BillsFailedExpiredCount,
                    PaymentsNotDoneExceptions = processStatistics.BillsFailedExceptionsCount,
                    GetBillsExceptions = processStatistics.GetBillsExceptionsCount
                };

                NLogLogger.LogEvent(NLogType.Info, "ServiceEmailMessage - SendAutomaticPaymentNotification - Comienza GenerateMail");
                var mail = GenerateMail(EmailType.AutomaticPaymentNotification, model, new[] { emailAddress });

                NLogLogger.LogEvent(NLogType.Info, "ServiceEmailMessage - SendAutomaticPaymentNotification - Comienza _serviceMailgun.SendHtml");
                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                if (mailgunResponse != null)
                {
                    NLogLogger.LogEvent(NLogType.Info, "ServiceEmailMessage - SendAutomaticPaymentNotification - " +
                        "Mailgun Response Cod. " + mailgunResponse.Code + " Desc. " + mailgunResponse.Description);
                }

                NLogLogger.LogEvent(NLogType.Info, "ServiceEmailMessage - SendAutomaticPaymentNotification - Comienza Save");
                Save(EmailType.AutomaticPaymentNotification, mail, mailgunResponse, model);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendUserAutomaticPaymentNotification(ApplicationUserDto user, bool serviceHasAutomaticPayment,
            string serviceAssociatedName, IDictionary<Guid, ProcessedBillResultDto> processedBillResults,
            string validationMessage)
        {
            try
            {
                var processedBillResultsList = processedBillResults.Values.ToList();

                var emailType = EmailType.UserBillNotification;
                var title = "Notificación de facturas";
                var message = "Notificación de facturas de la fecha " + DateTime.Now.ToString("dd/MM/yyyy") +
                              " para el servicio " + serviceAssociatedName + ":";

                if (serviceHasAutomaticPayment)
                {
                    emailType = EmailType.UserAutomaticPaymentNotification;
                    //TODO: se decidió que siempre diga notificacion de facturas
                    //title = "Pago programado";
                    //message = "Resultado del pago programado de la fecha " + DateTime.Now.ToString("dd/MM/yyyy") +
                    //          " para el servicio " + serviceAssociatedName + ":";
                }

                var emailAddress = user.Email;
                var model = new
                {
                    Title = title,
                    Message = message,
                    ProcessedBillResults = processedBillResultsList,
                    ValidationMessage = validationMessage
                };

                var mail = GenerateMail(emailType, model, new[] { emailAddress });
                var mailgunResponse = _serviceMailgun.SendHtml(mail);
                Save(emailType, mail, mailgunResponse, model, null, user.Id);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendBinNotDefined(Parameters parameter, string bin)
        {
            try
            {
                var email = parameter.ErrorNotification.EmailAddress;
                var model = new { Bin = bin };
                var mail = GenerateMail(EmailType.BinNotDefined, model, new[] { email });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.BinNotDefined, mail, mailgunResponse, model);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendGeneralNotification(ApplicationUserDto user, string title, string message)
        {
            try
            {
                var model = new { Title = title, Message = message };
                var mail = GenerateMail(EmailType.GeneralNotification, model, new[] { user.Email });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.GeneralNotification, mail, mailgunResponse, model, null, user.Id);
            }
            catch (Exception exception)
            {

                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendInternalErrorNotification(Parameters parameter, string title, object user, string message, string exceptionMessage, string stackTrace, Exception innerException)
        {
            try
            {
                var notificationEmail = parameter.ErrorNotification.EmailAddress;
                var model = new
                {
                    Title = title,
                    User = user,
                    Message = message,
                    ExceptionMessage = exceptionMessage,
                    StackTrace = stackTrace,
                    InnerException = innerException
                };

                var mail = GenerateMail(EmailType.InternalErrorNotification, model, new[] { notificationEmail });

                if (!string.IsNullOrWhiteSpace(title))
                {
                    mail.Subject = title;
                }

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.InternalErrorNotification, mail, mailgunResponse, model);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendInternalGeneralNotification(Parameters parameter, string title, string message)
        {
            try
            {
                var notificationEmail = parameter.ErrorNotification.EmailAddress;
                var model = new
                {
                    Title = title,
                    Message = message
                };

                var mail = GenerateMail(EmailType.InternalGeneralNotification, model, new[] { notificationEmail });


                if (!string.IsNullOrWhiteSpace(title))
                {
                    mail.Subject = title;
                }

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.InternalGeneralNotification, mail, mailgunResponse, model);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendCybersourceError(Parameters parameter, string title, string message)
        {
            try
            {
                var notificationEmail = parameter.ErrorNotification.EmailAddress;
                var model = new
                {
                    Title = title,
                    Message = message
                };

                var mail = GenerateMail(EmailType.CybersourceError, model, new[] { notificationEmail });


                if (!string.IsNullOrWhiteSpace(title))
                {
                    mail.Subject = title;
                }

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.CybersourceError, mail, mailgunResponse, model);

            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendPaymentDoneCancellation(string userEmail, string mailDescription, string originalTransaction,
            string date, string totalAmount, string cancellationTransaction, string dateCancellation,
            string totalAmountCancellation)
        {
            try
            {
                var model = new
                {
                    MailDescription = mailDescription,
                    OriginalTransaction = originalTransaction,
                    Date = date,
                    TotalAmount = totalAmount,
                    CancellationTransaction = cancellationTransaction,
                    DateCancellation = dateCancellation,
                    TotalAmountCancellation = totalAmountCancellation,
                };
                var mail = GenerateMail(EmailType.PaymentDoneCancellation, model, new[] { userEmail });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.PaymentDoneCancellation, mail, mailgunResponse, model);

            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public string CreateRoute(string email)
        {
            return _serviceMailgun.CreateRoute(email);
        }

        public void DeleteRoute(string routeId)
        {
            _serviceMailgun.DeleteRoute(routeId);
        }

        public void ResendEmail(Guid id)
        {
            //Obtengo el  mail
            var model = Repository.GetById(id);

            if (string.IsNullOrWhiteSpace(model.To))
                return;

            //Deserializo la data
            var data = LoadJsonData(model.DataByType);

            //Genero el mail
            var mail = GenerateMail(model.EmailType, data, model.To.Split(';'));

            //Genero el adjunto si es necesario
            if (model.EmailType == EmailType.NewPayment || model.EmailType == EmailType.CopyPayment)
            {
                byte[] arrBytes;
                string mimeType;
                var transactionNumber = (string)JObject.Parse(model.DataByType)["TransactionNumber"];
                _servicePaymentTicket.GeneratePaymentTicket(transactionNumber, Guid.Empty, out arrBytes, out mimeType);
                var stream = new MemoryStream(arrBytes);
                mail.Attachments.Add(new Attachment(stream, transactionNumber + ".pdf", mimeType));
            }
            else if (model.EmailType == EmailType.HighwayTransactionReportsOk || model.EmailType == EmailType.ExtractBanred ||
                    model.EmailType == EmailType.ExtractImporte || model.EmailType == EmailType.ExtractGeocom)
            {
                var path = (string)JObject.Parse(model.DataByType)["FilePath"];
                var fileName = (string)JObject.Parse(model.DataByType)["FileName"];
                var mimeType = (string)JObject.Parse(model.DataByType)["MimeType"];
                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(mimeType))
                {
                    var arrBytes = File.ReadAllBytes(path);
                    Stream stream = new MemoryStream(arrBytes);
                    mail.Attachments.Add(new Attachment(stream, fileName, mimeType));
                }
            }

            //Envío el mail
            var mailgunResponse = _serviceMailgun.SendHtml(mail);

            //Actualizo el mail origen con estado reenviado
            model.Status = MailgunStatus.Resend;
            Repository.Edit(model);
            Repository.Save();

            //Guardo el nuevo mail con una referencia a su padre
            Save(model.EmailType, mail, mailgunResponse, data, model.ParentId ?? id);
        }

        public void RegisterDelivery(MailgunDeliveryDto model)
        {
            Repository.ContextTrackChanges = true;
            var email = Repository.All(x => x.MailgunId == model.MessageId).FirstOrDefault();

            if (email == null)
            {
                Repository.ContextTrackChanges = false;
                throw new BusinessException(CodeExceptions.MAILGUN_EMAILNOTFOUND, model.MessageId);
            }

            if (!string.IsNullOrEmpty(email.To) && !string.IsNullOrEmpty(model.Recipient))
            {
                if (email.To.ToLower().Contains(model.Recipient.ToLower()))
                {
                    email.Status = MailgunStatus.Delivered;
                    email.SendDateTime = DateTime.Now;
                    Repository.Edit(email);
                    Repository.Save();
                }
                else
                {
                    NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Llego un mail con estado {0} y hay diferencias en los destinatarios. Mail '{1}', Mailgun '{2}'",
                        MailgunStatusDto.Delivered.ToString(), email.To, model.Recipient));
                }
            }
            else
            {
                NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Llego un mail con estado {0} y hay diferencias en los destinatarios. Mail '{1}', Mailgun '{2}'",
                    MailgunStatusDto.Delivered.ToString(), email.To, model.Recipient));
            }

            Repository.ContextTrackChanges = false;
        }

        public void RegisterBounce(MailgunBounceDto model)
        {
            Repository.ContextTrackChanges = true;
            var email = Repository.All(x => x.MailgunId == model.MessageId).FirstOrDefault();

            if (email == null)
            {
                Repository.ContextTrackChanges = false;
                throw new BusinessException(CodeExceptions.MAILGUN_EMAILNOTFOUND, model.MessageId);
            }

            if (!string.IsNullOrEmpty(email.To) && !string.IsNullOrEmpty(model.Recipient))
            {
                if (email.To.ToLower().Contains(model.Recipient.ToLower()))
                {
                    email.Status = MailgunStatus.DroppedOld;
                    email.SendDateTime = DateTime.Now;
                    Repository.Edit(email);
                    Repository.Save();
                }
                else
                {
                    NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Llego un mail con estado {0} y hay diferencias en los destinatarios. Mail '{1}', Mailgun '{2}'",
                        MailgunStatusDto.DroppedOld.ToString(), email.To, model.Recipient));
                }
            }
            else
            {
                NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Llego un mail con estado {0} y hay diferencias en los destinatarios. Mail '{1}', Mailgun '{2}'",
                    MailgunStatusDto.DroppedOld.ToString(), email.To, model.Recipient));
            }

            Repository.ContextTrackChanges = false;
        }

        public void RegisterFailure(MailgunFailureDto model)
        {
            Repository.ContextTrackChanges = true;
            var email = Repository.All(x => x.MailgunId == model.MessageId).FirstOrDefault();

            if (email == null)
            {
                Repository.ContextTrackChanges = false;
                throw new BusinessException(CodeExceptions.MAILGUN_EMAILNOTFOUND, model.MessageId);
            }

            if (!string.IsNullOrEmpty(email.To) && !string.IsNullOrEmpty(model.Recipient))
            {
                if (email.To.ToLower().Contains(model.Recipient.ToLower()))
                {
                    email.Status = MailgunStatus.DroppedHardFail;
                    email.SendDateTime = DateTime.Now;
                    email.MailgunErrorDescription = model.Description;
                    Repository.Edit(email);
                    Repository.Save();
                }
                else
                {
                    NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Llego un mail con estado {0} y hay diferencias en los destinatarios. Mail '{1}', Mailgun '{2}'",
                        MailgunStatusDto.DroppedHardFail.ToString(), email.To, model.Recipient));
                }
            }
            else
            {
                NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Llego un mail con estado {0} y hay diferencias en los destinatarios. Mail '{1}', Mailgun '{2}'",
                    MailgunStatusDto.DroppedHardFail.ToString(), email.To, model.Recipient));
            }

            Repository.ContextTrackChanges = false;
        }

        public void CheckAllEmailStatus()
        {
            NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Obteniendo mails"));
            var from = DateTime.Today.AddDays(-5).Date;
            var emails = Repository.AllNoTracking(e => (e.Status == MailgunStatus.SuccessReachingMg || e.Status == MailgunStatus.FailureReachingMg || e.Status == MailgunStatus.DroppedOld
                || e.Status == MailgunStatus.Unknown) && e.MailgunId != null && e.CreationDate > from);

            NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Cantidad de mails: {0}", emails.Count()));
            foreach (var email in emails.ToList())
            {
                try
                {
                    CheckEmailStatus(email);
                }
                catch (Exception exception)
                {
                    NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Se disparon una excepción mail: {0}, to {1}", email.MailgunId, email.To));
                    NLogLogger.LogEmailNotificationEvent(exception);
                }
            }
        }

        public int SendAllPendingEmails()
        {
            NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Obteniendo mails pendientes"));
            var emails = Repository.AllNoTracking(e => e.Status == MailgunStatus.FailureReachingMg || e.Status == MailgunStatus.DroppedOld).ToList();
            NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Cantidad de mails pendientes: {0}", emails.Count()));
            foreach (var email in emails)
            {
                if (!(email.EmailType == EmailType.HighwayTransactionReportsOk || email.EmailType == EmailType.ExtractBanred ||
                    email.EmailType == EmailType.ExtractImporte || email.EmailType == EmailType.ExtractGeocom))
                {
                    ResendEmail(email.Id);
                }
            }
            return emails.Count();
        }

        public void GenerateAttachment(Guid emailId, out FileDto file)
        {
            var model = Repository.GetById(emailId);
            if (model.EmailType == EmailType.NewPayment || model.EmailType == EmailType.CopyPayment)
            {
                byte[] renderedBytes;
                string mimeType;
                var transactionNumber = (string)JObject.Parse(model.DataByType)["TransactionNumber"];
                _servicePaymentTicket.GeneratePaymentTicket(transactionNumber, Guid.Empty, out renderedBytes, out mimeType);
                file = new FileDto
                {
                    ArrBytes = renderedBytes,
                    MimeType = mimeType,
                    FileName = string.Format("Ticket_{0}.pdf", transactionNumber)

                };
            }
            else if (model.EmailType == EmailType.HighwayTransactionReportsOk || model.EmailType == EmailType.ExtractBanred ||
                    model.EmailType == EmailType.ExtractImporte || model.EmailType == EmailType.ExtractGeocom)
            {
                var path = (string)JObject.Parse(model.DataByType)["FilePath"];
                var fileName = (string)JObject.Parse(model.DataByType)["FileName"];
                var mimeType = (string)JObject.Parse(model.DataByType)["MimeType"];
                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(mimeType))
                {
                    var renderedBytes = File.ReadAllBytes(path);
                    file = new FileDto
                    {
                        ArrBytes = renderedBytes,
                        MimeType = mimeType,
                        FileName = string.Format(fileName)

                    };
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                throw new Exception();
            }
        }

        public void SendHighwayEmailOk(HighwayEmailDataDto highwayEmailDataDto)
        {
            try
            {
                var emailAddress = highwayEmailDataDto.Email;
                var model = new
                {
                    Date = DateTime.Now.ToString("dd MMMM, yyyy", new CultureInfo("es-UY")),
                    ServiceName = highwayEmailDataDto.ServiceName,
                    CodCommerce = highwayEmailDataDto.CodCommerce,
                    CodBranch = highwayEmailDataDto.CodBranch,
                    FileName = highwayEmailDataDto.FileName,
                    TransactionNumber = highwayEmailDataDto.TransactionNumber,
                    CountPesos = highwayEmailDataDto.CountN,
                    CountDollars = highwayEmailDataDto.CountD,
                    ValuePesos = highwayEmailDataDto.ValueN.ToString("##,#0.00", CultureInfo.CurrentCulture),
                    ValueDollars = highwayEmailDataDto.ValueD.ToString("##,#0.00", CultureInfo.CurrentCulture)
                };

                var mail = GenerateMail(EmailType.HighwayProcessed, model, new[] { emailAddress }, null, null, highwayEmailDataDto.Subject);

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.HighwayProcessed, mail, mailgunResponse, model, null, null);

            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }
        public void SendHighwayEmailErrors(HighwayEmailDataDto highwayEmailDataDto)
        {
            try
            {
                var emailAddress = highwayEmailDataDto.Email;
                var model = new
                {
                    Date = DateTime.Now.ToString("dd MMMM, yyyy", new CultureInfo("es-UY")),
                    ServiceName = highwayEmailDataDto.ServiceName,
                    CodCommerce = highwayEmailDataDto.CodCommerce,
                    CodBranch = highwayEmailDataDto.CodBranch,
                    FileName = highwayEmailDataDto.FileName,
                    Errors = highwayEmailDataDto.Errors,
                };

                var mail = GenerateMail(EmailType.HighwayError, model, new[] { emailAddress }, null, null, highwayEmailDataDto.Subject);

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.HighwayError, mail, mailgunResponse, model, null, null);

            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }
        public void SendHighwayEmailRejected(HighwayEmailDataDto highwayEmailDataDto)
        {
            try
            {
                var emailAddress = highwayEmailDataDto.Email;
                var model = new
                {
                    Date = DateTime.Now.ToString("dd MMMM, yyyy", new CultureInfo("es-UY")),
                    ServiceName = highwayEmailDataDto.ServiceName,
                    CodCommerce = highwayEmailDataDto.CodCommerce,
                    CodBranch = highwayEmailDataDto.CodBranch,
                    RejectedMessage = highwayEmailDataDto.RejectedMessage,
                };

                var mail = GenerateMail(EmailType.HighwayRejected, model, new[] { emailAddress }, null, null, highwayEmailDataDto.Subject);

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.HighwayRejected, mail, mailgunResponse, model, null, null);

            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>0 = no se envio ni se guardo en BD, 1 = solo se envio, 2 = se envio y se persistio</returns>
        public int SendhighwayTransacctionReports(HighwayEmailDataDto dto)
        {
            var result = 0;
            try
            {
                var emailAddress = dto.Email;
                var model = new
                {
                    Title = dto.Subject,
                    Text = dto.Message,
                    FilePath = dto.FilePath,
                    FileName = dto.FileName,
                    MimeType = dto.MimeType
                };
                var mail = GenerateMail(EmailType.HighwayTransactionReportsOk, model, new[] { emailAddress }, null, null, dto.Subject);
                if (dto.AttachmentFile != null)
                {
                    mail.Attachments.Add(dto.AttachmentFile);
                }

                var mailgunResponse = _serviceMailgun.SendHtml(mail);
                result = 1;
                Save(EmailType.HighwayTransactionReportsOk, mail, mailgunResponse, model, null, null);
                result = 2;
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subject"></param>
        /// <param name="text"></param>
        /// <param name="to"></param>
        /// <param name="path"></param>
        /// <param name="fileName"></param>
        /// <param name="mimeType"></param>
        /// <returns>0 = no se envio ni se guardo en BD, 1 = solo se envio a mailgun, 2 = se envio y se persistio</returns>
        public int SendExtract(EmailType type, string subject, string text, string to, string path = null, string fileName = null, string mimeType = null)
        {
            var result = 0;
            try
            {

                var model = new { Text = text, Title = subject, FilePath = path, FileName = fileName, MimeType = mimeType };
                var mail = GenerateMail(type, model, new[] { to }, null, null, subject);

                if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(mimeType))
                {
                    var arrBytes = File.ReadAllBytes(path);
                    Stream stream = new MemoryStream(arrBytes);
                    mail.Attachments.Add(new Attachment(stream, fileName, mimeType));
                }

                var mailgunResponse = _serviceMailgun.SendHtml(mail);
                result = 1;

                Save(type, mail, mailgunResponse, model);
                result = 2;

                return result;

            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
            return result;
        }

        public void SendConciliationProcessResult(IDictionary<string, string> resultsDictionary)
        {
            try
            {
                var parameter = _repositoryParameters.AllNoTracking().First();
                var emailTo = parameter.ErrorNotification;
                var emailAddress = emailTo.EmailAddress;

                const string title = "Resultado procesamiento de conciliación";
                var message = DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " - Finalizó el procesamiento de los siguientes archivos de conciliación:";
                if (!resultsDictionary.Any())
                {
                    message = DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " - No se encontraron archivos para procesar.";
                }

                var model = new
                {
                    Title = title,
                    Message = message,
                    Results = resultsDictionary
                };

                var mail = GenerateMail(EmailType.ConciliationResults, model, new[] { emailAddress });
                var mailgunResponse = _serviceMailgun.SendHtml(mail);
                Save(EmailType.ConciliationResults, mail, mailgunResponse, model);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendCustomerSiteSystemUserCreationEmail(CustomerSiteSystemUserDto user)
        {
            try
            {
                var baseUrl = ConfigurationManager.AppSettings["BaseUrlCustomerSiteFrontend"];
                var model = new
                {
                    user.Email,
                    user.MembershipIdentifierObj.ConfirmationToken,
                    BaseUrl = baseUrl,
                    Commerce = user.CommerceDto.Name
                };
                var mail = GenerateMail(EmailType.CustomerSiteSystemUserCreation, model, new[] { user.Email });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.CustomerSiteSystemUserCreation, mail, mailgunResponse, model);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }
        public void SendCustomerSiteNewUserEmail(NewUserEmailDto dto)
        {
            try
            {
                var baseUrl = ConfigurationManager.AppSettings["BaseUrlCustomerSiteFrontend"];
                var model = new
                {
                    Email = dto.Email,
                    ConfirmationToken = dto.ConfirmationPasswordToken,
                    BaseUrl = baseUrl,
                    Commerce = dto.CommerceName,
                };
                var mail = GenerateMail(EmailType.CustomerSiteSystemUserCreation, model, new[] { dto.Email });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.CustomerSiteSystemUserCreation, mail, mailgunResponse, model);

            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }
        public void SendCustomerSiteBillEmail(CustomerSiteGenerateAccessTokenDto dto, string paymentUrl)
        {
            try
            {
                var emailAddress = dto.Email;
                var name = dto.ServiceDto.ServiceContainerDto == null ? dto.ServiceDto.Name : dto.ServiceDto.ServiceContainerDto.Name;
                var model = new
                {
                    Title = "Nueva factura para pagar ",
                    Message = "Hay una nueva factura para el comercio ",
                    ServiceName = name,
                    BillId = dto.BillExternalId,
                    BillDate = dto.BillExpirationDate.ToString("dd/MM/yyyy"),
                    BillAmount = dto.BillAmount.ToString("##,#0.00", CultureInfo.CurrentCulture),
                    BillCurrency =
                        dto.BillCurrency.Equals(Currency.PESO_URUGUAYO, StringComparison.CurrentCultureIgnoreCase) ||
                        dto.BillCurrency.Equals("N", StringComparison.CurrentCultureIgnoreCase) ? "$" : "U$S",
                    BillDesc = dto.BillDescription,
                    BillUrl = paymentUrl
                };

                var mail = GenerateMail(EmailType.CustomerSiteBillEmail, model, new[] { emailAddress });
                var mailgunResponse = _serviceMailgun.SendHtml(mail);
                Save(EmailType.CustomerSiteBillEmail, mail, mailgunResponse, model, null, null);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }
        public void SendCustomerSiteResetPasswordEmail(ResetPasswordEmailDto dto)
        {
            try
            {
                var baseUrl = ConfigurationManager.AppSettings["BaseUrlCustomerSiteFrontend"];
                var model = new
                {
                    Email = dto.Email,
                    ConfirmationToken = dto.ResetPasswordToken,
                    BaseUrl = baseUrl,
                    Commerce = dto.CommerceName,
                };
                var mail = GenerateMail(EmailType.CustomerSiteResetPassword, model, new[] { dto.Email });

                var mailgunResponse = _serviceMailgun.SendHtml(mail);

                Save(EmailType.CustomerSiteResetPassword, mail, mailgunResponse, model);

            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendDebitSuscriptionNotification(DebitRequestEmailDto dto)
        {
            try
            {
                var model = new
                {
                    ProductName = dto.ProductName,
                    ServiceName = dto.ServiceName,
                    Status = dto.Status.ToUpper(),
                    Type = dto.Type.ToLower(),
                    References = dto.References,
                    Email = dto.Email,
                    MaskedNumber = dto.MaskedNumber,
                };
                var mail = GenerateMail(EmailType.DebitSuscriptionNotification, model, new[] { model.Email });
                var mailgunResponse = _serviceMailgun.SendHtml(mail);
                Save(EmailType.DebitSuscriptionNotification, mail, mailgunResponse, model, null, dto.ApplicationUserId);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

        public void SendFileManualSynchronizationNotification(string email, string message)
        {
            var parameters = new Parameters { ErrorNotification = new Email { EmailAddress = email } };
            var title = "Débitos: listado de solicitudes en sincronización manual";

            SendInternalGeneralNotification(parameters, title, message);
        }

        public void SendFileManualSynchronizationNotificationError(string email, Exception e)
        {
            var parameters = new Parameters { ErrorNotification = new Email { EmailAddress = email } };
            var title = "Débitos: listado de solicitudes en sincronización manual";
            var message = "Hubo un error al querer generar el archivo de solicitudes de débitos en sincronización manual";

            SendInternalErrorNotification(parameters, title, null, message, e.Message, e.StackTrace, e.InnerException);
        }

        public void SendTc33SynchronizationNotification(string email, string message)
        {
            var parameters = new Parameters { ErrorNotification = new Email { EmailAddress = email } };
            var title = "Tc33: Procesamiento del archivo TC33 subido a VisaNetPagosAdmin";
            SendInternalGeneralNotification(parameters, title, message);
        }

        public void SendTc33SynchronizationNotificationError(string email, Exception e)
        {
            var parameters = new Parameters { ErrorNotification = new Email { EmailAddress = email } };
            var title = "Tc33: Procesamiento del archivo TC33 subido a VisaNetPagosAdmin";
            var message = "Hubo un error al querer generar el archivo.";
            SendInternalErrorNotification(parameters, title, null, message, e.Message, e.StackTrace, e.InnerException);
        }

        #region Private

        private MailMessage GenerateMail(EmailType emailType, object model, string[] To, string[] CC = null,
            string[] BCC = null, string subject = null)
        {
            try
            {
                if (RedirectToEmail.Any())
                {
                    To = RedirectToEmail;
                }

                var templateFileName = GetEmailTemplate(emailType);

                var mail = new MailMessage();
                var templatePath = Path.Combine(ConfigurationManager.AppSettings["MailgunTemplates"], templateFileName + ".cshtml");
                var templateService = new TemplateService();
                var emailHtmlBody = templateService.Parse(File.ReadAllText(templatePath), model, null, null);

                //Replace the images path
                emailHtmlBody = emailHtmlBody.Replace("{HeaderPath}", ConfigurationManager.AppSettings["HeaderPath"]);

                mail.Subject = !string.IsNullOrEmpty(subject) ? subject : string.Format(EnumHelpers.GetName(typeof(EmailType), (int)emailType, EnumsStrings.ResourceManager));

                mail.SubjectEncoding = Encoding.UTF8;

                mail.IsBodyHtml = true;
                mail.Body = emailHtmlBody;

                if (To == null) throw new Exception();

                if (RedirectToEmail.Any() && !string.IsNullOrEmpty(RedirectToEmail[0]))
                    RedirectToEmail.ForEach(m => mail.To.Add(m));
                else
                    To.ForEach(m => mail.To.Add(m));

                if (CC != null) CC.ForEach(m => mail.CC.Add(m));
                if (BCC != null) BCC.ForEach(m => mail.Bcc.Add(m));

                return mail;
            }
            catch (Exception ex)
            {
                var mailTo = To != null ? string.Join(", ", To) : string.Empty;
                NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("GenerateMail Exception - emailType: {0}, To: {1}", emailType, mailTo));
                NLogLogger.LogEmailNotificationEvent(ex);
                throw;
            }
        }

        /// <summary>
        /// TODO: AGREGAR ATTACHMENT
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>

        private string GetEmailTemplate(EmailType type)
        {
            switch (type)
            {
                case EmailType.NewUser:
                    return "NewApplicationUser";
                case EmailType.ResetPassword:
                    return "ResetPasswordApplicationUser";
                case EmailType.ContactForm:
                    return "NewContact";
                case EmailType.NewBill:
                    return "BillNotification";
                case EmailType.NewPayment:
                    return "NewPayment";
                case EmailType.CopyPayment:
                    return "CopyPayment";
                case EmailType.ServiceDeletedNotification:
                    return "ServiceDeletedNotification";
                case EmailType.ExpiringCard:
                    return "ExpiringCard";
                case EmailType.PaymentCancellationError:
                case EmailType.PaymentDoneCancellation:
                    return "PaymentDoneCancellation";
                case EmailType.BinNotDefined:
                    return "BinNotDefined";
                case EmailType.GeneralNotification:
                    return "GeneralNotification";
                case EmailType.InternalErrorNotification:
                    return "InternalErrorNotification";
                case EmailType.InternalGeneralNotification:
                    return "InternalGeneralNotification";
                case EmailType.AutomaticPaymentNotification:
                    return "AutomaticPaymentNotification";
                case EmailType.BillAboutToExpired:
                    return "BillNotification";
                case EmailType.BillExpired:
                    return "BillNotification";
                case EmailType.CybersourceError:
                    return "InternalGeneralNotification";
                case EmailType.HighwayProcessed:
                    return "HighwayProcessed";
                case EmailType.HighwayRejected:
                    return "HighwayRejected";
                case EmailType.HighwayError:
                    return "HighwayError";
                case EmailType.HighwayTransactionReportsOk:
                case EmailType.ExtractBanred:
                case EmailType.ExtractImporte:
                case EmailType.ExtractGeocom:
                    return "PaymentsFileExtract";
                case EmailType.UserAutomaticPaymentNotification:
                case EmailType.UserBillNotification:
                    return "UserAutomaticPaymentNotification";
                case EmailType.BinFileProcessed:
                    return "BinFileProcessed";
                case EmailType.NewUserRequestPassword:
                    return "NewUserRequestPassword";
                case EmailType.ConciliationResults:
                    return "ConciliationResults";
                case EmailType.CustomerSiteSystemUserCreation:
                    return "CustomerSiteSystemUserCreation";
                case EmailType.CustomerSiteBillEmail:
                    return "CustomerSiteBillNotification";
                case EmailType.CustomerSiteResetPassword:
                    return "CustomerSiteResetPassword";
                case EmailType.DebitSuscriptionNotification:
                    return "DebitSuscriptionNotification";
                case EmailType.DeleteCustomerCard:
                    return "DeletedCustomerCardNotification";
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        private void Save(EmailType type, MailMessage mail, MailgunResponse mailgunResponse, object model, Guid? parentId = null, Guid? applicationUserId = null)
        {
            try
            {
                var entity = new EmailMessage
                {
                    DataByType = JsonConvert.SerializeObject(model),
                    EmailType = type,
                    LastSendIntentDateTime = DateTime.Now,
                    MailgunDescription = mailgunResponse.Description,
                    Status = mailgunResponse.Code == HttpStatusCode.OK ? MailgunStatus.SuccessReachingMg : MailgunStatus.FailureReachingMg,
                    To = string.Join(";", mail.To),
                    ParentId = parentId,
                    Body = mail.Body,
                    MailgunId = GetMailgunId(mailgunResponse.Description),
                    ApplicationUserId = applicationUserId
                };

                Repository.Create(entity);
                Repository.Save();
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(NLogType.Error, string.Format("Save Mail Error - To {0}, EmailType {1}", mail.To, type));
                NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Id Usuario: " + applicationUserId));
                NLogLogger.LogEmailNotificationEvent(exception);
                throw;
            }
        }

        private string GetMailgunId(string description)
        {
            try
            {
                var json = (dynamic)JsonConvert.DeserializeObject(description);
                var rawId = (string)json.id;

                return string.IsNullOrWhiteSpace(rawId) ? null : rawId.Replace("<", "").Replace(">", "");
            }
            catch (Exception exception)
            {
                return string.Empty;
            }
        }

        private void CheckEmailStatus(EmailMessage message)
        {
            NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Inicio obtener estado de mail: (id) {0} (MailgunId) {1}", message.Id, message.MailgunId));
            var status = _serviceMailgun.GetEmailStatus(message.MailgunId, message.To);
            NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Mail {0}, Estado {1}", message.MailgunId, status));
            message.Status = status;

            Repository.Edit(message);
            Repository.Save();
        }

        private object LoadJsonData(string jsonString)
        {
            var obj = JObject.Parse(jsonString);
            var dynamicObject = new ExpandoObject() as IDictionary<string, Object>;

            foreach (var property in obj.Properties())
            {
                dynamicObject.Add(property.Name, obj.GetValue(property.Name));
            }

            return dynamicObject;
        }

        

        #endregion
        public void SendCustomerEliminateCard(DeleteCardRequestDto dto)
        {
            try
            {
                var model = new
                {
                    ProductName = dto.ProductName,
                    ServiceName = dto.ServiceName,
                    Status = dto.Status.ToUpper(),
                    References = dto.References,
                    Email = dto.ApplicationUserDto.Email,
                    Type = dto.Type.ToLower(),
                    CardName = dto.CardDto.Name,
                    dto.MaskedNumber,
                    Description = dto.CardDto.Description
                };
                var mail = GenerateMail(EmailType.DeleteCustomerCard, model, new[] { model.Email });
                var mailgunResponse = _serviceMailgun.SendHtml(mail);
                Save(EmailType.DeleteCustomerCard, mail, mailgunResponse, model, null, dto.ApplicationUserDto.Id);
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(exception);
            }
        }

    }
}