using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Texts;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.Entities.NotificationHelpersEntities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.NotificationsData;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations
{
    public class UserNotificationFactory : IUserNotificationFactory
    {
        private readonly IServiceEmailMessage _serviceEmailMessage;
        private readonly IServiceNotification _serviceNotification;
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;
        private readonly IServiceNewBillNotificationInfo _serviceNewBillNotificationInfo;

        public UserNotificationFactory(IServiceEmailMessage serviceEmailMessage, IServiceNotification serviceNotification,
            IServiceServiceAssosiate serviceServiceAssosiate, IServiceNewBillNotificationInfo serviceNewBillNotificationInfo)
        {
            _serviceEmailMessage = serviceEmailMessage;
            _serviceNotification = serviceNotification;
            _serviceServiceAssosiate = serviceServiceAssosiate;
            _serviceNewBillNotificationInfo = serviceNewBillNotificationInfo;
        }

        public void BillExceedsQuotasNotification(ServiceAssociatedDto serviceAssociatedDto)
        {
            var message = string.Format(
                "El pago programado para el servicio {0} {1} no pudo ser ejecutado, el máximo número {2} de facturas " +
                "a pagar ya fue superado.",
                serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "",
                serviceAssociatedDto.Description,
                serviceAssociatedDto.AutomaticPaymentDto.Quotas);

            var notification = new NotificationDto
            {
                Date = DateTime.Now,
                Message = message,
                NotificationPrupose = NotificationPruposeDto.AlertNotification,
                RegisteredUserId = serviceAssociatedDto.UserId,
                ServiceId = serviceAssociatedDto.ServiceId,
            };
            _serviceNotification.Create(notification);
        }

        public void BillExceedsAmountNotification(ServiceAssociatedDto serviceAssociatedDto, BillDto bill)
        {
            var message = string.Format(
                "El pago programado para el servicio {0} {1} no pudo ser ejecutado, el monto máximo ingresado " +
                "{2} {3} es inferior al monto de la factura {4} {5}",
                serviceAssociatedDto.ServiceDto.Name,
                serviceAssociatedDto.Description,
                bill.Currency.Equals("UYU") ? "$" : "U$S",
                serviceAssociatedDto.AutomaticPaymentDto.Maximum,
                bill.Currency.Equals("UYU") ? "$" : "U$S",
                bill.Amount);

            var notification = new NotificationDto
            {
                Date = DateTime.Now,
                Message = message,
                NotificationPrupose = NotificationPruposeDto.AlertNotification,
                RegisteredUserId = serviceAssociatedDto.UserId,
                ServiceId = serviceAssociatedDto.ServiceId,
            };
            _serviceNotification.Create(notification);
        }

        public void NotifyServiceResultToUser(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult,
            IEnumerable<BillDto> bills, Dictionary<Guid, PaymentResultTypeDto> billResultDictionary)
        {
            var processedBillResults = InitializeProcessedBillsDictionary(bills, serviceAssociatedDto, billResultDictionary);

            if (processedBillResults != null && processedBillResults.Any())
            {
                //Mensaje facturas vencidas
                ExpiredBillMessages(serviceAssociatedDto, processedBillResults);

                //Mensaje facturas por vencer
                BeforeDueDateBillMessages(serviceAssociatedDto, processedBillResults);

                //Mensaje facturas nuevas
                NewBillMessages(serviceAssociatedDto, processedBillResults);

                //Mensaje resultado de intentos de pago
                BillPaymentResultMessages(serviceAssociatedDto, processedBillResults);
            }

            var serviceAssociatedName = serviceAssociatedDto.ServiceDto.Name;
            if (!string.IsNullOrEmpty(serviceAssociatedDto.Description))
            {
                serviceAssociatedName += " (" + serviceAssociatedDto.Description + ")";
            }

            bool serviceHasAutomaticPayment = serviceAssociatedDto.AutomaticPaymentDtoId.HasValue &&
                                              !serviceAssociatedDto.AutomaticPaymentDtoId.Value.Equals(Guid.Empty);

            //Si es necesario se agrega un mensaje adicional
            var additionalMessage = AdditionalMessage(serviceAssociatedDto, serviceResult, processedBillResults);

            //Envio de email
            if ((processedBillResults != null && processedBillResults.Any()) || !string.IsNullOrEmpty(additionalMessage))
            {
                _serviceEmailMessage.SendUserAutomaticPaymentNotification(serviceAssociatedDto.RegisteredUserDto,
                    serviceHasAutomaticPayment, serviceAssociatedName, processedBillResults, additionalMessage);
            }
        }


        //Bill notifications
        private void ExpiredBillMessages(ServiceAssociatedDto serviceAssociatedDto,
            IDictionary<Guid, ProcessedBillResultDto> processedBillResults)
        {
            var expiredBills = processedBillResults.Where(x => x.Value.BillType == BillType.ExpiredBill).ToList();

            foreach (var bill in expiredBills)
            {
                var billTmp = bill.Value;
                var alreadyExistsNotification =
                    _serviceNewBillNotificationInfo.AlreadyExistsExpiredBillNotification(billTmp.BillExternalId, serviceAssociatedDto.UserId, serviceAssociatedDto.ServiceId);

                if (!alreadyExistsNotification)
                {
                    NLogLogger.LogEvent(NLogType.Info,
                        "Generación de facturas -- Factura: " + billTmp.BillExternalId + " no se notifico aun");
                    if (billTmp.ExpirationDate.CompareTo(DateTime.Today.Date) < 0)
                    {
                        NLogLogger.LogEvent(NLogType.Info,
                            "Generación de facturas -- Fecha de vencimiento menor a fecha de hoy. Factura: " +
                            billTmp.BillExternalId);

                        processedBillResults[billTmp.Id].AlreadyNotified = false;

                        //Se registra la notificacion para que no se vuelva a notificar
                        _serviceNotification.Create(new NotificationDto
                        {
                            Date = DateTime.Now.Date,
                            RegisteredUserId = serviceAssociatedDto.UserId,
                            ServiceId = serviceAssociatedDto.ServiceId,
                            NotificationPrupose = NotificationPruposeDto.AlertNotification,
                            Message = TextsMessagesStrings.ExpiredBill,
                        });

                        var newBillNotificationDto = new NewBillNotificationInfoDto
                        {
                            BillNumber = billTmp.BillExternalId,
                            BillType = BillTypeDto.ExpiredBill,
                            ExpirationDate = billTmp.ExpirationDate,
                            CreationDate = DateTime.Now,
                            ServiceId = serviceAssociatedDto.ServiceId,
                            ApplicationUserId = serviceAssociatedDto.UserId
                        };
                        _serviceNewBillNotificationInfo.Create(newBillNotificationDto);
                    }
                }
                else
                {
                    if (billTmp.PaymentResultType == PaymentResultTypeDto.BillOk ||
                        billTmp.PaymentResultType == PaymentResultTypeDto.AutomaticPaymentDisabled)
                    {
                        //Factura ya notificada
                        processedBillResults.Remove(bill.Key);
                    }
                }
            }
        }

        private void BeforeDueDateBillMessages(ServiceAssociatedDto serviceAssociatedDto,
            IDictionary<Guid, ProcessedBillResultDto> processedBillResults)
        {
            var aboutToExpireBills = processedBillResults.Where(x => x.Value.BillType == BillType.AboutToExpireBill).ToList();

            foreach (var bill in aboutToExpireBills)
            {
                var billTmp = bill.Value;
                var alreadyExistsNotification =
                    _serviceNewBillNotificationInfo.AlreadyExistsBeforeDueDateBillNotification(billTmp.BillExternalId, serviceAssociatedDto.UserId, serviceAssociatedDto.ServiceId);

                if (!alreadyExistsNotification)
                {
                    NLogLogger.LogEvent(NLogType.Info, "Generación de facturas -- Factura: " + billTmp.BillExternalId +
                        " no se notifico aun");
                    if (billTmp.ExpirationDate.AddDays(-serviceAssociatedDto.NotificationConfigDto.DaysBeforeDueDate)
                        .CompareTo(DateTime.Today.Date) <= 0)
                    {
                        NLogLogger.LogEvent(NLogType.Info, "Generación de facturas -- Dentro de paramero de días que el " +
                            "usuario pidio que se notifique. Facutra: " + billTmp.BillExternalId);

                        processedBillResults[billTmp.Id].AlreadyNotified = false;

                        //Se registra la notificacion para que no se vuelva a notificar
                        _serviceNotification.Create(new NotificationDto
                        {
                            Date = DateTime.Now.Date,
                            RegisteredUserId = serviceAssociatedDto.UserId,
                            ServiceId = serviceAssociatedDto.ServiceId,
                            NotificationPrupose =
                                NotificationPruposeDto.AlertNotification,
                            Message = TextsMessagesStrings.AboutToExpire
                        });

                        var newBillNotificationDto = new NewBillNotificationInfoDto
                        {
                            BillNumber = billTmp.BillExternalId,
                            BillType = BillTypeDto.AboutToExpireBill,
                            ExpirationDate = billTmp.ExpirationDate,
                            CreationDate = DateTime.Now,
                            ServiceId = serviceAssociatedDto.ServiceId,
                            ApplicationUserId = serviceAssociatedDto.UserId
                        };
                        _serviceNewBillNotificationInfo.Create(newBillNotificationDto);
                    }
                }
                else
                {
                    if (billTmp.PaymentResultType == PaymentResultTypeDto.BillOk ||
                        billTmp.PaymentResultType == PaymentResultTypeDto.AutomaticPaymentDisabled)
                    {
                        //Factura ya notificada
                        processedBillResults.Remove(bill.Key);
                    }
                }
            }
        }

        private void NewBillMessages(ServiceAssociatedDto serviceAssociatedDto,
            IDictionary<Guid, ProcessedBillResultDto> processedBillResults)
        {
            var newBills = processedBillResults.Where(x => x.Value.BillType == BillType.NewBill).ToList();

            foreach (var bill in newBills)
            {
                var billTmp = bill.Value;
                var alreadyExistsNotification =
                    _serviceNewBillNotificationInfo.AlreadyExistsNewBillNotification(billTmp.BillExternalId, serviceAssociatedDto.UserId, serviceAssociatedDto.ServiceId);

                if (!alreadyExistsNotification)
                {
                    NLogLogger.LogEvent(NLogType.Info, "Generación de facturas -- Factura: " + billTmp.BillExternalId + " no se notifico aun");

                    //TODO HOTFIX
                    NLogLogger.LogEvent(NLogType.Info, string.Format("FACTURA NUEVA. NO NOTIFICO AL USUARIO. servicio asociado {0}, nro factura {1}, monto {2}, ExpirationDate {3}", serviceAssociatedDto.Id, billTmp.BillExternalId, billTmp.Amount, billTmp.ExpirationDate));
                    processedBillResults[billTmp.Id].AlreadyNotified = true;

                    //Se registra la notificacion para que no se vuelva a notificar
                    _serviceNotification.Create(new NotificationDto
                    {
                        Date = DateTime.Now.Date,
                        RegisteredUserId = serviceAssociatedDto.UserId,
                        ServiceId = serviceAssociatedDto.ServiceId,
                        NotificationPrupose = NotificationPruposeDto.AlertNotification,
                        Message = TextsMessagesStrings.NewBill
                    });

                    var newBillNotificationDto = new NewBillNotificationInfoDto
                    {
                        BillNumber = billTmp.BillExternalId,
                        BillType = BillTypeDto.NewBill,
                        ExpirationDate = billTmp.ExpirationDate,
                        CreationDate = DateTime.Now,
                        ServiceId = serviceAssociatedDto.ServiceId,
                        ApplicationUserId = serviceAssociatedDto.UserId
                    };
                    _serviceNewBillNotificationInfo.Create(newBillNotificationDto);
                }
                else
                {
                    if (billTmp.PaymentResultType == PaymentResultTypeDto.BillOk ||
                        billTmp.PaymentResultType == PaymentResultTypeDto.AutomaticPaymentDisabled)
                    {
                        //Factura ya notificada
                        processedBillResults.Remove(bill.Key);
                    }
                }
            }
        }

        private void BillPaymentResultMessages(ServiceAssociatedDto serviceAssociatedDto,
            IDictionary<Guid, ProcessedBillResultDto> processedBillResults)
        {
            foreach (var processedBill in processedBillResults)
            {
                //Se notifica según el resultado del pago de la factura
                var billTmp = processedBill.Value;
                var billResult = billTmp.PaymentResultType;
                switch (billTmp.PaymentResultType)
                {
                    case PaymentResultTypeDto.Success:
                        processedBillResults[billTmp.Id].PaymentResultMessage = BillPayedMessage();
                        break;
                    case PaymentResultTypeDto.BillOk:
                        processedBillResults[billTmp.Id].PaymentResultMessage = BillOnlyToNotifyMessage(serviceAssociatedDto, billTmp);
                        break;
                    case PaymentResultTypeDto.AutomaticPaymentDisabled:
                        processedBillResults[billTmp.Id].PaymentResultMessage = BillAutomaticPaymentDisabledMessage();
                        break;
                    case PaymentResultTypeDto.ExceededPaymentsAmount:
                        processedBillResults[billTmp.Id].PaymentResultMessage = BillExceedsAmountMessage(serviceAssociatedDto, billTmp);
                        break;
                    case PaymentResultTypeDto.ExceededPaymentsCount:
                        processedBillResults[billTmp.Id].PaymentResultMessage = BillExceedsQuotasMessage(serviceAssociatedDto);
                        break;
                    case PaymentResultTypeDto.BillExpired:
                        processedBillResults[billTmp.Id].PaymentResultMessage = BillExpiredMessage();
                        break;
                    case PaymentResultTypeDto.BinNotValidForService:
                        processedBillResults[billTmp.Id].PaymentResultMessage = BillBinNotValidForServiceMessage(serviceAssociatedDto);
                        break;
                    case PaymentResultTypeDto.ServiceNotAllowsAutomaticPayment:
                    case PaymentResultTypeDto.InvalidCardDueDate:
                    case PaymentResultTypeDto.InvalidCardToken:
                    case PaymentResultTypeDto.InvalidCardBin:
                    case PaymentResultTypeDto.CsExpiredCard:
                    case PaymentResultTypeDto.CsStolenCard:
                    case PaymentResultTypeDto.CsInactiveCard:
                    case PaymentResultTypeDto.CsCardTypeNotAccepted:
                    case PaymentResultTypeDto.CsInvalidAccountNumber:
                        //Con estos códigos se elimina el pago programado
                        processedBillResults[billTmp.Id].PaymentResultMessage =
                            BillDeclinedDeleteAutomaticPaymentMessage(serviceAssociatedDto, billResult);
                        break;
                    default:
                        //Cualquier otro error se manda mensaje genérico
                        var controlledErrorCodes = RunState.ControlledErrorCodes();
                        var retryErrorCodes = RunState.RetryErrorCodes();
                        if (controlledErrorCodes.Contains(billResult) ||
                            retryErrorCodes.Contains(billResult))
                        {
                            processedBillResults[billTmp.Id].PaymentResultMessage = BillGeneralErrorMessage();
                        }
                        break;
                }
            }
        }

        private string BillPayedMessage()
        {
            return "La factura se pagó correctamente. El comprobante de pago te llegará en otro correo.";
        }

        private string BillOnlyToNotifyMessage(ServiceAssociatedDto serviceAssociatedDto, ProcessedBillResultDto bill)
        {
            var msg = "";
            if (serviceAssociatedDto.AutomaticPaymentDto == null ||
                !serviceAssociatedDto.AutomaticPaymentDtoId.HasValue ||
                serviceAssociatedDto.AutomaticPaymentDtoId.Value.Equals(Guid.Empty))
            {
                msg =
                    "Esta factura no tiene pago programado configurado. Podés pagarla en https://www.visanetpagos.com.uy. " +
                    "¡Programá tu pago sin miedo! Tú decidís el monto máximo que querés pagar y hasta cuándo querés " +
                    "pagarlo de forma programada.";
            }
            else
            {
                var paymentDate =
                    bill.ExpirationDate.AddDays(-serviceAssociatedDto.AutomaticPaymentDto.DaysBeforeDueDate);
                if (paymentDate.CompareTo(DateTime.Today) > 0)
                {
                    msg = "Se ha detectado una nueva factura que se pagará a partir del día " +
                        paymentDate.ToString("dd/MM/yyyy") + ".";
                }
                else
                {
                    //Caso en que la fecha de pago es igual o menor a la fecha actual (se debería pagar o haber pagado)
                    msg =
                        "No se pudo pagar esta factura. " +
                        "Intente pagarla de forma manual ingresando a https://www.visanetpagos.com.uy.";
                }
            }
            return msg;
        }

        private string BillAutomaticPaymentDisabledMessage()
        {
            var msg =
                "Esta factura no tiene pago programado configurado. " +
                "Podés pagarla en https://www.visanetpagos.com.uy.";

            return msg;
        }

        private string BillExceedsAmountMessage(ServiceAssociatedDto serviceAssociatedDto, ProcessedBillResultDto bill)
        {
            var message = string.Format(
                   "El pago programado de la factura no pudo ser completado porque " +
                   "el monto máximo configurado para el servicio {0} {1} es inferior al monto de la factura {2} {3}. " +
                   "Podés ajustar el monto máximo permitido o pagarla en https://www.visanetpagos.com.uy " +
                   "(revisá la fecha de vencimiento).",
                   bill.Currency.Equals("UYU") ? "$" : "U$S",
                   serviceAssociatedDto.AutomaticPaymentDto.Maximum,
                   bill.Currency.Equals("UYU") ? "$" : "U$S",
                   bill.Amount);

            return message;
        }

        private string BillExceedsQuotasMessage(ServiceAssociatedDto serviceAssociatedDto)
        {
            var message = string.Format(
                "El pago programado de la factura no pudo ser completado porque " +
                "se excedió el número máximo de facturas a pagar configurado ({0}). Podés volver a programar el pago o " +
                "pagarla en https://www.visanetpagos.com.uy (revisá la fecha de vencimiento).",
                serviceAssociatedDto.AutomaticPaymentDto.Quotas);

            return message;
        }

        private string BillExpiredMessage()
        {
            var message = string.Format(
                "El pago programado de la factura no pudo ser completado porque " +
                "la factura está vencida y el comercio/ente no permite su pago.");
            return message;
        }

        private string BillBinNotValidForServiceMessage(ServiceAssociatedDto serviceAssociatedDto)
        {
            var message = string.Format(
                "El pago programado de la factura no pudo ser completado porque " +
                "la tarjeta asociada al mismo {0} no está habilitada para realizar pagos para este servicio. " +
                "No se realizará más el pago programado para este servicio, podés volver a programarlo " +
                "en https://www.visanetpagos.com.uy asociándole una nueva tarjeta.",
                serviceAssociatedDto.DefaultCard.MaskedNumber);
            return message;
        }

        private string BillDeclinedDeleteAutomaticPaymentMessage(ServiceAssociatedDto serviceAssociatedDto,
            PaymentResultTypeDto result)
        {
            var message = "";
            switch (result)
            {
                case PaymentResultTypeDto.ServiceNotAllowsAutomaticPayment:
                    message = string.Format(
                        "El pago programado de la factura no pudo ser completado porque " +
                        "el servicio {0} ya no acepta esta modalidad de pago. " +
                        "Podés pagar tus facturas en https://www.visanetpagos.com.uy (revisá la fecha de vencimiento). " +
                        "No se realizará más el pago programado para este servicio.",
                        serviceAssociatedDto.ServiceDto.Name);
                    break;
                case PaymentResultTypeDto.InvalidCardDueDate:
                case PaymentResultTypeDto.CsExpiredCard:
                    message = string.Format(
                        "El pago programado de la factura no pudo ser completado porque " +
                        "la tarjeta asociada al mismo {0} está vencida (fecha de vencimiento: {1}). " +
                        "No se realizará más el pago programado para este servicio, podés volver a programarlo " +
                        "en https://www.visanetpagos.com.uy asociándole una nueva tarjeta.",
                        serviceAssociatedDto.DefaultCard.MaskedNumber,
                        serviceAssociatedDto.DefaultCard.DueDate.ToString("MM/yy"));
                    break;
                case PaymentResultTypeDto.InvalidCardToken:
                case PaymentResultTypeDto.InvalidCardBin:
                    message = string.Format(
                        "El pago programado de la factura no pudo ser completado porque " +
                        "la tarjeta asociada al mismo {0} no está habilitada para realizar pagos en VisaNetPagos. " +
                        "No se realizará más el pago programado para este servicio, podés volver a programarlo " +
                        "en https://www.visanetpagos.com.uy asociándole una nueva tarjeta.",
                        serviceAssociatedDto.DefaultCard.MaskedNumber);
                    break;
                case PaymentResultTypeDto.CsStolenCard:
                case PaymentResultTypeDto.CsInactiveCard:
                    message = string.Format(
                        "El pago programado de la factura no pudo ser completado porque " +
                        "la tarjeta asociada al mismo {0} no fue aprobada por el emisor. " +
                        "No se realizará más el pago programado para este servicio, podés volver a programarlo " +
                        "en https://www.visanetpagos.com.uy asociándole una nueva tarjeta.",
                        serviceAssociatedDto.DefaultCard.MaskedNumber);
                    break;
                case PaymentResultTypeDto.CsCardTypeNotAccepted:
                    message = string.Format(
                        "El pago programado de la factura no pudo ser completado porque " +
                        "la tarjeta asociada al mismo {0} no acepta este tipo de pago. " +
                        "No se realizará más el pago programado para este servicio, podés volver a programarlo " +
                        "en https://www.visanetpagos.com.uy asociándole una nueva tarjeta.",
                        serviceAssociatedDto.DefaultCard.MaskedNumber);
                    break;
                case PaymentResultTypeDto.CsInvalidAccountNumber:
                    message = string.Format(
                        "El pago programado de la factura no pudo ser completado porque " +
                        "la tarjeta asociada al mismo {0} no está permitida. " +
                        "No se realizará más el pago programado para este servicio, podés volver a programarlo " +
                        "en https://www.visanetpagos.com.uy asociándole una nueva tarjeta.",
                        serviceAssociatedDto.DefaultCard.MaskedNumber);
                    break;
            }
            return message;
        }

        private string BillGeneralErrorMessage()
        {
            return "El pago programado de la factura no pudo ser completado. " +
                   "Por favor comunicate con el call center en caso de dudas, o " +
                   "consultá tu factura en https://www.visanetpagos.com.uy para evitar vencimientos.";
        }


        //Additional message
        private string AdditionalMessage(ServiceAssociatedDto serviceAssociatedDto, PaymentResultTypeDto serviceResult,
            IEnumerable<KeyValuePair<Guid, ProcessedBillResultDto>> processedBillResults)
        {
            string additionalMessage = null;
            if (serviceResult == PaymentResultTypeDto.GetBillsException)
            {
                //Si falló al obtener facturas y tiene pago programado configurado agrego el mensaje
                if (serviceAssociatedDto.AutomaticPaymentDtoId.HasValue &&
                    !serviceAssociatedDto.AutomaticPaymentDtoId.Value.Equals(Guid.Empty))
                {
                    additionalMessage = GetBillsExceptionMessage(serviceAssociatedDto);
                }
            }
            else
            {
                if (serviceAssociatedDto.AutomaticPaymentDtoId.HasValue &&
                    !serviceAssociatedDto.AutomaticPaymentDtoId.Value.Equals(Guid.Empty) &&
                    !serviceAssociatedDto.AutomaticPaymentDto.UnlimitedQuotas)
                {
                    //Si se alcanzó pero no se superó la cantidad de pagos configurado agrego el mensaje
                    if (processedBillResults.All(kvp => kvp.Value.PaymentResultType != PaymentResultTypeDto.ExceededPaymentsCount))
                    {
                        var serviceAssociated = _serviceServiceAssosiate.GetById(serviceAssociatedDto.Id, s => s.AutomaticPayment);

                        if (serviceAssociated != null)
                        {
                            if (serviceAssociated.AutomaticPaymentDto != null)
                            {
                                if (serviceAssociated.AutomaticPaymentDto.QuotasDone == serviceAssociated.AutomaticPaymentDto.Quotas)
                                {
                                    additionalMessage = ReachedQuotasMessage(serviceAssociatedDto);
                                }
                            }
                            else
                            {
                                NLogLogger.LogEvent(NLogType.Info, "INFO - AdditionalMessage - AutomaticPaymentDto == null. ID: " + serviceAssociatedDto.Id);
                            }

                        }
                        else
                        {
                            NLogLogger.LogEvent(NLogType.Info, "INFO - AdditionalMessage - serviceAssociated == null. ID: " + serviceAssociatedDto.Id);
                        }
                    }
                }
            }
            return additionalMessage;
        }

        private string GetBillsExceptionMessage(ServiceAssociatedDto serviceAssociatedDto)
        {
            var msg = string.Format(
                "No se pudieron obtener las facturas correspondientes al servicio que tiene programado para {0} {1} en la " +
                "fecha {2}. VisaNetPagos consulta diariamente a este esta institución para conocer si Ud. tiene vencimiento " +
                "y/o deuda de esta factura. En el día de hoy, no nos fue posible acceder a esa información. " +
                "En caso de haber configurado su pago programado para 1 o 2 días previos al vencimiento, le sugerimos " +
                "que consulte manualmente a través del Portal https://www.visanetpagos.com.uy. " +
                "En caso de realizar su pago de forma manual a través del Portal, no se realizará el pago programado. " +
                "Si tiene alguna duda, por favor comuniquese con el call center.",
                serviceAssociatedDto.ServiceDto.Name,
                serviceAssociatedDto.Description,
                DateTime.Now.ToString("dd/MM/yyyy"));

            return msg;
        }

        private string ReachedQuotasMessage(ServiceAssociatedDto serviceAssociatedDto)
        {
            //Cuando se alcanza el último pago configurado se notifica (si se supera no se notifica)
            var msg = string.Format(
                "Se ha pagado por última vez en forma programada la cuota del servicio {0}{1}. " +
                "Aumentá la cantidad de cuotas para que se continúe pagando en forma programada.",
                serviceAssociatedDto.ServiceDto != null ? serviceAssociatedDto.ServiceDto.Name : "",
                !string.IsNullOrEmpty(serviceAssociatedDto.Description) ? " - " + serviceAssociatedDto.Description : "");

            return msg;
        }


        //Auxiliar
        private IDictionary<Guid, ProcessedBillResultDto> InitializeProcessedBillsDictionary(IEnumerable<BillDto> bills,
            ServiceAssociatedDto serviceAssociatedDto, IReadOnlyDictionary<Guid, PaymentResultTypeDto> billResultDictionary)
        {
            var processedBillResults = new Dictionary<Guid, ProcessedBillResultDto>();

            if (bills != null && bills.Any())
            {
                foreach (var billDto in bills.ToList())
                {
                    var processedBill = new ProcessedBillResultDto
                    {
                        Id = billDto.Id,
                        BillExternalId = billDto.BillExternalId,
                        BillType = BillType.NewBill,
                        ExpirationDate = billDto.ExpirationDate,
                        ExpirationDateMessage = billDto.ExpirationDate.ToString("dd/MM/yyyy"),
                        Currency = billDto.Currency.Equals("UYU") ? "$U" : "U$S",
                        Amount = billDto.Amount,
                        AlreadyNotified = true,
                        PaymentResultType = billResultDictionary[billDto.Id]
                    };

                    if (billDto.ExpirationDate.CompareTo(DateTime.Today.Date) < 0)
                    {
                        processedBill.ExpirationDateMessage += " (vencida)";
                        processedBill.BillType = BillType.ExpiredBill;
                    }
                    else if (billDto.ExpirationDate.AddDays(-serviceAssociatedDto.NotificationConfigDto.DaysBeforeDueDate)
                            .CompareTo(DateTime.Today.Date) <= 0)
                    {
                        processedBill.ExpirationDateMessage += " (vencimiento próximo)";
                        processedBill.BillType = BillType.AboutToExpireBill;
                    }

                    if (ShouldNotifyBill(serviceAssociatedDto, processedBill))
                    {
                        processedBillResults.Add(billDto.Id, processedBill);
                    }
                }
            }

            return processedBillResults;
        }

        private bool ShouldNotifyBill(ServiceAssociatedDto serviceAssociatedDto, ProcessedBillResultDto processedBill)
        {
            var notify = false;

            //TODO: Solucion provisoria: no se notifica el pago exitoso en este mail. A futuro otra solucion podria ser por ejemplo adjuntar el pdf en este mismo mail
            if (processedBill.PaymentResultType != PaymentResultTypeDto.Success)
            {
                if (processedBill.PaymentResultType == PaymentResultTypeDto.BillOk ||
                    processedBill.PaymentResultType == PaymentResultTypeDto.AutomaticPaymentDisabled)
                {
                    //Notificación de factura
                    if (processedBill.BillType == BillType.AboutToExpireBill)
                    {
                        //Factura por vencer
                        if (serviceAssociatedDto.NotificationConfigDto != null &&
                            serviceAssociatedDto.NotificationConfigDto.BeforeDueDateConfigDto != null &&
                            serviceAssociatedDto.NotificationConfigDto.BeforeDueDateConfigDto.Email == true)
                        {
                            notify = true;
                        }
                    }
                    else if (processedBill.BillType == BillType.ExpiredBill)
                    {
                        //Factura vencida
                        if (serviceAssociatedDto.NotificationConfigDto != null &&
                            serviceAssociatedDto.NotificationConfigDto.ExpiredBillDto != null &&
                            serviceAssociatedDto.NotificationConfigDto.ExpiredBillDto.Email == true)
                        {
                            notify = true;
                        }
                    }
                    else
                    {
                        //Factura nueva
                        if (serviceAssociatedDto.NotificationConfigDto != null &&
                            serviceAssociatedDto.NotificationConfigDto.NewBillDto != null &&
                            serviceAssociatedDto.NotificationConfigDto.NewBillDto.Email == true)
                        {
                            notify = true;
                        }
                    }
                }
                else
                {
                    //Resultado de pago programado de factura
                    notify = true;
                    //TODO: Ahora se notifica siempre sin controlar la flag de NotificationConfigDto.FailedAutomaticPaymentDto. Si se quiere empezar a controlar se agregaría acá
                }
            }

            return notify;
        }

    }
}