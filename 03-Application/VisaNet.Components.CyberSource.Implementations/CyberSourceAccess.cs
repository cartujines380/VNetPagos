using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CyberSource.CyberSourceWCF;
using Newtonsoft.Json;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;
using VisaNet.Components.CyberSource.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.ExtensionMethods;
using System.Net;

namespace CyberSource
{
    public class CyberSourceAccess : ICyberSourceAccess
    {
        private readonly ILoggerService _loggerService;
        private readonly IServiceService _serviceService;
        private readonly IServiceFixedNotification _serviceFixedNotification;
        private readonly IServiceApplicationUser _serviceApplicationUser;
        private readonly IServiceAnonymousUser _serviceAnonymousUser;
        private readonly IServiceParameters _serviceParameters;
        private readonly IRepositoryPayment _repositoryPayment;
        private readonly IRepositoryCard _repositoryCard;
        private readonly IServiceBank _serviceBank;
        private readonly IServiceVonData _serviceVonData;
        private readonly IRepositoryConciliationCybersource _repositoryConciliationCybersource;
        private readonly IServiceBin _serviceBin;

        private bool UseToken;
        private const string CodePaymentSuccess = "100";
        private const string CodePaymentDecisionManagerDecline = "481";
        private const string CodePaymentNotVoidableDecline = "246";

        public CyberSourceAccess(ILoggerService loggerService, IServiceService serviceService, IServiceFixedNotification serviceFixedNotification,
            IServiceApplicationUser serviceApplicationUser, IServiceAnonymousUser serviceAnonymousUser, IServiceParameters serviceParameters,
            IRepositoryPayment repositoryPayment, IRepositoryCard repositoryCard, IRepositoryConciliationCybersource repositoryConciliationCybersource,
            IServiceBank serviceBank, IServiceVonData serviceVonData, IServiceBin serviceBin)
        {
            _loggerService = loggerService;
            _serviceService = serviceService;
            _serviceFixedNotification = serviceFixedNotification;
            _serviceApplicationUser = serviceApplicationUser;
            _serviceAnonymousUser = serviceAnonymousUser;
            _serviceParameters = serviceParameters;
            _repositoryPayment = repositoryPayment;
            _repositoryCard = repositoryCard;
            _repositoryConciliationCybersource = repositoryConciliationCybersource;
            _serviceBank = serviceBank;
            _serviceVonData = serviceVonData;
            _serviceBin = serviceBin;
        }

        /// <summary>
        /// Method that generate a payment in cybersource.
        /// </summary>
        /// <param name="payment">Object with data like token, key, amount, merchandid, etc to make the payment</param>
        /// <param name="cyberSourceMerchantDefinedData"></param>
        /// <returns>Returns the request ID needed for cancelation</returns>
        public CyberSourceDataDto GeneratePayment(GeneratePayment payment, CyberSourceMerchantDefinedDataDto cyberSourceMerchantDefinedData)
        {
            var merchandId = string.Empty;
            var responseReasonCode = string.Empty;

            var voidObj = InitializeVoidPaymentObject(payment, cyberSourceMerchantDefinedData);
            var reverseObj = InitializeReversePaymentObject(payment, cyberSourceMerchantDefinedData);
            var isVoidObjectReady = false;
            var isReverseObjectReady = false;

            try
            {
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                if (payment.Quota > 1 && payment.AdditionalInfo.CardTypeDto != CardTypeDto.Credit)
                {
                    throw new QuotaNotAllowWithCardTypeException(CodeExceptions.CARDTYPE_NOT_ALLOWED_QUOTA_PAYMENT);
                }

                if (_serviceBank.IsQuotaForbidden(payment.Quota, payment.AdditionalInfo.BinValue))
                {
                    throw new QuotaNotAllowInBankException(CodeExceptions.BANK_DONOT_ALLOW_QUOTA, new[] { payment.Quota.ToString() });
                }

                if (payment.Quota > 1)
                {
                    throw new QuotaNotAllowInServiceException(CodeExceptions.BANK_DONOT_ALLOW_QUOTA, new[] { cyberSourceMerchantDefinedData.ServiceName });
                }

                var grandTotalAmount = payment.GrandTotalAmount.Replace('.', ',');

                if (double.Parse(grandTotalAmount) <= 0)
                {
                    throw new BillAmountNotAllowException(CodeExceptions.BILL_AMOUNT_NOT_ALLOW, new[] { cyberSourceMerchantDefinedData.OperationId,
                        payment.GrandTotalAmount });
                }

                var totalTaxedAmount = cyberSourceMerchantDefinedData.TotalTaxedAmount.Replace('.', ',');
                if (double.Parse(totalTaxedAmount) < 0 || double.Parse(totalTaxedAmount) > double.Parse(grandTotalAmount))
                {
                    throw new BillTaxedAmountNotAllow(CodeExceptions.BILL_TAXED_AMOUNT_NOT_ALLOW, new[] { cyberSourceMerchantDefinedData.OperationId,
                        payment.GrandTotalAmount });
                }

                merchandId = GetMerchandForThisEnviroment(payment.MerchandId.Trim());
                var field48 = GenerateField48(payment, cyberSourceMerchantDefinedData);

                voidObj.ServiceDto.MerchantId = merchandId;
                reverseObj.ServiceDto.MerchantId = merchandId;

                var request = new RequestMessage
                {
                    merchantID = merchandId,
                    //usar como identidicador de pago
                    merchantReferenceCode = payment.TransaccionId,
                    clientLibrary = ".NET WCF",
                    clientLibraryVersion = Environment.Version.ToString(),
                    clientEnvironment = Environment.OSVersion.Platform +
                                        Environment.OSVersion.Version.ToString(),
                    ccAuthService = new CCAuthService
                    {
                        run = "true",
                        //indica que se procese la autorización                        
                        //commerceIndicator = "install" //indica que el pago se realiza en forma recurrente ejecutada por el servidor

                    },
                    ccCaptureService = new CCCaptureService
                    {
                        run = "true" //indica que se procese la transacción
                    },
                    merchantDefinedData = new MerchantDefinedData
                    {
                        mddField = new MDDField[99]
                    },
                    issuer = new issuer
                    {
                        additionalData = field48
                    }
                    //installment = new Installment()
                    //{ 
                    //    totalAmount = cyberSourceMerchantDefinedData.Discount,
                    //    amount = billNumber,
                    //    sequence = cyberSourceMerchantDefinedData.DiscountApplyed
                    //}
                };

                if (!string.IsNullOrEmpty(payment.DeviceFingerprint))
                {
                    request.deviceFingerprintID = payment.DeviceFingerprint;
                }
                if (!string.IsNullOrEmpty(payment.CustomerIp))
                {
                    request.billTo = new BillTo
                    {
                        ipAddress = payment.CustomerIp,
                    };
                }
                if (payment.CustomerShippingAddresDto != null)
                {
                    request.shipTo = new ShipTo
                    {
                        street1 = payment.CustomerShippingAddresDto.Street,
                        buildingNumber = payment.CustomerShippingAddresDto.DoorNumber,
                        street2 = payment.CustomerShippingAddresDto.Complement,
                        street3 = payment.CustomerShippingAddresDto.Corner,
                        postalCode = payment.CustomerShippingAddresDto.PostalCode,
                        district = payment.CustomerShippingAddresDto.Neighborhood,
                        phoneNumber = payment.CustomerShippingAddresDto.Phone,
                        city = payment.CustomerShippingAddresDto.City,
                        country = string.IsNullOrEmpty(payment.CustomerShippingAddresDto.Country) ? null
                            : payment.CustomerShippingAddresDto.Country,
                    };
                }

                request.merchantDefinedData.mddField[0] = new MDDField() { id = "1", Value = cyberSourceMerchantDefinedData.ServiceType };
                request.merchantDefinedData.mddField[1] = new MDDField() { id = "2", Value = cyberSourceMerchantDefinedData.OperationTypeDto };
                request.merchantDefinedData.mddField[2] = new MDDField() { id = "3", Value = cyberSourceMerchantDefinedData.UserRegistered };
                request.merchantDefinedData.mddField[3] = new MDDField() { id = "4", Value = cyberSourceMerchantDefinedData.UserRegisteredDays };
                request.merchantDefinedData.mddField[4] = new MDDField() { id = "5", Value = cyberSourceMerchantDefinedData.ReferenceNumber1 };
                request.merchantDefinedData.mddField[5] = new MDDField() { id = "6", Value = cyberSourceMerchantDefinedData.ReferenceNumber2 };
                request.merchantDefinedData.mddField[6] = new MDDField() { id = "7", Value = cyberSourceMerchantDefinedData.ReferenceNumber3 };
                request.merchantDefinedData.mddField[7] = new MDDField() { id = "8", Value = cyberSourceMerchantDefinedData.ReferenceNumber4 };
                request.merchantDefinedData.mddField[8] = new MDDField() { id = "9", Value = cyberSourceMerchantDefinedData.ReferenceNumber5 };
                request.merchantDefinedData.mddField[9] = new MDDField() { id = "10", Value = cyberSourceMerchantDefinedData.ReferenceNumber6 };
                request.merchantDefinedData.mddField[10] = new MDDField() { id = "11", Value = cyberSourceMerchantDefinedData.RedirctTo };
                request.merchantDefinedData.mddField[11] = new MDDField() { id = "12", Value = cyberSourceMerchantDefinedData.DiscountApplyed };
                request.merchantDefinedData.mddField[12] = new MDDField() { id = "13", Value = cyberSourceMerchantDefinedData.TotalAmount };
                request.merchantDefinedData.mddField[13] = new MDDField() { id = "14", Value = cyberSourceMerchantDefinedData.TotalTaxedAmount };
                request.merchantDefinedData.mddField[14] = new MDDField() { id = "15", Value = cyberSourceMerchantDefinedData.Discount };
                request.merchantDefinedData.mddField[15] = new MDDField() { id = "16", Value = cyberSourceMerchantDefinedData.BillNumber };
                request.merchantDefinedData.mddField[16] = new MDDField() { id = "17", Value = cyberSourceMerchantDefinedData.AditionalNumberElectornicBill };
                request.merchantDefinedData.mddField[17] = new MDDField() { id = "18", Value = cyberSourceMerchantDefinedData.NameTh };
                request.merchantDefinedData.mddField[18] = new MDDField() { id = "19", Value = cyberSourceMerchantDefinedData.PaymentCount.ToString() };
                request.merchantDefinedData.mddField[19] = new MDDField() { id = "20", Value = cyberSourceMerchantDefinedData.UserMobile };
                request.merchantDefinedData.mddField[20] = new MDDField() { id = "21", Value = cyberSourceMerchantDefinedData.UserRegisteredAddress };
                request.merchantDefinedData.mddField[21] = new MDDField() { id = "22", Value = cyberSourceMerchantDefinedData.UserCi };
                request.merchantDefinedData.mddField[22] = new MDDField() { id = "23", Value = cyberSourceMerchantDefinedData.MerchandId };
                request.merchantDefinedData.mddField[23] = new MDDField() { id = "24", Value = cyberSourceMerchantDefinedData.CardBin };
                request.merchantDefinedData.mddField[24] = new MDDField() { id = "25", Value = cyberSourceMerchantDefinedData.ServiceName };
                request.merchantDefinedData.mddField[25] = new MDDField() { id = "26", Value = cyberSourceMerchantDefinedData.CallcenterUser };
                request.merchantDefinedData.mddField[26] = new MDDField() { id = "27", Value = cyberSourceMerchantDefinedData.OperationId };
                request.merchantDefinedData.mddField[27] = new MDDField() { id = "28", Value = payment.PaymentPlatform.ToString() };
                request.merchantDefinedData.mddField[28] = new MDDField() { id = "29", Value = cyberSourceMerchantDefinedData.TemporaryTransactionIdentifier };
                request.merchantDefinedData.mddField[29] = new MDDField() { id = "30", Value = LimitStringLength(cyberSourceMerchantDefinedData.ServiceId.ToString(), 99) };
                request.merchantDefinedData.mddField[30] = new MDDField() { id = "31", Value = LimitStringLength(cyberSourceMerchantDefinedData.UserId.ToString(), 99) };
                request.merchantDefinedData.mddField[31] = new MDDField() { id = "32", Value = LimitStringLength(cyberSourceMerchantDefinedData.CardId.ToString(), 99) };
                request.merchantDefinedData.mddField[32] = new MDDField() { id = "33", Value = LimitStringLength(cyberSourceMerchantDefinedData.GatewayId.ToString(), 99) };
                request.merchantDefinedData.mddField[33] = new MDDField() { id = "34", Value = LimitStringLength(cyberSourceMerchantDefinedData.DiscountObjId.ToString(), 99) };
                request.merchantDefinedData.mddField[34] = new MDDField() { id = "35", Value = LimitStringLength(cyberSourceMerchantDefinedData.PaymentTypeDto.ToString(), 99) };
                request.merchantDefinedData.mddField[35] = new MDDField() { id = "36", Value = LimitStringLength(cyberSourceMerchantDefinedData.BillExpirationDate, 99) };
                request.merchantDefinedData.mddField[36] = new MDDField() { id = "37", Value = LimitStringLength(cyberSourceMerchantDefinedData.BillDescription, 99) };
                request.merchantDefinedData.mddField[37] = new MDDField() { id = "38", Value = LimitStringLength(cyberSourceMerchantDefinedData.BillGatewayTransactionId, 99) };
                request.merchantDefinedData.mddField[38] = new MDDField() { id = "39", Value = LimitStringLength(cyberSourceMerchantDefinedData.BillSucivePreBillNumber, 99) };
                request.merchantDefinedData.mddField[39] = new MDDField() { id = "40", Value = LimitStringLength(cyberSourceMerchantDefinedData.BillFinalConsumer, 99) };
                request.merchantDefinedData.mddField[40] = new MDDField() { id = "41", Value = LimitStringLength(cyberSourceMerchantDefinedData.BillDiscount, 99) };
                request.merchantDefinedData.mddField[41] = new MDDField() { id = "42", Value = LimitStringLength(cyberSourceMerchantDefinedData.BillDateInitTransaccion, 99) };
                request.merchantDefinedData.mddField[42] = new MDDField() { id = "43", Value = LimitStringLength(cyberSourceMerchantDefinedData.BillGatewayTransactionBrouId, 99) };
                request.merchantDefinedData.mddField[43] = new MDDField() { id = "44", Value = string.Empty };
                request.merchantDefinedData.mddField[44] = new MDDField() { id = "45", Value = string.Empty };
                request.merchantDefinedData.mddField[45] = new MDDField() { id = "46", Value = string.Empty };
                request.merchantDefinedData.mddField[46] = new MDDField() { id = "47", Value = LimitStringLength(payment.Quota.ToString(), 99) };
                request.merchantDefinedData.mddField[47] = new MDDField() { id = "48", Value = payment.AdditionalInfo.CardTypeDto.ToString() };
                request.merchantDefinedData.mddField[48] = new MDDField() { id = "49", Value = string.Empty };
                request.merchantDefinedData.mddField[49] = new MDDField() { id = "50", Value = string.Empty };
                request.merchantDefinedData.mddField[50] = new MDDField() { id = "51", Value = string.Empty };
                request.merchantDefinedData.mddField[51] = new MDDField() { id = "52", Value = string.Empty };
                request.merchantDefinedData.mddField[52] = new MDDField() { id = "53", Value = field48 };

                request.purchaseTotals = new PurchaseTotals
                {
                    currency = payment.Currency,
                    grandTotalAmount = payment.GrandTotalAmount
                };
                request.recurringSubscriptionInfo = new RecurringSubscriptionInfo { subscriptionID = payment.Token };

                var proc = new TransactionProcessorClient();
                proc.ChannelFactory.Credentials.UserName.UserName = request.merchantID;
                proc.ChannelFactory.Credentials.UserName.Password = payment.Key.Trim();

                NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Intento realizar PAYMENT. Merchand id: {0}, monto {1} ",
                    payment.MerchandId, payment.GrandTotalAmount));

                //SE EJECUTA LA LLAMADA A CYBERSOURCE
                var reply = proc.runTransaction(request);
                responseReasonCode = reply.reasonCode;

                //Se termina de cargar los objetos para posible Void o Reverse
                voidObj.RequestId = reply.requestID;
                voidObj.IdTransaccion = reply.requestID;
                isVoidObjectReady = true;
                reverseObj.RequestId = reply.requestID;
                reverseObj.IdTransaccion = reply.requestID;
                isReverseObjectReady = true;

                CyberSourceDataDto result = null;
                if (reply.reasonCode.Equals(CodePaymentSuccess))
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format(
                        "Cybersource - Intento realizado PAYMENT. Request id: {0}, merchand id: {1}, monto {2} ",
                        reply.requestID, payment.MerchandId, payment.GrandTotalAmount));

                    var logMsg = string.Format(LogStrings.Payment_Cybersource_WCFPayment_Done, reply.requestID, payment.GrandTotalAmount, payment.Currency);
                    var logCallcenterMsg = string.Format(LogStrings.Payment_Cybersource_WCFPayment_Done, reply.requestID, payment.GrandTotalAmount, payment.Currency);
                    var csLogDataDto = new CyberSourceLogDataDto
                    {
                        AuthAmount = reply.ccAuthReply.amount,
                        AuthTime = reply.ccAuthReply.authorizedDateTime,
                        AuthCode = reply.ccAuthReply.authorizationCode,
                        AuthAvsCode = reply.ccAuthReply.avsCode,
                        AuthResponse = reply.ccAuthReply.reasonCode,
                        AuthTransRefNo = reply.ccAuthReply.transactionID,
                        Decision = reply.decision,
                        BillTransRefNo = reply.requestID,
                        PaymentToken = payment.Token,
                        ReasonCode = reply.reasonCode,
                        ReqAmount = reply.ccAuthReply.amount, //requestAmount viene vacio siempre
                        ReqCurrency = reply.purchaseTotals.currency,
                        TransactionId = reply.requestID,
                        TransactionType = TransactionType.Payment,
                        ReqTransactionUuid = payment.TransaccionId,
                        ReqReferenceNumber = payment.TransaccionId,
                        ReqTransactionType = VisaNet.Utilities.Cybersource.Enums.TransactionType.Sale,
                        PaymentPlatform = (PaymentPlatform)(int)payment.PaymentPlatform
                    };

                    //Se guarda Log + LogPaymentCyberSource
                    _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.CyberSource, payment.ApplicationUserId,
                        logMsg, logCallcenterMsg, null, csLogDataDto);

                    result = new CyberSourceDataDto()
                    {
                        AuthAmount = reply.ccAuthReply.amount,
                        AuthTime = reply.ccAuthReply.authorizedDateTime,
                        AuthCode = reply.ccAuthReply.authorizationCode,
                        AuthAvsCode = reply.ccAuthReply.avsCode,
                        AuthResponse = reply.ccAuthReply.reasonCode,
                        AuthTransRefNo = reply.ccAuthReply.transactionID,
                        Decision = reply.decision,
                        BillTransRefNo = reply.requestID,
                        PaymentToken = payment.Token,
                        ReasonCode = reply.reasonCode,
                        ReqAmount = reply.ccAuthReply.amount, //requestAmount viene vacio siempre
                        ReqCurrency = reply.purchaseTotals.currency,
                        TransactionId = reply.requestID,
                        ReqTransactionUuid = payment.TransaccionId,
                        ReqReferenceNumber = payment.TransaccionId,
                        ReqTransactionType = VisaNet.Utilities.Cybersource.Enums.TransactionType.Sale,
                    };
                    return result;
                }

                result = new CyberSourceDataDto() { ReasonCode = reply.reasonCode };
                NLogLogger.LogEvent(NLogType.Error, "Cybersource - Error Metodo GeneratePayment");
                NLogLogger.LogEvent(NLogType.Error, string.Format("Codigo de error: {0} - Transaccion: {1}", reply.reasonCode, reply.requestID));

                var logErrMsg = string.Format(LogStrings.Payment_Cybersource_Bad_ReasonCode, reply.reasonCode, reply.requestID);
                var logErrCallcenterMsg = string.Format(LogStrings.Payment_Cybersource_Bad_ReasonCode, reply.reasonCode, reply.requestID);
                var errCsLogDataDto = new CyberSourceLogDataDto
                {
                    AuthAmount = reply.ccAuthReply != null ? reply.ccAuthReply.amount : string.Empty,
                    AuthTime = reply.ccAuthReply != null ? reply.ccAuthReply.authorizedDateTime : string.Empty,
                    AuthCode = reply.ccAuthReply != null ? reply.ccAuthReply.authorizationCode : string.Empty,
                    AuthAvsCode = reply.ccAuthReply != null ? reply.ccAuthReply.avsCode : string.Empty,
                    AuthResponse = reply.ccAuthReply != null ? reply.ccAuthReply.reasonCode : string.Empty,
                    AuthTransRefNo = reply.ccAuthReply != null ? reply.ccAuthReply.transactionID : string.Empty,
                    Decision = reply.decision,
                    BillTransRefNo = reply.requestID,
                    ReasonCode = reply.reasonCode,
                    ReqAmount = reply.ccAuthReply != null ? reply.ccAuthReply.requestAmount : string.Empty,
                    ReqCurrency = reply.purchaseTotals != null ? reply.purchaseTotals.currency : string.Empty,
                    TransactionId = reply.requestID,
                    TransactionType = TransactionType.Payment,
                    ReqTransactionUuid = payment.TransaccionId,
                    ReqReferenceNumber = payment.TransaccionId,
                    ReqTransactionType = VisaNet.Utilities.Cybersource.Enums.TransactionType.Sale,
                    PaymentPlatform = (PaymentPlatform)(int)payment.PaymentPlatform
                };

                //Se guarda Log + LogPaymentCyberSource
                _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.CyberSource, payment.ApplicationUserId,
                    logErrMsg, logErrCallcenterMsg, null, errCsLogDataDto);

                if (reply.reasonCode.Equals(CodePaymentDecisionManagerDecline))
                {
                    NLogLogger.LogEvent(NLogType.Info, "Cybersource - El pago devolvio 481. Voy a realizar un reverso");
                    ExecuteReverse(reverseObj);
                }

                return result;

            }
            catch (TimeoutException e)
            {
                ExecuteVoidOrReverseIfNecessary(responseReasonCode, voidObj, reverseObj, isVoidObjectReady, isReverseObjectReady);

                //Se guarda Log
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource,
                    payment.ApplicationUserId,
                    string.Format(LogStrings.Payment_Cybersource_Timeout, payment.TransaccionId));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - TimeoutException Metodo GeneratePayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_TIMEOUT);
            }
            catch (FaultException e)
            {
                ExecuteVoidOrReverseIfNecessary(responseReasonCode, voidObj, reverseObj, isVoidObjectReady, isReverseObjectReady);

                //Se guarda Log
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource,
                    payment.ApplicationUserId,
                    string.Format(LogStrings.Payment_Cybersource_Fault, payment.TransaccionId));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - FaultException Metodo GeneratePayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_FAULT);
            }
            catch (CommunicationException e)
            {
                ExecuteVoidOrReverseIfNecessary(responseReasonCode, voidObj, reverseObj, isVoidObjectReady, isReverseObjectReady);

                //Se guarda Log
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource,
                    payment.ApplicationUserId,
                    string.Format(LogStrings.Payment_Cybersource_Communication, payment.TransaccionId));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - CommunicationException Metodo GeneratePayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_COMMUNICATION);
            }
            catch (QuotaNotAllowWithCardTypeException e)
            {
                throw;
            }
            catch (QuotaNotAllowInBankException e)
            {
                throw;
            }
            catch (QuotaNotAllowInServiceException e)
            {
                throw;
            }
            catch (BillTaxedAmountNotAllow e)
            {
                throw;
            }
            catch (BillAmountNotAllowException e)
            {
                throw;
            }
            catch (Exception e)
            {
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, payment.ApplicationUserId, string.Format(LogStrings.Payment_Cybersource_Error, payment.TransaccionId));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - Exception Metodo GeneratePayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_COMMUNICATION);
            }
        }

        public CyberSourceOperationData DeleteCard(DeleteCardDto delete)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            var param = _serviceParameters.GetParametersForCard();

            var request = new RequestMessage
            {
                merchantID = param.MerchantId, //merchandId,
                merchantReferenceCode = delete.RequestId,
                clientLibrary = ".NET WCF",
                clientLibraryVersion = Environment.Version.ToString(),
                clientEnvironment = Environment.OSVersion.Platform +
                                   Environment.OSVersion.Version.ToString(),
                recurringSubscriptionInfo = new RecurringSubscriptionInfo
                {
                    subscriptionID = delete.Token
                },
                paySubscriptionDeleteService = new PaySubscriptionDeleteService
                {
                    run = "true"
                }
            };

            try
            {
                var proc = new TransactionProcessorClient();
                proc.ChannelFactory.Credentials.UserName.UserName = param.MerchantId;
                proc.ChannelFactory.Credentials.UserName.Password = param.CybersourceTransactionKey.Trim();

                NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Intento realizar Delete. Request id: {0}, merchand id: {1} ", delete.RequestId, param.MerchantId));
                var reply = proc.runTransaction(request);

                if (reply.reasonCode.Equals(CodePaymentSuccess))
                {

                    NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Se realizo un Delete. Request id: {0}, merchand id: {1} ", delete.RequestId, param.MerchantId));
                    _loggerService.CreateLog(LogType.Info,
                        LogOperationType.Void,
                        LogCommunicationType.CyberSource,
                        delete.UserType,
                        delete.UserId,
                        string.Format(LogStrings.Payment_Cybersource_Cancel_Done, delete.RequestId),
                        string.Format(LogStrings.CallCenter_Payment_Cancel_Success, delete.RequestId),
                        null,
                        new CyberSourceLogDataDto
                        {
                            Decision = reply.decision,
                            BillTransRefNo = delete.RequestId,
                            PaymentToken = delete.Token,
                            ReasonCode = reply.reasonCode,
                            TransactionId = reply.requestID,
                            TransactionType = TransactionType.CardDelete,
                        });
                    var csData = new CyberSourceOperationData()
                    {
                        DeleteData = new CsResponseDeleteData()
                        {
                            DeleteRequestId = reply.requestID,
                            DeleteResponseCode = int.Parse(reply.reasonCode),
                        },
                    };
                    return csData;
                }

                NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Error en Delete. Request id: {0}, merchand id: {1}, Codigo de error: {2}, Transaccion: {3}", delete.RequestId, param.MerchantId, reply.reasonCode, reply.requestID));
                //NotificationFix(reply.reasonCode, "VOID", cancel.RequestId, reply.requestID, reply.decision, cancel.MerchandId);
                _loggerService.CreateLog(LogType.Error,
                    LogOperationType.Void,
                    LogCommunicationType.CyberSource,
                    delete.UserType,
                    delete.UserId,
                    string.Format(LogStrings.Payment_Cybersource_Void_Error, reply.reasonCode, delete.IdTransaccion),
                    null,
                    new CyberSourceLogDataDto
                    {
                        Decision = reply.decision,
                        BillTransRefNo = delete.RequestId,
                        PaymentToken = delete.Token,
                        ReasonCode = reply.reasonCode,
                        TransactionId = reply.requestID,
                        TransactionType = TransactionType.CardDelete,

                    });

                var result = new CyberSourceOperationData()
                {
                    DeleteData = new CsResponseDeleteData()
                    {
                        DeleteRequestId = reply.requestID,
                        DeleteResponseCode = int.Parse(reply.reasonCode),
                    },
                };
                return result;
            }
            catch (TimeoutException e)
            {
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, delete.UserType, delete.UserId, string.Format(LogStrings.Payment_Cybersource_Void_Error,
                        e.Message, delete.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - TimeoutException Metodo DeleteCard - Servicio {0}", param.MerchantId));
                NLogLogger.LogEvent(e);

                throw new CybersourceException(CodeExceptions.CYBERSOURCE_TIMEOUT);
            }
            catch (FaultException e)
            {
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, delete.UserType, delete.UserId, string.Format(LogStrings.Payment_Cybersource_Void_Error,
                        e.Message, delete.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - FaultException Metodo DeleteCard - Servicio {0}", param.MerchantId));
                NLogLogger.LogEvent(e);

                throw new CybersourceException(CodeExceptions.CYBERSOURCE_FAULT);
            }
            catch (CommunicationException e)
            {
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, delete.UserType, delete.UserId, string.Format(LogStrings.Payment_Cybersource_Void_Error,
                        e.Message, delete.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - CommunicationException Metodo DeleteCard - Servicio {0}", param.MerchantId));
                NLogLogger.LogEvent(e);

                throw new CybersourceException(CodeExceptions.CYBERSOURCE_COMMUNICATION);
            }
            catch (Exception e)
            {
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, delete.UserType, delete.UserId, string.Format(LogStrings.Payment_Cybersource_Void_Error,
                        e.Message, delete.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - Exception Metodo DeleteCard - Servicio {0}", param.MerchantId));
                NLogLogger.LogEvent(e);

                throw new CybersourceException(CodeExceptions.GENERAL_COMUNICATION_ERROR);
            }
        }

        /// <summary>
        /// Method that cancel a payment in cybersource.
        /// </summary>
        /// <param name="cancel"></param>
        /// <returns>Returns true if the cancellation was made</returns>
        public CyberSourceOperationData VoidPayment(CancelPayment cancel)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var serviceDto = cancel.ServiceDto ?? _serviceService.GetById(cancel.ServiceId);
            if (cancel.ServiceDto == null)
                cancel.ServiceDto = serviceDto;
            var merchandId = GetMerchandForThisEnviroment(serviceDto.MerchantId.Trim());

            var request = new RequestMessage
            {
                merchantID = merchandId,
                merchantReferenceCode = cancel.RequestId,
                clientLibrary = ".NET WCF",
                clientLibraryVersion = Environment.Version.ToString(),
                clientEnvironment = Environment.OSVersion.Platform +
                                    Environment.OSVersion.Version.ToString(),
                voidService = new VoidService
                {
                    run = "true",
                    voidRequestID = cancel.RequestId,
                    //saco el token
                    //voidRequestToken = cancel.Token
                }
            };

            try
            {
                var proc = new TransactionProcessorClient();
                proc.ChannelFactory.Credentials.UserName.UserName = merchandId;
                proc.ChannelFactory.Credentials.UserName.Password = serviceDto.CybersourceTransactionKey.Trim();

                NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Intento realizar VOID. Request id: {0}, merchand id: {1} , monto {2} ", cancel.RequestId, serviceDto.MerchantId, cancel.Amount));
                var reply = proc.runTransaction(request);

                NotificationFix(reply, cancel, null, "VOID", null);

                if (reply.reasonCode.Equals(CodePaymentSuccess))
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Se realizo un VOID. Request id: {0}, merchand id: {1} , monto {2} ", cancel.RequestId, serviceDto.MerchantId, cancel.Amount));
                    _loggerService.CreateLog(LogType.Info,
                        LogOperationType.Void,
                        LogCommunicationType.CyberSource,
                        cancel.UserType,
                        cancel.UserId,
                        string.Format(LogStrings.Payment_Cybersource_Cancel_Done, cancel.RequestId),
                        string.Format(LogStrings.CallCenter_Payment_Cancel_Success, cancel.RequestId),
                        null,
                        new CyberSourceLogDataDto
                        {
                            AuthAmount = reply.voidReply.amount,
                            Decision = reply.decision,
                            BillTransRefNo = cancel.RequestId,
                            PaymentToken = cancel.Token,
                            ReasonCode = reply.reasonCode,
                            TransactionId = reply.requestID,
                            TransactionType = TransactionType.Void,
                            PaymentPlatform = (PaymentPlatform)(int)cancel.PaymentPlatform
                        });
                    var csData = new CyberSourceOperationData() { VoidData = new CsResponseData() { PaymentRequestId = reply.requestID, PaymentResponseCode = int.Parse(reply.reasonCode) } };
                    //Luego de realizar un void tengo que hace run reverso. Si se cae, igual hago el void?
                    try
                    {
                        //EL REVERSO SE HACE AUTOMATICAMENTE EN CS. SE COMENTA LA ACCION

                        //var rData = ReversePayment(new RefundPayment()
                        //{
                        //    Amount = cancel.Amount,
                        //    UserId = cancel.UserId,
                        //    Currency = cancel.Currency,
                        //    UserType = cancel.UserType,
                        //    PaymentPlatform = cancel.PaymentPlatform,
                        //    RequestId = cancel.RequestId,
                        //    ServiceDto = serviceDto,
                        //    ServiceId = cancel.ServiceId,
                        //    Token = cancel.Token
                        //});
                        //csData.ReversalData = rData.ReversalData;
                        NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - El REVESARL para la trns {0} se realizo automaticamente del lado de CS.", cancel.RequestId));
                        csData.ReversalData = new CsResponseData()
                        {
                            PaymentResponseCode = (int)CybersourceMsg.Accepted,
                        };

                    }
                    catch (Exception e)
                    {

                    }

                    return csData;
                }
                NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Error en VOID. Request id: {0}, merchand id: {1}, monto {2},Codigo de error: {3}, Transaccion: {4}", cancel.RequestId, serviceDto.MerchantId, cancel.Amount, reply.reasonCode, reply.requestID));
                //NotificationFix(reply.reasonCode, "VOID", cancel.RequestId, reply.requestID, reply.decision, cancel.MerchandId);
                _loggerService.CreateLog(LogType.Error,
                    LogOperationType.Void,
                    LogCommunicationType.CyberSource,
                    cancel.UserType,
                    cancel.UserId,
                    string.Format(LogStrings.Payment_Cybersource_Void_Error, reply.reasonCode, cancel.IdTransaccion),
                    null,
                    new CyberSourceLogDataDto
                    {
                        AuthAmount = reply.voidReply != null ? reply.voidReply.amount : string.Empty,
                        Decision = reply.decision,
                        BillTransRefNo = cancel.RequestId,
                        PaymentToken = cancel.Token,
                        ReasonCode = reply.reasonCode,
                        TransactionId = reply.requestID,
                        TransactionType = TransactionType.Void,
                        PaymentPlatform = (PaymentPlatform)(int)cancel.PaymentPlatform
                    });

                var result = new CyberSourceOperationData() { VoidData = new CsResponseData() { PaymentRequestId = reply.requestID, PaymentResponseCode = int.Parse(reply.reasonCode) } };

                //HACER UN REFUND
                if (reply.reasonCode.Equals(CodePaymentNotVoidableDecline))
                {
                    var refundData = RefundPayment(new RefundPayment
                    {
                        Amount = cancel.Amount,
                        UserId = cancel.UserId,
                        Currency = cancel.Currency,
                        UserType = cancel.UserType,
                        PaymentPlatform = cancel.PaymentPlatform,
                        RequestId = cancel.RequestId,
                        ServiceDto = serviceDto,
                        ServiceId = cancel.ServiceId,
                        Token = cancel.Token,
                        IdOperation = cancel.IdOperation,
                        IdTransaccion = cancel.IdTransaccion
                    });
                    result.RefundData = refundData.RefundData;
                }

                return result;
            }
            catch (TimeoutException e)
            {
                //Console.WriteLine("TimeoutException: " + e.Message + "\n" + e.StackTrace);
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, cancel.UserType, cancel.UserId, string.Format(LogStrings.Payment_Cybersource_Void_Error,
                        e.Message, cancel.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - TimeoutException Metodo VoidPayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                NotificationFix(null, cancel, null, "VOID", e);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_TIMEOUT);
            }
            catch (FaultException e)
            {
                //Console.WriteLine("FaultException: " + e.Message + "\n" + e.StackTrace);
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, cancel.UserType, cancel.UserId, string.Format(LogStrings.Payment_Cybersource_Void_Error,
                        e.Message, cancel.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - FaultException Metodo VoidPayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                NotificationFix(null, cancel, null, "VOID", e);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_FAULT);
            }
            catch (CommunicationException e)
            {
                //Console.WriteLine("CommunicationException: " + e.Message + "\n" + e.StackTrace);
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, cancel.UserType, cancel.UserId, string.Format(LogStrings.Payment_Cybersource_Void_Error,
                        e.Message, cancel.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - CommunicationException Metodo VoidPayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                NotificationFix(null, cancel, null, "VOID", e);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_COMMUNICATION);
            }
            catch (Exception e)
            {
                //Console.WriteLine("CommunicationException: " + e.Message + "\n" + e.StackTrace);
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, cancel.UserType, cancel.UserId, string.Format(LogStrings.Payment_Cybersource_Void_Error,
                        e.Message, cancel.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - Exception Metodo VoidPayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                NotificationFix(null, cancel, null, "VOID", e);
                throw new CybersourceException(CodeExceptions.GENERAL_COMUNICATION_ERROR);
            }
        }

        public CyberSourceOperationData RefundPayment(RefundPayment refund)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var serviceDto = refund.ServiceDto ?? _serviceService.GetById(refund.ServiceId);
            if (refund.ServiceDto == null)
                refund.ServiceDto = serviceDto;

            var merchandId = GetMerchandForThisEnviroment(serviceDto.MerchantId.Trim());

            var request = new RequestMessage
            {
                merchantID = merchandId,
                merchantReferenceCode = refund.RequestId,
                clientLibrary = ".NET WCF",
                clientLibraryVersion = Environment.Version.ToString(),
                clientEnvironment = Environment.OSVersion.Platform +
                                    Environment.OSVersion.Version.ToString(),

                ccCreditService = new CCCreditService
                {
                    run = "true",
                    captureRequestID = refund.RequestId,
                },
                purchaseTotals = new PurchaseTotals
                {
                    grandTotalAmount = refund.Amount
                }
            };

            try
            {
                var proc = new TransactionProcessorClient();
                proc.ChannelFactory.Credentials.UserName.UserName = merchandId;
                proc.ChannelFactory.Credentials.UserName.Password = serviceDto.CybersourceTransactionKey.Trim();
                NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Intento realizar REFUND. Request id: {0}, merchand id: {1}, monto {2} ", refund.RequestId, serviceDto.MerchantId, refund.Amount));
                var reply = proc.runTransaction(request);

                NotificationFix(reply, null, refund, "REFUND", null);

                if (reply.reasonCode.Equals(CodePaymentSuccess))
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Se realizo un REFUND. Request id: {0}, merchand id: {1}, monto {2} ", refund.RequestId, serviceDto.MerchantId, refund.Amount));
                    _loggerService.CreateLog(LogType.Info,
                        LogOperationType.Refund,
                        LogCommunicationType.CyberSource,
                        refund.UserType,
                        refund.UserId,
                        string.Format(LogStrings.Payment_Cybersource_Cancel_Done, refund.RequestId),
                        string.Format(LogStrings.CallCenter_Payment_Cancel_Success, refund.RequestId),
                        null,
                        new CyberSourceLogDataDto
                        {
                            AuthAmount = reply.voidReply != null ? reply.voidReply.amount : string.Empty,
                            Decision = reply.decision,
                            BillTransRefNo = refund.RequestId,
                            PaymentToken = refund.Token,
                            ReasonCode = reply.reasonCode,
                            TransactionId = reply.requestID,
                            TransactionType = TransactionType.Refund,
                            //ReqCurrency = reply.voidReply != null ? reply.voidReply.currency : string.Empty,
                            ReqCurrency = reply.purchaseTotals.currency,
                            ReqAmount = reply.ccCreditReply.amount,
                            PaymentPlatform = (PaymentPlatform)(int)refund.PaymentPlatform
                        });
                }
                else
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Error en REFUND. Request id: {0}, merchand id: {1}, monto {2},Codigo de error: {3}, Transaccion: {4}", refund.RequestId, serviceDto.MerchantId, refund.Amount, reply.reasonCode, reply.requestID));
                    _loggerService.CreateLog(LogType.Error,
                        LogOperationType.Refund,
                        LogCommunicationType.CyberSource,
                        refund.UserType,
                        refund.UserId,
                        string.Format(LogStrings.Payment_Cybersource_Refund_Error, reply.reasonCode, refund.IdTransaccion),
                        null,
                        new CyberSourceLogDataDto
                        {
                            AuthAmount = reply.voidReply != null ? reply.voidReply.amount : string.Empty,
                            Decision = reply.decision,
                            BillTransRefNo = refund.RequestId,
                            PaymentToken = refund.Token,
                            ReasonCode = reply.reasonCode,
                            TransactionId = reply.requestID,
                            TransactionType = TransactionType.Refund,
                            PaymentPlatform = (PaymentPlatform)(int)refund.PaymentPlatform
                        });
                }
                return new CyberSourceOperationData() { RefundData = new CsResponseData() { PaymentRequestId = reply.requestID, PaymentResponseCode = int.Parse(reply.reasonCode) } };
            }
            catch (TimeoutException e)
            {
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, refund.UserType, refund.UserId, string.Format(LogStrings.Payment_Cybersource_Refund_Error, e.Message, refund.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - TimeoutException Metodo RefundPayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                NotificationFix(null, null, refund, "REFUND", null);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_TIMEOUT);
            }
            catch (FaultException e)
            {
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, refund.UserType, refund.UserId, string.Format(LogStrings.Payment_Cybersource_Refund_Error, e.Message, refund.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - FaultException Metodo RefundPayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                NotificationFix(null, null, refund, "REFUND", null);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_FAULT);
            }
            catch (CommunicationException e)
            {
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, refund.UserType, refund.UserId, string.Format(LogStrings.Payment_Cybersource_Refund_Error, e.Message, refund.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - CommunicationException Metodo RefundPayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                NotificationFix(null, null, refund, "REFUND", null);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_COMMUNICATION);
            }
            catch (Exception e)
            {
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, refund.UserType, refund.UserId, string.Format(LogStrings.Payment_Cybersource_Refund_Error, e.Message, refund.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - Exception Metodo RefundPayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                NotificationFix(null, null, refund, "REFUND", null);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_COMMUNICATION);
            }
        }

        public CyberSourceOperationData ReversePayment(RefundPayment refund)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var serviceDto = refund.ServiceDto ?? _serviceService.GetById(refund.ServiceId);
            if (refund.ServiceDto == null)
                refund.ServiceDto = serviceDto;
            var merchandId = GetMerchandForThisEnviroment(serviceDto.MerchantId.Trim());

            refund.ServiceDto = serviceDto;
            var request = new RequestMessage
            {
                merchantID = merchandId,
                merchantReferenceCode = refund.RequestId,
                clientLibrary = ".NET WCF",
                clientLibraryVersion = Environment.Version.ToString(),
                clientEnvironment = Environment.OSVersion.Platform +
                                    Environment.OSVersion.Version.ToString(),

                ccAuthReversalService = new CCAuthReversalService()
                {
                    run = "true",
                    authRequestID = refund.RequestId,
                },
                purchaseTotals = new PurchaseTotals
                {
                    grandTotalAmount = refund.Amount,
                    currency = refund.Currency
                }
            };

            try
            {
                var proc = new TransactionProcessorClient();
                proc.ChannelFactory.Credentials.UserName.UserName = merchandId;
                proc.ChannelFactory.Credentials.UserName.Password = serviceDto.CybersourceTransactionKey.Trim();
                NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Intento realizar REVESARL. Request id: {0}, merchand id: {1}, monto {2} ", refund.RequestId, serviceDto.MerchantId, refund.Amount));
                var reply = proc.runTransaction(request);

                NotificationFix(reply, null, refund, "REVERSE", null);

                if (reply.reasonCode.Equals(CodePaymentSuccess))
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Se realizo un REVESARL. Request id: {0}, merchand id: {1}, monto {2} ", refund.RequestId, serviceDto.MerchantId, refund.Amount));
                    _loggerService.CreateLog(LogType.Info,
                        LogOperationType.Reverse,
                        LogCommunicationType.CyberSource,
                        refund.UserType,
                        refund.UserId,
                        string.Format(LogStrings.Payment_Cybersource_Reverse_Done, refund.RequestId),
                        string.Format(LogStrings.CallCenter_Payment_Reverse_Success, refund.RequestId),
                        null,
                        new CyberSourceLogDataDto
                        {
                            AuthAmount = reply.voidReply != null ? reply.voidReply.amount : string.Empty,
                            Decision = reply.decision,
                            BillTransRefNo = refund.RequestId,
                            PaymentToken = refund.Token,
                            ReasonCode = reply.reasonCode,
                            TransactionId = reply.requestID,
                            TransactionType = TransactionType.Reverse,
                            ReqAmount = reply.ccAuthReversalReply.amount,
                            ReqCurrency = reply.purchaseTotals.currency,
                            PaymentPlatform = (PaymentPlatform)(int)refund.PaymentPlatform
                        });
                }
                else
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Error en REVERSAL. Request id: {0}, merchand id: {1}, monto {2},Codigo de error: {3}, Transaccion: {4}", refund.RequestId, serviceDto.MerchantId, refund.Amount, reply.reasonCode, reply.requestID));
                    _loggerService.CreateLog(LogType.Error,
                        LogOperationType.Reverse,
                        LogCommunicationType.CyberSource,
                        refund.UserType,
                        refund.UserId,
                        string.Format(LogStrings.Payment_Cybersource_Reverse_Error, reply.reasonCode, refund.IdTransaccion),
                        null,
                        new CyberSourceLogDataDto
                        {
                            AuthAmount = reply.voidReply != null ? reply.voidReply.amount : string.Empty,
                            Decision = reply.decision,
                            BillTransRefNo = refund.RequestId,
                            PaymentToken = refund.Token,
                            ReasonCode = reply.reasonCode,
                            TransactionId = reply.requestID,
                            TransactionType = TransactionType.Reverse,
                            PaymentPlatform = (PaymentPlatform)(int)refund.PaymentPlatform
                        });
                }
                return new CyberSourceOperationData() { ReversalData = new CsResponseData() { PaymentRequestId = reply.requestID, PaymentResponseCode = int.Parse(reply.reasonCode) } };
            }
            catch (TimeoutException e)
            {
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, refund.UserType, refund.UserId, string.Format(LogStrings.Payment_Cybersource_Reverse_Error, e.Message, refund.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - TimeoutException Metodo ReversePayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                NotificationFix(null, null, refund, "REVERSE", null);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_TIMEOUT);
            }
            catch (FaultException e)
            {
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, refund.UserType, refund.UserId, string.Format(LogStrings.Payment_Cybersource_Reverse_Error, e.Message, refund.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - FaultException Metodo ReversePayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                NotificationFix(null, null, refund, "REVERSE", null);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_FAULT);
            }
            catch (CommunicationException e)
            {
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, refund.UserType, refund.UserId, string.Format(LogStrings.Payment_Cybersource_Reverse_Error, e.Message, refund.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - CommunicationException Metodo ReversePayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                NotificationFix(null, null, refund, "REVERSE", null);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_COMMUNICATION);
            }
            catch (Exception e)
            {
                _loggerService.CreateLog(LogType.Error, LogOperationType.Cybersource, LogCommunicationType.CyberSource, refund.UserType, refund.UserId, string.Format(LogStrings.Payment_Cybersource_Reverse_Error, e.Message, refund.IdTransaccion));
                NLogLogger.LogEvent(NLogType.Error, string.Format("Cybersource - Exception Metodo ReversePayment - Servicio {0}", merchandId));
                NLogLogger.LogEvent(e);
                NotificationFix(null, null, refund, "REVERSE", null);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_COMMUNICATION);
            }
        }

        public async Task<List<ConciliationCybersourceDto>> GenerateConciliation(DateTime from, DateTime to)
        {
            try
            {
                var errors = new List<String>();

                var originalFrom = new DateTime(from.Year, from.Month, from.Day);
                var dayList = ObtainData(from, errors);
                var title = "Obtención reporte Cybersource" + (errors.Any() ? " Error" : " Ok");
                var desc = "";

                if (errors.Any())
                {
                    desc = "El proceso batch de obtencion de transacciones desde Cybersource no termino bien. <br /><br />";
                    desc += string.Format("Se consultaros las transacciones entre los dias {0} y {1}. Las consultas que fallaron son las siguientes: <br />",
                        originalFrom.ToString("d"), to.ToString("d"));
                    desc += string.Join("<br />", errors);
                }
                else
                {
                    desc = "El proceso batch de obtencion de transacciones desde Cybersource termino bien. <br /><br />";
                    desc += string.Format("Se consultaros las transacciones entre los dias {0} y {1}. <br />", originalFrom.ToString("d"), to.ToString("d"));
                }

                _serviceFixedNotification.Create(new FixedNotificationDto()
                {
                    Category = FixedNotificationCategoryDto.Conciliation,
                    DateTime = DateTime.Now,
                    Level = errors.Any() ? FixedNotificationLevelDto.Warning : FixedNotificationLevelDto.Info,
                    Description = title,
                    Detail = desc
                });

                return dayList;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "Cybersource - Exception Metodo GenerateConciliation");
                NLogLogger.LogEvent(e);
                throw;
            }
        }

        private List<ConciliationCybersourceDto> ObtainData(DateTime day, List<String> errorsList, Guid? serviceId = null)
        {
            var list = new List<ConciliationCybersourceDto>();
            NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Obtengo las transacciones de Cybersource para la fecha {0}", day.ToString("G")));
            var request = "";
            try
            {
                var servs = _serviceService.AllNoTracking(null, x => x.MerchantId != null && !x.MerchantId.Equals("")).OrderBy(x => x.Name);
                var mids = new List<string>();
                if (serviceId == null)
                {
                    mids = servs.Select(s => s.MerchantId).Distinct().ToList();
                }
                else
                {
                    var service = servs.FirstOrDefault(s => s.Id.Equals(serviceId));
                    if (service != null)
                        mids.Add(service.MerchantId);
                }

                NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Hay {0} mids", mids.Count));
                var midsArray = mids.ToArray();
                midsArray = midsArray.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                Parallel.ForEach(midsArray, new ParallelOptions { MaxDegreeOfParallelism = 20 }, m =>
                {
                    var listPerMerchandId = AnaliceData(m, day);
                    list.AddRange(listPerMerchandId);
                });
                _repositoryConciliationCybersource.Save();
                return list;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                NLogLogger.LogEvent(NLogType.Error, "Cybersource - Exception Metodo ObtainData");
                NLogLogger.LogEvent(e);
                if (errorsList != null)
                    errorsList.Add(string.Format("{0}, Error {1}", request, e.Message));
                throw;
            }
        }

        private List<ConciliationCybersourceDto> AnaliceData(string merchandId, DateTime day)
        {
            var list = new List<ConciliationCybersourceDto>();
            var request = string.Empty;
            var trnsCount = 0;
            try
            {
                var reportUrl = ConfigurationManager.AppSettings["reportUrl"];
                var userName = ConfigurationManager.AppSettings["userName"];
                var password = ConfigurationManager.AppSettings["password"];


                var reportName = ConfigurationManager.AppSettings["reportConciliationName"];
                var reportFormat = ConfigurationManager.AppSettings["reportConciliationFormat"];

                request = string.Format("{0}{1}/{2}/{3}/{4}/{5}.{6}", reportUrl, day.Year.ToString("0000"), day.Month.ToString("00"), day.Day.ToString("00"), merchandId, reportName, reportFormat);
                request = request.Trim();
                var userNameBase64 = EncodeTo64(string.Format("{0}:{1}", userName, password));

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

                var httpClient = new HttpClient();
                var requestH = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(request),
                };

                requestH.Headers.Add("Authorization", "Basic " + userNameBase64);
                requestH.Headers.Add("Accept-Charset", "UTF-8");

                var response = httpClient.SendAsync(requestH).Result;

                NLogLogger.LogEvent(NLogType.Info, string.Format("    Cybersource Response - Servicio {0}, Code: {1}, Desc: {2}, Url: {3}",
                    merchandId, response.StatusCode, response.ReasonPhrase, request));



                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;

                    var arr = result.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    var a = false;
                    //primeras dos pos son headers
                    int pos = 0;
                    foreach (var item in arr)
                    {
                        if (pos > 1 && !String.IsNullOrEmpty(item))
                        {
                            var intent = 0;
                            try
                            {
                                trnsCount = trnsCount + AnaliceOneRaw(item, intent);
                            }
                            catch (Exception exception)
                            {
                                NLogLogger.LogEvent(NLogType.Info, "Cybersource - Exception Metodo AnaliceOneRaw intento " + intent + " en requestUrl " + request);
                                NLogLogger.LogEvent(NLogType.Error, "Cybersource - Exception Metodo AnaliceOneRaw");
                                NLogLogger.LogEvent(exception);
                                if (intent < 5)
                                {
                                    intent = intent + 1;
                                    AnaliceOneRaw(item, intent++);
                                }
                            }
                        }
                        pos++;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                NLogLogger.LogEvent(NLogType.Info, "Cybersource - Exception Metodo AnaliceData en requestUrl " + request);
                NLogLogger.LogEvent(NLogType.Error, "Cybersource - Exception Metodo AnaliceData");
                NLogLogger.LogEvent(exception);
            }
            return list;
        }

        private int AnaliceOneRaw(string item, int intent)
        {
            var result = 0;
            // extract the fields
            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            var paymentRCode = false;
            var refundRCode = false;
            var concilitation = new ConciliationCybersource()
            {
                TransactionType = TransactionType.Payment
            };
            concilitation.PaymentDone = true;
            var splitedValues = CSVParser.Split(item);
            // clean up the fields (remove " and leading spaces)
            for (int i = 0; i < splitedValues.Length; i++)
            {
                if (i == 1 || i == 2 || i == 3 || i == 4 || i == 5 || i == 6 || i == 9 || i == 12 || i == 15 ||
                                i == 94 || i == 95 || i == 125)
                {
                    splitedValues[i] = splitedValues[i].TrimStart(' ', '"');
                    splitedValues[i] = splitedValues[i].TrimEnd('"');
                    switch (i)
                    {
                        case 1:
                            concilitation.RequestId = splitedValues[i];
                            break;
                        case 2:
                            var str = splitedValues[i];
                            concilitation.DateString = str;
                            str = str.Replace('T', ' ');
                            var rest = int.Parse(str.Substring(20, 2));
                            str = str.Substring(0, 19);

                            if (!String.IsNullOrEmpty(str))
                            {
                                concilitation.Date =
                                                DateTime.ParseExact(str, "yyyy-MM-dd HH:mm:ss",
                                                    System.Globalization.CultureInfo.InvariantCulture)
                                                    .AddHours(-rest);
                            }
                            break;
                        case 3:
                            concilitation.MerchantReferenceNumber = splitedValues[i];
                            break;
                        case 4:
                            concilitation.MerchandId = splitedValues[i];
                            break;
                        case 5:
                            concilitation.IcsApplications = splitedValues[i];
                            break;
                        case 6:
                            //var authRcode = String.IsNullOrEmpty(splitedValues[i])
                            //    ? 0
                            //    : Int16.Parse(splitedValues[i]);
                            //reasonCodeOk = authRcode == 1;
                            break;
                        case 9:
                            //concilitation.AuthReversalRcode =
                            //    String.IsNullOrEmpty(splitedValues[i])
                            //        ? 0
                            //        : Int16.Parse(splitedValues[i]);
                            break;
                        case 12:
                            var billRcode = String.IsNullOrEmpty(splitedValues[i])
                                                ? 0
                                                : Int16.Parse(splitedValues[i]);
                            paymentRCode = billRcode == 1;
                            break;
                        case 15:
                            var rCode = String.IsNullOrEmpty(splitedValues[i])
                                            ? 0
                                            : Int16.Parse(splitedValues[i]);
                            refundRCode = rCode == 1;
                            if (refundRCode)
                            { concilitation.TransactionType = TransactionType.Refund; }

                            break;
                        case 94:
                            concilitation.Amount = String.IsNullOrEmpty(splitedValues[i])
                                            ? 0
                                            : Double.Parse(splitedValues[i],
                                                System.Globalization.CultureInfo.InvariantCulture);
                            break;
                        case 95:
                            concilitation.Currency = splitedValues[i];
                            break;
                        case 125:
                            concilitation.Source = splitedValues[i];
                            break;
                    }
                }
            }
            if (concilitation != null && (paymentRCode || refundRCode))
            {
                try
                {
                    _repositoryConciliationCybersource.Create(concilitation);
                    result = 1;
                    NLogLogger.LogEvent(NLogType.Info, string.Format("Cybersource - Metodo AnaliceRowData - Merchant_Id {0} , Persisto el request id {1}",
                        concilitation.MerchandId, concilitation.RequestId));
                }
                catch (Exception exception)
                {
                    NLogLogger.LogEvent(NLogType.Info, "Cybersource - Metodo AnaliceRowData - Error al intentar persistir. Request id " + concilitation.RequestId);
                    NLogLogger.LogEvent(exception);
                }
                result = 0;
            }

            if (intent > 0)
            {
                NLogLogger.LogEvent(NLogType.Info, "Cybersource - Metodo AnaliceRowData guarde en el intento " + intent + " el request id " + concilitation.RequestId);
            }
            return result;
        }

        private string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        private string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.UTF8.GetString(encodedDataAsBytes);
            return returnValue;
        }
        private void GetAll(object attr, string result)
        {
            Type t = attr.GetType();
            PropertyInfo[] props = t.GetProperties();
            foreach (PropertyInfo prp in props)
            {
                Type tt = prp.GetType();
                var pp = tt.GetProperties();
                //GetAll(pp, result);
                object value = prp.GetValue(attr, new object[] { });
                //dict.Add(prp.Name, value);
                result = result + " ; " + prp.Name + " VALOR : " + value;
            }
        }

        public CyberSourceMerchantDefinedDataDto LoadMerchantDefinedData(IAssociationInfoDto associatedDto, CyberSourceExtraDataDto cyberSourceExtraData, int paymentsCount)
        {
            //TODO: revisar si funciona
            var isRegisteredUser = true;
            ServiceAssociatedDto serviceAssociatedDto = null;
            VonDataAssociationDto vonAssociationDto = null;
            IUserDto user;
            if (associatedDto.GetType() == typeof(ServiceAssociatedDto))
            {
                serviceAssociatedDto = (ServiceAssociatedDto)associatedDto;
                user = serviceAssociatedDto.RegisteredUserDto;
            }
            else
            {
                vonAssociationDto = (VonDataAssociationDto)associatedDto;
                user = vonAssociationDto.AnonymousUserDto;
                isRegisteredUser = false;
            }

            var merchantDefinedData = new CyberSourceMerchantDefinedDataDto
            {
                ServiceType = isRegisteredUser ? serviceAssociatedDto.ServiceDto.ServiceCategory.Name : vonAssociationDto.ServiceDto.ServiceCategory.Name,
                OperationTypeDto = OperationTypeDto.UniquePayment.ToString(),
                UserRegistered = isRegisteredUser ? "Y" : "N",
                UserRegisteredDays = isRegisteredUser ? (DateTime.Now.Date - user.CreationDate.Date).Days.ToString() : "0",
                ReferenceNumber1 = isRegisteredUser ? serviceAssociatedDto.ReferenceNumber : vonAssociationDto.ReferenceNumber,
                ReferenceNumber2 = isRegisteredUser ? serviceAssociatedDto.ReferenceNumber2 : vonAssociationDto.ReferenceNumber,
                ReferenceNumber3 = isRegisteredUser ? serviceAssociatedDto.ReferenceNumber3 : vonAssociationDto.ReferenceNumber,
                ReferenceNumber4 = isRegisteredUser ? serviceAssociatedDto.ReferenceNumber4 : vonAssociationDto.ReferenceNumber,
                ReferenceNumber5 = isRegisteredUser ? serviceAssociatedDto.ReferenceNumber5 : vonAssociationDto.ReferenceNumber,
                ReferenceNumber6 = isRegisteredUser ? serviceAssociatedDto.ReferenceNumber6 : vonAssociationDto.ReferenceNumber,
                RedirctTo = 0.ToString(),
                DiscountApplyed = cyberSourceExtraData.BillDto.DiscountAmount > 0 ? ((int)cyberSourceExtraData.DiscountDto.DiscountLabel).ToString() : "0",
                TotalAmount = cyberSourceExtraData.BillDto.Amount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                TotalTaxedAmount = cyberSourceExtraData.BillDto.TaxedAmount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                Discount = cyberSourceExtraData.BillDto.DiscountAmount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                BillNumber = cyberSourceExtraData.BillDto.BillExternalId,
                AditionalNumberElectornicBill = "",
                NameTh = isRegisteredUser ? serviceAssociatedDto.DefaultCard.Name : vonAssociationDto.DefaultCard.Name,
                PaymentCount = paymentsCount,
                UserCi = user.IdentityNumber,
                UserMobile = user.MobileNumber,
                UserRegisteredAddress = user.Address,
                OperationId = cyberSourceExtraData.OperationId,
                MerchandId = isRegisteredUser ? serviceAssociatedDto.ServiceDto.MerchantId : vonAssociationDto.ServiceDto.MerchantId,
                CardBin = cyberSourceExtraData.BinNumber.ToString(),
                ServiceId = isRegisteredUser ? serviceAssociatedDto.ServiceId : vonAssociationDto.ServiceId,
                UserId = isRegisteredUser ? serviceAssociatedDto.UserId : vonAssociationDto.UserId,
                BillExpirationDate = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.ExpirationDate.ToString("dd-MM-yyyy") : string.Empty,
                BillDateInitTransaccion = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.DateInitTransaccion : string.Empty,
                BillDescription = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.Description : string.Empty,
                BillDiscount = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.Discount.ToString() : string.Empty,
                BillFinalConsumer = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.FinalConsumer.ToString() : string.Empty,
                BillGatewayTransactionBrouId = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.GatewayTransactionBrouId : string.Empty,
                BillGatewayTransactionId = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.GatewayTransactionId : string.Empty,
                BillSucivePreBillNumber = cyberSourceExtraData.BillDto != null ? cyberSourceExtraData.BillDto.SucivePreBillNumber : string.Empty,
                DiscountObjId = cyberSourceExtraData.DiscountDto != null ? cyberSourceExtraData.DiscountDto.Id : Guid.Empty,
            };
            return merchantDefinedData;
        }

        public CyberSourceOperationData TestPayment(Guid serviceId)
        {
            var service = _serviceService.GetById(serviceId, x => x.ServiceCategory);
            var userEmailTest = ConfigurationManager.AppSettings["AppUserForTest"];

            var user = _serviceApplicationUser.GetUserByUserName(userEmailTest);
            var card = user.CardDtos.FirstOrDefault(x => x.Active);

            var tryCounter = 5;
            var exit = false;
            CyberSourceDataDto data = null;
            CyberSourceOperationData cSOperationData = null;
            var transactionId = Guid.NewGuid();

            while (tryCounter > 0 && exit == false)
            {
                try
                {
                    var payment = new GeneratePayment
                    {
                        ApplicationUserId = user.Id,
                        Currency = "UYU",
                        MerchandId = service.MerchantId,
                        Token = card.PaymentToken,
                        TransaccionId = transactionId.ToString(),
                        GrandTotalAmount = ((double)1).SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                        Key = service.CybersourceTransactionKey,
                        UserType = LogUserType.Other,
                        PaymentPlatform = PaymentPlatformDto.VisaNet
                    };

                    var merchantDefinedData = new CyberSourceMerchantDefinedDataDto
                    {
                        ServiceType = service.ServiceCategory.Name,
                        OperationTypeDto = ((int)OperationTypeDto.UniquePayment).ToString(),
                        UserRegistered = "Y",
                        UserRegisteredDays = (DateTime.Now.Date - user.CreationDate.Date).Days.ToString(),
                        ReferenceNumber1 = "Test",
                        RedirctTo = 0.ToString(),
                        DiscountApplyed = "0",
                        TotalAmount = ((double)1).SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                        TotalTaxedAmount = ((double)0).SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                        Discount = ((double)0).SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                        BillNumber = "TestBillNumber",
                        NameTh = card.Name,

                        MerchandId = service.MerchantId,
                        CardBin = card.MaskedNumber.Substring(0, 6),
                        ServiceName = service.Name,
                    };

                    if (!String.IsNullOrEmpty(payment.Token))
                    {
                        data = GeneratePayment(payment, merchantDefinedData); //descomentar
                        exit = true;
                        cSOperationData = new CyberSourceOperationData
                        {
                            PaymentData = new CsResponseData()
                            {
                                PaymentResponseCode = int.Parse(data.ReasonCode),
                                PaymentRequestId = data.TransactionId
                            }
                        };
                    }
                    tryCounter--;
                }
                catch (CybersourceException e)
                {
                    tryCounter--;
                }
            }

            if (data != null && data.ReasonCode.Equals(CodePaymentSuccess))
            {
                //se realizo el pago en CyberSource correctamente
                cSOperationData = TestCancelPayment(serviceId, cSOperationData, user, card);
                return cSOperationData;
            }
            else
            {
                //no se pudo realizar el pago en CyberSource
                return cSOperationData;
            }
        }

        public CyberSourceOperationData TestCancelPayment(Guid serviceId, CyberSourceOperationData cSOperationData, ApplicationUserDto user = null, CardDto card = null)
        {
            var userEmailTest = ConfigurationManager.AppSettings["AppUserForTest"];
            var userDto = user ?? _serviceApplicationUser.GetUserByUserName(userEmailTest);
            var cardDto = card ?? userDto.CardDtos.FirstOrDefault(x => x.Active);
            var cancel = new CancelPayment
            {
                UserId = userDto.Id,
                ServiceId = serviceId,
                UserType = LogUserType.Registered,
                Token = cardDto.PaymentToken,
                RequestId = cSOperationData.PaymentData.PaymentRequestId,
                Amount = ((double)1).SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                IdTransaccion = cSOperationData.PaymentData.PaymentRequestId,
                Currency = "UYU",
                PaymentPlatform = PaymentPlatformDto.VisaNet
            };

            var csOperationDataVoid = VoidPayment(cancel);

            //Si el Void no es null, entonces lo agrego al response
            if (csOperationDataVoid.VoidData != null)
            {
                cSOperationData.VoidData = new CsResponseData
                {
                    PaymentRequestId = csOperationDataVoid.VoidData.PaymentRequestId,
                    PaymentResponseCode = csOperationDataVoid.VoidData.PaymentResponseCode
                };
            }

            //Si el Reverse no es null, entonces lo agrego al response
            if (csOperationDataVoid.ReversalData != null)
            {
                cSOperationData.ReversalData = new CsResponseData
                {
                    PaymentRequestId = csOperationDataVoid.ReversalData.PaymentRequestId,
                    PaymentResponseCode = csOperationDataVoid.ReversalData.PaymentResponseCode
                };
            }

            return cSOperationData;
        }

        public CyberSourceOperationData TestReversePayment(Guid serviceId)
        {
            try
            {
                var userEmailTest = ConfigurationManager.AppSettings["AppUserForTest"];
                var user = _serviceApplicationUser.GetUserByUserName(userEmailTest);
                var card = user.CardDtos.FirstOrDefault(x => x.Active);

                return ReversePayment(new RefundPayment()
                {
                    Amount = ((double)1).SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                    Currency = "UYU",
                    UserId = user.Id,
                    ServiceId = serviceId,
                    Token = card.PaymentToken,
                    PaymentPlatform = PaymentPlatformDto.VisaNet,
                    UserType = LogUserType.Registered
                });
            }
            catch (Exception exception)
            {

                throw;
            }

        }

        public bool TestReports(Guid serviceId)
        {
            try
            {
                var list = ObtainData(DateTime.Now.AddDays(-2), null, serviceId);
                if (list != null)
                {
                    return true;
                }
                return false;

            }
            catch (Exception)
            {
                return false;
            }
        }

        private void NotificationFix(ReplyMessage replay, CancelPayment cancel, RefundPayment refund, string action, Exception exception)
        {
            if (replay != null && replay.reasonCode.Equals(CodePaymentSuccess)) return;

            try
            {
                var notiService = string.Format("Cybersource error.");
                var notiDetail = string.Format("Ha ocurrido un error al intentar realizar un {0} en Cybersource.<br /><br />", action);
                notiDetail += string.Format("El codigo de error es el {0}, motivo: {1}.<br />", replay == null ? "" : replay.reasonCode, replay == null ? "" : replay.decision);

                if (action.Equals("VOID"))
                {
                    notiDetail += string.Format("Los datos de la transacción son los siguientes: <br /> " +
                                                "Nro de Transaccion de pago: {0}<br />" +
                                                "Nro de Transaccion del void: {1}<br />" +
                                                "MerchandId: {2}<br /><br />", cancel.RequestId, replay == null ? "" : replay.requestID, cancel.ServiceDto.MerchantId);
                }
                if (action.Equals("REVERSE") || action.Equals("REFUND"))
                {
                    notiDetail += string.Format("Los datos de la transacción son los siguientes: <br /> " +
                                                "Nro de Transaccion de pago: {0}<br />" +
                                                "Nro de Transaccion del void: {1}<br />" +
                                                "MerchandId: {2}<br />", refund.RequestId, replay == null ? "" : replay.requestID, refund.ServiceDto.MerchantId);
                    notiDetail += string.Format("Monto: {0}<br />Moneda: {1}<br /><br />", refund.Amount, refund.Currency);
                }
                if (exception != null)
                {
                    notiDetail += _serviceFixedNotification.ExceptionMsg(exception);
                }

                _serviceFixedNotification.Create(new FixedNotificationDto()
                {
                    Category = FixedNotificationCategoryDto.CybersourceError,
                    DateTime = DateTime.Now,
                    Level = FixedNotificationLevelDto.Error,
                    Description = notiService,
                    Detail = notiDetail
                });
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "NotificationFix - Excepcion");
                NLogLogger.LogEvent(e);
            }

        }

        private void LoadBillData(ref IDictionary<string, string> data, IUserDto user)
        {
            var strSignedFields = data["signed_field_names"];
            if (string.IsNullOrEmpty(strSignedFields) == false) { strSignedFields = string.Concat(strSignedFields, ","); }
            data["signed_field_names"] = string.Concat(strSignedFields, "bill_to_forename,bill_to_surname,bill_to_email,bill_to_phone,bill_to_address_line1,bill_city,bill_to_address_city,bill_to_address_state,bill_to_address_country,bill_to_address_postal_code");

            var userPhoneTrimed = !string.IsNullOrEmpty(user.PhoneNumber) ? user.PhoneNumber.Trim() :
                !string.IsNullOrEmpty(user.MobileNumber) ? user.MobileNumber.Trim() :
                string.Empty;

            data.Add("bill_to_forename", !string.IsNullOrEmpty(user.Name) && user.Name.Length > 60 ? user.Name.Substring(0, 60) : user.Name);
            data.Add("bill_to_surname", !string.IsNullOrEmpty(user.Surname) && user.Surname.Length > 60 ? user.Surname.Substring(0, 60) : user.Surname);
            data.Add("bill_to_email", user.Email);
            data.Add("bill_to_phone", !string.IsNullOrEmpty(userPhoneTrimed) && userPhoneTrimed.Length > 15 ? userPhoneTrimed.Substring(0, 15) : userPhoneTrimed);

            //datos indicados por cybersource
            data.Add("bill_to_address_line1", "1295 Charleston Road");
            data.Add("bill_city", "Mountain View");
            data.Add("bill_to_address_city", "Mountain View");
            data.Add("bill_to_address_state", "CA");
            data.Add("bill_to_address_country", "US");
            data.Add("bill_to_address_postal_code", "94043");
        }

        private void LoadMerchantDefinedDataToDictionary(IGenerateToken generateToken, ref IDictionary<string, string> data, IUserDto user)
        {
            var item = (KeysInfoForPayment)generateToken;

            var service = _serviceService.GetById(item.ServiceId, x => x.ServiceCategory);

            var userRegisteredDays = user.IsRegisteredUser ? (DateTime.Now.Date - user.CreationDate.Date).Days : 0;

            var paymentCount = user.IsRegisteredUser ? _repositoryPayment.All(x => x.RegisteredUserId == user.Id).Count() : 0;

            var strSignedFields = data["signed_field_names"];
            if (string.IsNullOrEmpty(strSignedFields) == false) { strSignedFields = string.Concat(strSignedFields, ","); }
            data["signed_field_names"] = string.Concat(strSignedFields, "merchant_defined_data1,merchant_defined_data2,merchant_defined_data3,merchant_defined_data4,merchant_defined_data5,merchant_defined_data6,merchant_defined_data7,merchant_defined_data8,merchant_defined_data9,merchant_defined_data10,merchant_defined_data11,merchant_defined_data12,merchant_defined_data13,merchant_defined_data14,merchant_defined_data15,merchant_defined_data16,merchant_defined_data17,merchant_defined_data18,merchant_defined_data19,merchant_defined_data20,merchant_defined_data21,merchant_defined_data22,merchant_defined_data23,merchant_defined_data24,merchant_defined_data25,merchant_defined_data26,merchant_defined_data27,merchant_defined_data28,merchant_defined_data29,merchant_defined_data30,merchant_defined_data31,merchant_defined_data32,merchant_defined_data33,merchant_defined_data34,merchant_defined_data35,merchant_defined_data36,merchant_defined_data37,merchant_defined_data38,merchant_defined_data39,merchant_defined_data40,merchant_defined_data41,merchant_defined_data42,merchant_defined_data43,merchant_defined_data44,merchant_defined_data45,merchant_defined_data46,merchant_defined_data47,merchant_defined_data48,merchant_defined_data49,merchant_defined_data50,merchant_defined_data51,merchant_defined_data52,merchant_defined_data53");

            data.Add("merchant_defined_data1", service.ServiceCategory != null ? service.ServiceCategory.Name : string.Empty);
            data.Add("merchant_defined_data2", item.OperationTypeDto.ToString());
            data.Add("merchant_defined_data3", user.IsRegisteredUser ? "Y" : "N");
            data.Add("merchant_defined_data4", userRegisteredDays.ToString());
            data.Add("merchant_defined_data5", LimitStringLength(item.ReferenceNumber1, 99));
            data.Add("merchant_defined_data6", LimitStringLength(item.ReferenceNumber2, 99));
            data.Add("merchant_defined_data7", LimitStringLength(item.ReferenceNumber3, 99));
            data.Add("merchant_defined_data8", LimitStringLength(item.ReferenceNumber4, 99));
            data.Add("merchant_defined_data9", LimitStringLength(item.ReferenceNumber5, 99));
            data.Add("merchant_defined_data10", LimitStringLength(item.ReferenceNumber6, 99));
            data.Add("merchant_defined_data11", item.RedirectTo);
            data.Add("merchant_defined_data12", item.Bill.DiscountType.ToString());
            data.Add("merchant_defined_data13", item.Bill.Amount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")));
            data.Add("merchant_defined_data14", item.Bill.TaxedAmount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")));
            data.Add("merchant_defined_data15", item.Bill.DiscountAmount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")));
            data.Add("merchant_defined_data16", LimitStringLength(item.Bill.BillNumber, 99));
            data.Add("merchant_defined_data17", item.Bill.AditionalNumberElectronicBill);
            data.Add("merchant_defined_data18", LimitStringLength(item.NameTh, 99));
            data.Add("merchant_defined_data19", paymentCount.ToString());
            data.Add("merchant_defined_data20", LimitStringLength(user.MobileNumber, 99));
            data.Add("merchant_defined_data21", LimitStringLength(user.Address, 99));
            data.Add("merchant_defined_data22", LimitStringLength(user.IdentityNumber, 99));
            data.Add("merchant_defined_data23", service.MerchantId);
            data.Add("merchant_defined_data24", item.CardBin);
            data.Add("merchant_defined_data25", LimitStringLength(service.Name, 99));
            data.Add("merchant_defined_data26", LimitStringLength(item.CallcenterUser, 99));
            data.Add("merchant_defined_data27", LimitStringLength(item.OperationId, 99));
            data.Add("merchant_defined_data28", LimitStringLength(item.Platform, 99));
            data.Add("merchant_defined_data29", item.TemporaryTransactionIdentifier);
            data.Add("merchant_defined_data30", LimitStringLength(item.ServiceId.ToString(), 99));
            data.Add("merchant_defined_data31", LimitStringLength(item.UserId.ToString(), 99));
            data.Add("merchant_defined_data32", item.CardId.ToString());
            data.Add("merchant_defined_data33", item.GatewayId.ToString());
            data.Add("merchant_defined_data34", LimitStringLength(item.DiscountObjId.ToString(), 99));
            data.Add("merchant_defined_data35", item.PaymentTypeDto.ToString());
            data.Add("merchant_defined_data36", LimitStringLength(item.Bill.BillExpirationDate, 99));
            data.Add("merchant_defined_data37", LimitStringLength(item.Bill.BillDescription, 99));
            data.Add("merchant_defined_data38", LimitStringLength(item.Bill.BillGatewayTransactionId, 99));
            data.Add("merchant_defined_data39", LimitStringLength(item.Bill.BillSucivePreBillNumber, 99));
            data.Add("merchant_defined_data40", LimitStringLength(item.Bill.BillFinalConsumer, 99));
            data.Add("merchant_defined_data41", LimitStringLength(item.Bill.BillDiscount, 99));
            data.Add("merchant_defined_data42", LimitStringLength(item.Bill.BillDateInitTransaccion, 99));
            data.Add("merchant_defined_data43", LimitStringLength(item.Bill.BillGatewayTransactionBrouId, 99));
            data.Add("merchant_defined_data44", string.Empty);//reservado para generacion servicios. Notificaciones
            data.Add("merchant_defined_data45", string.Empty);//reservado para generacion servicios. Descripcion del servicio asociado
            data.Add("merchant_defined_data46", string.Empty);//reservado para generacion servicios y tarjeta en apps. clave usuario hasheada
            data.Add("merchant_defined_data47", item.Bill.Quota.ToString());
            data.Add("merchant_defined_data48", item.CardTypeDto.ToString());
            data.Add("merchant_defined_data49", string.Empty);//reservado para el tipo de emisor de la tarjeta
            data.Add("merchant_defined_data50", string.Empty);//reservado para debitos con nombre del comercio y producto 
            data.Add("merchant_defined_data51", string.Empty);//reservado para debitos con id del comercio
            data.Add("merchant_defined_data52", string.Empty);//reservado para debitos con id del producto
            data.Add("merchant_defined_data53", item.Field48);//reservado para campo 48
        }

        //Keys for Payment
        public IDictionary<string, string> LoadKeysForNewUserPayment(IGenerateToken generateToken)
        {
            var item = (KeysInfoForPaymentNewUser)generateToken;

            var user = _serviceAnonymousUser.GetById(item.UserId);
            item.CyberSourceIdentifier = user.CyberSourceIdentifier.ToString();
            UseToken = false;
            var transactionType = VisaNet.Utilities.Cybersource.Enums.TransactionType.SaleAndCreatePaymentToken;

            var keys = LoadKeysForPayment(item, transactionType, user);
            return keys;
        }

        public IDictionary<string, string> LoadKeysForRegisteredUserPayment(IGenerateToken generateToken)
        {
            var item = (KeysInfoForPaymentRegisteredUser)generateToken;

            var user = _serviceApplicationUser.GetById(item.UserId);
            if (user == null)
            {
                throw new FatalException(ExceptionMessages.USER_NOT_EXIST);
            }
            item.CyberSourceIdentifier = user.CyberSourceIdentifier.ToString();
            if (item.CardId != Guid.Empty)
            {
                UseToken = true;
            }
            var transactionType = UseToken ? VisaNet.Utilities.Cybersource.Enums.TransactionType.Sale : VisaNet.Utilities.Cybersource.Enums.TransactionType.SaleAndCreatePaymentToken;
            var keys = LoadKeysForPayment(item, transactionType, user);
            return keys;
        }

        public IDictionary<string, string> LoadKeysForRecurrentUserPayment(IGenerateToken generateToken)
        {
            var item = (KeysInfoForPaymentRecurrentUser)generateToken;

            var user = _serviceAnonymousUser.GetById(item.UserId);
            if (user == null)
            {
                throw new FatalException(ExceptionMessages.USER_NOT_EXIST);
            }
            item.CyberSourceIdentifier = user.CyberSourceIdentifier.ToString();
            if (item.CardId != Guid.Empty)
            {
                UseToken = true;
            }
            var transactionType = UseToken ? VisaNet.Utilities.Cybersource.Enums.TransactionType.Sale : VisaNet.Utilities.Cybersource.Enums.TransactionType.SaleAndCreatePaymentToken;
            var keys = LoadKeysForPayment(item, transactionType, user);
            return keys;
        }

        public IDictionary<string, string> LoadKeysForAnonymousUserPayment(IGenerateToken generateToken)
        {
            var item = (KeysInfoForPaymentAnonymousUser)generateToken;

            var anonymousUser = _serviceAnonymousUser.GetById(item.UserId);
            if (anonymousUser == null)
            {
                throw new FatalException(ExceptionMessages.USER_NOT_EXIST);
            }
            item.CyberSourceIdentifier = anonymousUser.CyberSourceIdentifier.ToString();
            UseToken = false;
            var transactionType = VisaNet.Utilities.Cybersource.Enums.TransactionType.Sale;
            var keys = LoadKeysForPayment(item, transactionType, anonymousUser);
            return keys;
        }

        private IDictionary<string, string> LoadKeysForPayment(IGenerateToken generateToken, string transactionType, IUserDto user = null)
        {
            var item = (KeysInfoForPayment)generateToken;

            var strDateTime = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");

            var signedFields = "reference_number,transaction_type,currency,locale,access_key,profile_id,transaction_uuid,signed_date_time,signed_field_names,unsigned_field_names,amount,override_custom_receipt_page,consumer_id,device_fingerprint_id,issuer_additional_data";
            var unsignedFields = string.Empty;

            var service = _serviceService.GetById(item.ServiceId);
            if (service == null)
            {
                throw new FatalException(ExceptionMessages.SERVICE_NOT_EXIST);
            }

            //FIELD 48 TIENE QUE TENER EL CARD TYPE SIEMPRE
            if (generateToken.CardTypeDto == 0)
            {
                if (!string.IsNullOrEmpty(generateToken.CardBin))
                {
                    var binVal = int.Parse(generateToken.CardBin);
                    var bin = _serviceBin.Find(binVal);
                    generateToken.CardTypeDto = bin.CardType;
                }
                else if (item.CardId != Guid.Empty)
                {
                    var card = _repositoryCard.GetById(item.CardId);
                    var bin = _serviceBin.Find(card.BIN);
                    generateToken.CardTypeDto = bin.CardType;
                }
            }

            var field48 = GenerateField48(generateToken);
            item.Field48 = field48;

            IDictionary<string, string> keys = new Dictionary<string, string>
            {
                    {"reference_number",item.TransactionReferenceNumber},
                    {"transaction_type",transactionType},
                    {"currency",item.Bill.Currency},
                    {"locale","en"},
                    {"access_key",service.CybersourceAccessKey},
                    {"profile_id",service.CybersourceProfileId},
                    {"transaction_uuid",item.TransactionReferenceNumber},
                    {"signed_date_time",strDateTime},
                    {"amount",item.CybersourceAmount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US"))},
                    {"override_custom_receipt_page", item.UrlReturn},
                    {"device_fingerprint_id", item.FingerPrint},
                    {"consumer_id", item.CyberSourceIdentifier},
                    {"issuer_additional_data", field48},
            };

            if (UseToken && user != null && user.GetType() == typeof(AnonymousUserDto))
            {
                //Usuario recurrente con tarjeta existente
                var cardToken = _serviceVonData.GetCardPaymentToken(user.Id, item.CardId.ToString());
                if (!string.IsNullOrEmpty(signedFields))
                {
                    signedFields = string.Concat(signedFields, ",");
                }
                signedFields = string.Concat(signedFields, "payment_token");
                keys.Add("payment_token", cardToken);
            }
            else if (UseToken)
            {
                //Usuario registrado con tarjeta existente
                var card = _repositoryCard.GetById(item.CardId);
                if (!string.IsNullOrEmpty(signedFields))
                {
                    signedFields = string.Concat(signedFields, ",");
                }
                signedFields = string.Concat(signedFields, "payment_token");
                keys.Add("payment_token", card.PaymentToken);
            }
            else
            {
                //Tarjeta nueva
                if (!string.IsNullOrEmpty(signedFields))
                {
                    signedFields = string.Concat(signedFields, ",");
                }
                signedFields = string.Concat(signedFields, "payment_method");
                keys.Add("payment_method", "card");

                if (!string.IsNullOrEmpty(unsignedFields))
                {
                    unsignedFields = string.Concat(unsignedFields, ",");
                }
                unsignedFields = string.Concat(unsignedFields, "card_type,card_cvn,card_number,card_expiry_date");
            }

            keys.Add("signed_field_names", signedFields);
            keys.Add("unsigned_field_names", unsignedFields);

            if (!UseToken)
                LoadBillData(ref keys, user);

            LoadMerchantDefinedDataToDictionary(item, ref keys, user);

            keys.Add("signature", Security.Sign(keys, service.CybersourceSecretKey));

            return keys;
        }

        //Keys for Token
        public IDictionary<string, string> LoadKeysForToken(IGenerateToken generateToken)
        {
            var item = (KeysInfoForTokenRegisteredUser)generateToken;

            UseToken = false;
            var param = _serviceParameters.GetParametersForCard();

            var cybersourceAccessKey = param.CybersourceAccessKey;
            var cybersourceProfileId = param.CybersourceProfileId;
            var cybersourceCybersourceSecretKey = param.CybersourceSecretKey;

            var notifications = item.NotificationsConfig != null
                ? SerializeNotificationconfig(item.NotificationsConfig)
                : string.Empty;

            var user = _serviceApplicationUser.GetById(item.UserId);
            if (user == null)
            {
                throw new FatalException(ExceptionMessages.USER_NOT_EXIST);
            }

            IDictionary<string, string> keys = new Dictionary<string, string>
            {
                {"signed_field_names",@"access_key,profile_id,transaction_uuid,signed_field_names,unsigned_field_names,signed_date_time,locale,transaction_type,reference_number,merchant_defined_data11,override_custom_receipt_page,merchant_defined_data18,consumer_id,merchant_defined_data24,merchant_defined_data26,merchant_defined_data27,merchant_defined_data28,merchant_defined_data29,payment_method,currency,merchant_defined_data3,merchant_defined_data20,merchant_defined_data21,merchant_defined_data22,merchant_defined_data30,merchant_defined_data31,merchant_defined_data35,merchant_defined_data44,merchant_defined_data5,merchant_defined_data6,merchant_defined_data7,merchant_defined_data8,merchant_defined_data9,merchant_defined_data10,device_fingerprint_id"},
                {"unsigned_field_names", "card_type,card_number,card_expiry_date,card_cvn"},

                {"access_key", cybersourceAccessKey},
                {"profile_id", cybersourceProfileId},
                {"transaction_uuid", item.TransactionReferenceNumber},
                {"signed_date_time", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")},
                {"locale", "en"},
                {"transaction_type", "create_payment_token"},
                {"reference_number", item.TransactionReferenceNumber},
                {"merchant_defined_data11", item.RedirectTo},
                {"override_custom_receipt_page", item.UrlReturn },
                {"merchant_defined_data3", "Y"},
                {"merchant_defined_data18", !string.IsNullOrEmpty(item.NameTh) && item.NameTh.Length > 99 ? item.NameTh.Substring(0,99) : item.NameTh},
                {"consumer_id", item.CyberSourceIdentifier},
                {"merchant_defined_data24", item.CardBin},
                {"merchant_defined_data26", string.IsNullOrEmpty(item.CallcenterUser) ? string.Empty : item.CallcenterUser},
                {"merchant_defined_data27", string.IsNullOrEmpty(item.OperationId) ? string.Empty : item.OperationId},
                {"merchant_defined_data28", string.IsNullOrEmpty(item.Platform) ? string.Empty : item.Platform},
                {"merchant_defined_data29", string.IsNullOrEmpty(item.TemporaryTransactionIdentifier) ? string.Empty : item.TemporaryTransactionIdentifier},
                {"payment_method", "card"},
                {"currency", "UYU"},
                {"merchant_defined_data20", !string.IsNullOrEmpty(user.MobileNumber) && user.MobileNumber.Length > 99 ? user.MobileNumber.Substring(0,99) : user.MobileNumber},
                {"merchant_defined_data21", !string.IsNullOrEmpty(user.Address) && user.Address.Length > 99 ? user.Address.Substring(0,99) : user.Address},
                {"merchant_defined_data22", !string.IsNullOrEmpty(user.IdentityNumber) && user.IdentityNumber.Length > 99 ? user.IdentityNumber.Substring(0,99) : user.IdentityNumber},
                {"merchant_defined_data30", item.ServiceId.ToString()},
                {"merchant_defined_data31", user.Id.ToString()},
                {"merchant_defined_data35", item.PaymentTypeDto.ToString()},
                {"merchant_defined_data44", notifications},
                {"merchant_defined_data5", !string.IsNullOrEmpty(item.ReferenceNumber1) && item.ReferenceNumber1.Length > 99 ? item.ReferenceNumber1.Substring(0,99) : item.ReferenceNumber1},
                {"merchant_defined_data6", !string.IsNullOrEmpty(item.ReferenceNumber2) && item.ReferenceNumber2.Length > 99 ? item.ReferenceNumber2.Substring(0,99) : item.ReferenceNumber2},
                {"merchant_defined_data7", !string.IsNullOrEmpty(item.ReferenceNumber3) && item.ReferenceNumber3.Length > 99 ? item.ReferenceNumber3.Substring(0,99) : item.ReferenceNumber3},
                {"merchant_defined_data8", !string.IsNullOrEmpty(item.ReferenceNumber4) && item.ReferenceNumber4.Length > 99 ? item.ReferenceNumber4.Substring(0,99) : item.ReferenceNumber4},
                {"merchant_defined_data9", !string.IsNullOrEmpty(item.ReferenceNumber5) && item.ReferenceNumber5.Length > 99 ? item.ReferenceNumber5.Substring(0,99) : item.ReferenceNumber5},
                {"merchant_defined_data10", !string.IsNullOrEmpty(item.ReferenceNumber6) && item.ReferenceNumber6.Length > 99 ? item.ReferenceNumber6.Substring(0,99) : item.ReferenceNumber6},
                {"device_fingerprint_id", item.FingerPrint},
            };
            LoadBillData(ref keys, user);
            keys.Add("signature", Security.Sign(keys, cybersourceCybersourceSecretKey));

            return keys;
        }

        public IDictionary<string, string> LoadKeysForRegisteredUserTokenApps(IGenerateToken generateToken)
        {
            var item = (KeysInfoForTokenRegisteredUser)generateToken;
            UseToken = false;

            var user = _serviceApplicationUser.GetById(item.UserId);
            if (user == null)
            {
                throw new FatalException(ExceptionMessages.USER_NOT_EXIST);
            }
            item.CyberSourceIdentifier = user.CyberSourceIdentifier.ToString();

            return LoadKeysForTokenApps(item, user);
        }

        public IDictionary<string, string> LoadKeysForRecurrentUserToken(IGenerateToken generateToken)
        {
            var item = (KeysInfoForTokenRecurrentUser)generateToken;
            UseToken = false;

            var user = _serviceAnonymousUser.GetById(item.UserId);
            if (user == null)
            {
                throw new FatalException(ExceptionMessages.USER_NOT_EXIST);
            }
            item.CyberSourceIdentifier = user.CyberSourceIdentifier.ToString();

            return LoadKeysForTokenApps(item, user);
        }

        public IDictionary<string, string> LoadKeysForNewUserTokenApps(IGenerateToken generateToken)
        {
            var item = (KeysInfoForTokenNewUser)generateToken;
            UseToken = false;

            var user = new ApplicationUserDto
            {
                Name = item.Name,
                Surname = item.Surname,
                Email = item.Email,
                Address = item.Address,
                MobileNumber = item.MobileNumber,
                PhoneNumber = item.PhoneNumber,
                IdentityNumber = item.IdentityNumber,
                Password = item.Password
            };
            item.CyberSourceIdentifier = _serviceApplicationUser.GetNextCyberSourceIdentifier().ToString();

            return LoadKeysForTokenApps(item, user);
        }

        public IDictionary<string, string> LoadKeysForNewRecurrentUser(IGenerateToken generateToken)
        {
            var item = (KeysInfoForTokenNewUser)generateToken;
            UseToken = false;

            var user = _serviceAnonymousUser.GetById(item.UserId);
            item.CyberSourceIdentifier = user.CyberSourceIdentifier.ToString();

            return LoadKeysForTokenApps(item, user);
        }

        private IDictionary<string, string> LoadKeysForTokenApps(IGenerateToken generateToken, IUserDto user)
        {
            var item = (KeysInfoForToken)generateToken;

            var param = _serviceParameters.GetParametersForCard();

            var cybersourceAccessKey = param.CybersourceAccessKey;
            var cybersourceProfileId = param.CybersourceProfileId;
            var cybersourceCybersourceSecretKey = param.CybersourceSecretKey;

            var isUserRegistered = user != null && user.GetType() == typeof(ApplicationUserDto);

            IDictionary<string, string> keys = new Dictionary<string, string>
            {
                {"signed_field_names",@"access_key,profile_id,transaction_uuid,signed_field_names,unsigned_field_names,signed_date_time,locale,transaction_type,reference_number,merchant_defined_data11,override_custom_receipt_page,merchant_defined_data18,consumer_id,merchant_defined_data24,merchant_defined_data26,merchant_defined_data27,merchant_defined_data28,merchant_defined_data29,payment_method,currency,merchant_defined_data3,merchant_defined_data20,merchant_defined_data21,merchant_defined_data22,merchant_defined_data30,merchant_defined_data31,merchant_defined_data35,merchant_defined_data5,merchant_defined_data6,merchant_defined_data7,merchant_defined_data8,merchant_defined_data9,merchant_defined_data10,merchant_defined_data46,device_fingerprint_id"},
                {"unsigned_field_names", "card_type,card_number,card_expiry_date,card_cvn"},

                {"access_key", cybersourceAccessKey},
                {"profile_id", cybersourceProfileId},
                {"transaction_uuid", item.TransactionReferenceNumber},
                {"signed_date_time", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")},
                {"locale", "en"},
                {"transaction_type", "create_payment_token"},
                {"reference_number", item.TransactionReferenceNumber},
                {"merchant_defined_data11", item.RedirectTo},
                {"override_custom_receipt_page", item.UrlReturn },
                {"merchant_defined_data3", isUserRegistered ? "Y" : "N"},
                {"device_fingerprint_id", item.FingerPrint},
                {"merchant_defined_data18", !string.IsNullOrEmpty(item.NameTh) && item.NameTh.Length > 99 ? item.NameTh.Substring(0,99) : item.NameTh},
                {"consumer_id", item.CyberSourceIdentifier},
                {"merchant_defined_data24", item.CardBin},
                {"merchant_defined_data26", string.IsNullOrEmpty(item.CallcenterUser) ? string.Empty : item.CallcenterUser},
                {"merchant_defined_data27", string.IsNullOrEmpty(item.OperationId) ? string.Empty : item.OperationId},
                {"merchant_defined_data28", string.IsNullOrEmpty(item.Platform) ? string.Empty : item.Platform},
                {"merchant_defined_data29", string.IsNullOrEmpty(item.TemporaryTransactionIdentifier) ? string.Empty : item.TemporaryTransactionIdentifier},
                {"payment_method", "card"},
                {"currency", "UYU"},
                {"merchant_defined_data20", LimitStringLength(user.MobileNumber, 99)},
                {"merchant_defined_data21", LimitStringLength(user.Address, 99)},
                {"merchant_defined_data22", LimitStringLength(user.IdentityNumber, 99)},
                {"merchant_defined_data30", item.ServiceId.ToString()},
                {"merchant_defined_data31", item.UserId.ToString()},
                {"merchant_defined_data35", item.PaymentTypeDto.ToString()},
                {"merchant_defined_data5", LimitStringLength(item.ReferenceNumber1, 99)},
                {"merchant_defined_data6", LimitStringLength(item.ReferenceNumber2, 99)},
                {"merchant_defined_data7", LimitStringLength(item.ReferenceNumber3, 99)},
                {"merchant_defined_data8", LimitStringLength(item.ReferenceNumber4, 99)},
                {"merchant_defined_data9", LimitStringLength(item.ReferenceNumber5, 99)},
                {"merchant_defined_data10", LimitStringLength(item.ReferenceNumber6, 99)},
                {"merchant_defined_data46", user.Password},
            };
            LoadBillData(ref keys, user);
            keys.Add("signature", Security.Sign(keys, cybersourceCybersourceSecretKey));

            return keys;
        }

        //Keys for Debit
        public IDictionary<string, string> LoadKeysForTokenDebitNewUser(IGenerateToken generateToken)
        {
            var item = (KeysInfoForTokenDebitNewUser)generateToken;

            UseToken = false;
            var param = _serviceParameters.GetParametersForCard();

            var cybersourceAccessKey = param.CybersourceAccessKey;
            var cybersourceProfileId = param.CybersourceProfileId;
            var cybersourceCybersourceSecretKey = param.CybersourceSecretKey;

            IDictionary<string, string> keys = new Dictionary<string, string>
            {
                {"signed_field_names",@"access_key,profile_id,transaction_uuid,signed_field_names,unsigned_field_names,signed_date_time,locale,transaction_type,reference_number,merchant_defined_data11,override_custom_receipt_page,merchant_defined_data18,consumer_id,merchant_defined_data24,merchant_defined_data26,merchant_defined_data27,merchant_defined_data28,merchant_defined_data29,payment_method,currency,merchant_defined_data3,merchant_defined_data20,merchant_defined_data22,merchant_defined_data30,merchant_defined_data31,merchant_defined_data35,merchant_defined_data5,merchant_defined_data6,merchant_defined_data7,merchant_defined_data8,merchant_defined_data9,merchant_defined_data10,device_fingerprint_id,merchant_defined_data46,merchant_defined_data50,merchant_defined_data51,merchant_defined_data52"},
                {"unsigned_field_names", "card_type,card_number,card_expiry_date,card_cvn"},

                {"access_key", cybersourceAccessKey},
                {"profile_id", cybersourceProfileId},
                {"transaction_uuid", item.TransactionReferenceNumber},
                {"signed_date_time", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")},
                {"locale", "en"},
                {"transaction_type", "create_payment_token"},
                {"reference_number", item.TransactionReferenceNumber},
                {"override_custom_receipt_page", item.UrlReturn },
                {"consumer_id", item.CyberSourceIdentifier},
                {"payment_method", "card"},
                {"currency", "UYU"},


                {"merchant_defined_data2", item.OperationTypeDto.ToString()},
                {"merchant_defined_data3", "Y"},

                {"merchant_defined_data5", !string.IsNullOrEmpty(item.ReferenceNumber1) && item.ReferenceNumber1.Length > 99 ? item.ReferenceNumber1.Substring(0,99) : item.ReferenceNumber1},
                {"merchant_defined_data6", !string.IsNullOrEmpty(item.ReferenceNumber2) && item.ReferenceNumber2.Length > 99 ? item.ReferenceNumber2.Substring(0,99) : item.ReferenceNumber2},
                {"merchant_defined_data7", !string.IsNullOrEmpty(item.ReferenceNumber3) && item.ReferenceNumber3.Length > 99 ? item.ReferenceNumber3.Substring(0,99) : item.ReferenceNumber3},
                {"merchant_defined_data8", !string.IsNullOrEmpty(item.ReferenceNumber4) && item.ReferenceNumber4.Length > 99 ? item.ReferenceNumber4.Substring(0,99) : item.ReferenceNumber4},
                {"merchant_defined_data9", !string.IsNullOrEmpty(item.ReferenceNumber5) && item.ReferenceNumber5.Length > 99 ? item.ReferenceNumber5.Substring(0,99) : item.ReferenceNumber5},
                {"merchant_defined_data10", !string.IsNullOrEmpty(item.ReferenceNumber6) && item.ReferenceNumber6.Length > 99 ? item.ReferenceNumber6.Substring(0,99) : item.ReferenceNumber6},

                {"merchant_defined_data11", item.RedirectTo},
                {"merchant_defined_data18", !string.IsNullOrEmpty(item.NameTh) && item.NameTh.Length > 99 ? item.NameTh.Substring(0,99) : item.NameTh},

                {"merchant_defined_data20", !string.IsNullOrEmpty(item.MobileNumber) && item.MobileNumber.Length > 99 ? item.MobileNumber.Substring(0,99) : item.MobileNumber},
                {"merchant_defined_data22", !string.IsNullOrEmpty(item.IdentityNumber) && item.IdentityNumber.Length > 99 ? item.IdentityNumber.Substring(0,99) : item.IdentityNumber},

                {"merchant_defined_data24", item.CardBin},
                {"merchant_defined_data26", string.IsNullOrEmpty(item.CallcenterUser) ? string.Empty : item.CallcenterUser},
                {"merchant_defined_data27", string.IsNullOrEmpty(item.OperationId) ? string.Empty : item.OperationId},
                {"merchant_defined_data28", string.IsNullOrEmpty(item.Platform) ? string.Empty : item.Platform},
                {"merchant_defined_data29", string.IsNullOrEmpty(item.TemporaryTransactionIdentifier) ? string.Empty : item.TemporaryTransactionIdentifier},
                {"merchant_defined_data30", item.ServiceId.ToString()},
                {"merchant_defined_data31", Guid.Empty.ToString()},
                {"merchant_defined_data35", item.PaymentTypeDto.ToString()},

                {"device_fingerprint_id", item.FingerPrint},

                {"merchant_defined_data46", item.Password},

                {"merchant_defined_data50", item.CommerceAndProductName},
                {"merchant_defined_data51", item.CommerceId.ToString()},
                {"merchant_defined_data52", item.ProductId.ToString()},

            };
            LoadBillData(ref keys, new ApplicationUserDto()
            {
                Name = item.Name,
                Surname = item.Surname,
                MobileNumber = item.MobileNumber,
                Email = item.Email,
                IdentityNumber = item.IdentityNumber,
            });
            keys.Add("signature", Security.Sign(keys, cybersourceCybersourceSecretKey));

            return keys;
        }

        public IDictionary<string, string> LoadKeysForTokenDebitRegisteredUser(IGenerateToken generateToken)
        {
            var item = (KeysInfoForTokenDebitRegisteredUser)generateToken;

            UseToken = false;
            var param = _serviceParameters.GetParametersForCard();

            var cybersourceAccessKey = param.CybersourceAccessKey;
            var cybersourceProfileId = param.CybersourceProfileId;
            var cybersourceCybersourceSecretKey = param.CybersourceSecretKey;

            var user = _serviceApplicationUser.GetById(item.UserId);
            if (user == null)
            {
                throw new FatalException(ExceptionMessages.USER_NOT_EXIST);
            }

            IDictionary<string, string> keys = new Dictionary<string, string>
            {
                {"signed_field_names",@"access_key,profile_id,transaction_uuid,signed_field_names,unsigned_field_names,signed_date_time,locale,transaction_type,reference_number,merchant_defined_data11,override_custom_receipt_page,merchant_defined_data18,consumer_id,merchant_defined_data24,merchant_defined_data26,merchant_defined_data27,merchant_defined_data28,merchant_defined_data29,payment_method,currency,merchant_defined_data3,merchant_defined_data20,merchant_defined_data22,merchant_defined_data30,merchant_defined_data31,merchant_defined_data35,merchant_defined_data5,merchant_defined_data6,merchant_defined_data7,merchant_defined_data8,merchant_defined_data9,merchant_defined_data10,device_fingerprint_id,merchant_defined_data50,merchant_defined_data51,merchant_defined_data52"},
                {"unsigned_field_names", "card_type,card_number,card_expiry_date,card_cvn"},

                {"access_key", cybersourceAccessKey},
                {"profile_id", cybersourceProfileId},
                {"transaction_uuid", item.TransactionReferenceNumber},
                {"signed_date_time", DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss'Z'")},
                {"locale", "en"},
                {"transaction_type", "create_payment_token"},
                {"reference_number", item.TransactionReferenceNumber},
                {"consumer_id", item.CyberSourceIdentifier},
                {"override_custom_receipt_page", item.UrlReturn },
                {"payment_method", "card"},
                {"currency", "UYU"},
                {"merchant_defined_data2", item.OperationTypeDto.ToString()},
                {"merchant_defined_data3", "Y"},
                {"merchant_defined_data5", !string.IsNullOrEmpty(item.ReferenceNumber1) && item.ReferenceNumber1.Length > 99 ? item.ReferenceNumber1.Substring(0,99) : item.ReferenceNumber1},
                {"merchant_defined_data6", !string.IsNullOrEmpty(item.ReferenceNumber2) && item.ReferenceNumber2.Length > 99 ? item.ReferenceNumber2.Substring(0,99) : item.ReferenceNumber2},
                {"merchant_defined_data7", !string.IsNullOrEmpty(item.ReferenceNumber3) && item.ReferenceNumber3.Length > 99 ? item.ReferenceNumber3.Substring(0,99) : item.ReferenceNumber3},
                {"merchant_defined_data8", !string.IsNullOrEmpty(item.ReferenceNumber4) && item.ReferenceNumber4.Length > 99 ? item.ReferenceNumber4.Substring(0,99) : item.ReferenceNumber4},
                {"merchant_defined_data9", !string.IsNullOrEmpty(item.ReferenceNumber5) && item.ReferenceNumber5.Length > 99 ? item.ReferenceNumber5.Substring(0,99) : item.ReferenceNumber5},
                {"merchant_defined_data10", !string.IsNullOrEmpty(item.ReferenceNumber6) && item.ReferenceNumber6.Length > 99 ? item.ReferenceNumber6.Substring(0,99) : item.ReferenceNumber6},
                {"merchant_defined_data11", item.RedirectTo},
                {"merchant_defined_data18", !string.IsNullOrEmpty(item.NameTh) && item.NameTh.Length > 99 ? item.NameTh.Substring(0,99) : item.NameTh},
                {"merchant_defined_data20", !string.IsNullOrEmpty(user.MobileNumber) && user.MobileNumber.Length > 99 ? user.MobileNumber.Substring(0,99) : user.MobileNumber},
                {"merchant_defined_data22", !string.IsNullOrEmpty(user.IdentityNumber) && user.IdentityNumber.Length > 99 ? user.IdentityNumber.Substring(0,99) : user.IdentityNumber},
                {"merchant_defined_data24", item.CardBin},
                {"merchant_defined_data26", string.IsNullOrEmpty(item.CallcenterUser) ? string.Empty : item.CallcenterUser},
                {"merchant_defined_data27", string.IsNullOrEmpty(item.OperationId) ? string.Empty : item.OperationId},
                {"merchant_defined_data28", string.IsNullOrEmpty(item.Platform) ? string.Empty : item.Platform},
                {"merchant_defined_data29", string.IsNullOrEmpty(item.TemporaryTransactionIdentifier) ? string.Empty : item.TemporaryTransactionIdentifier},
                {"merchant_defined_data30", item.ServiceId.ToString()},
                {"merchant_defined_data31", user.Id.ToString()},
                {"merchant_defined_data35", item.PaymentTypeDto.ToString()},
                {"merchant_defined_data50", item.CommerceAndProductName},
                {"merchant_defined_data51", item.CommerceId.ToString()},
                {"merchant_defined_data52", item.ProductId.ToString()},

                {"device_fingerprint_id", item.FingerPrint},
            };
            LoadBillData(ref keys, user);
            keys.Add("signature", Security.Sign(keys, cybersourceCybersourceSecretKey));

            return keys;
        }

        public string GetCardNumberByToken(CybersourceGetCardNameDto dto)
        {
            var request = new RequestMessage()
            {
                merchantID = dto.MerchantId,
                paySubscriptionRetrieveService = new PaySubscriptionRetrieveService()
                {
                    run = "true"
                },
                recurringSubscriptionInfo = new RecurringSubscriptionInfo()
                {
                    subscriptionID = dto.Token
                },
                merchantReferenceCode = dto.MerchantReferenceCode
            };
            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var proc = new TransactionProcessorClient();
                proc.ChannelFactory.Credentials.UserName.UserName = dto.MerchantId;

                var transactionKey = string.IsNullOrEmpty(dto.TransactionKey) ? _serviceParameters.GetParametersForCard().CybersourceTransactionKey : dto.TransactionKey;

                proc.ChannelFactory.Credentials.UserName.Password = transactionKey;

                var reply = proc.runTransaction(request);

                return reply.paySubscriptionRetrieveReply.cardAccountNumber;
            }
            catch (TimeoutException e)
            {
                NLogLogger.LogEvent(NLogType.Error, "Cybersource - Exception Metodo GetCardNumberByToken");
                NLogLogger.LogEvent(e);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_TIMEOUT);
            }
            catch (FaultException e)
            {
                NLogLogger.LogEvent(NLogType.Error, "Cybersource - Exception Metodo GetCardNumberByToken");
                NLogLogger.LogEvent(e);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_FAULT);
            }
            catch (CommunicationException e)
            {
                NLogLogger.LogEvent(NLogType.Error, "Cybersource - Exception Metodo VoidPayment");
                NLogLogger.LogEvent(e);
                throw new CybersourceException(CodeExceptions.CYBERSOURCE_COMMUNICATION);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "Cybersource - Exception Metodo VoidPayment");
                NLogLogger.LogEvent(e);
                throw new CybersourceException(CodeExceptions.GENERAL_COMUNICATION_ERROR);
            }
        }

        private string SerializeNotificationconfig(NotificationConfigDto notificationModel)
        {
            return JsonConvert.SerializeObject(ToDto(notificationModel));
        }

        private object[] ToDto(NotificationConfigDto notificationConfig)
        {
            var final = new object[6];
            final[0] = notificationConfig.DaysBeforeDueDate.ToString();
            final[1] = NotificationConfigBasicData(notificationConfig.BeforeDueDateConfigDto);
            final[2] = NotificationConfigBasicData(notificationConfig.ExpiredBillDto);
            final[3] = NotificationConfigBasicData(notificationConfig.FailedAutomaticPaymentDto);
            final[4] = NotificationConfigBasicData(notificationConfig.NewBillDto);
            final[5] = NotificationConfigBasicData(notificationConfig.SuccessPaymentDto);
            return final;
        }

        private object[] NotificationConfigBasicData(IBasicNotifConfig basicNotifConfig)
        {
            return new object[] { basicNotifConfig.Email ? "1" : "0", basicNotifConfig.Sms ? "1" : "0", basicNotifConfig.Web ? "1" : "0" };
        }

        private bool IsTesting()
        {
            var environment = ConfigurationManager.AppSettings["CsEnvironment"];
            if (string.IsNullOrEmpty(environment)) return false;

            return environment.ToUpper().Equals("TEST");
        }

        private string GetMerchandForThisEnviroment(string serviceMerchandId)
        {
            return IsTesting() ? "visanetuy" : serviceMerchandId;
        }

        private string LimitStringLength(string text, int size)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (text.Length > size)
            {
                return text.Substring(0, size);
            }

            return text;
        }

        private string GenerateField48(GeneratePayment payment, CyberSourceMerchantDefinedDataDto cyberSourceMerchantDefinedData)
        {
            var result = string.Empty;

            var max = Enum.GetValues(typeof(CardTypeDto)).Cast<int>().Max();
            var min = Enum.GetValues(typeof(CardTypeDto)).Cast<int>().Min();

            if ((int)payment.AdditionalInfo.CardTypeDto < min || (int)payment.AdditionalInfo.CardTypeDto > max)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("CARDTYPE INVALIDO. Plataforma {0}, CardType {1}, mddCardtype {2}, monto {3}",
                    payment.PaymentPlatform, payment.AdditionalInfo.CardTypeDto, cyberSourceMerchantDefinedData.CardTypeDto, payment.GrandTotalAmount));
                throw new BusinessException(CodeExceptions.PAYMENT_FIELD48_WRONG);
            }

            #region Como se arma el string
            //1-2 2 Plan type. Set this value to 00. Specifies that the transaction is an e-commerce transaction.
            //3 1 Grace period. Number of months that the issuer waits before charging customers.
            //4-5 2 Total number of installments. Possible values: 00 through 99.
            //6 1 POS entry mode. Set this value to 0. Specifies that the transaction is an e-commerce transaction.
            //7-15 9 Identity document number. Set this value to the number on the cardholder’s identity document or leave it blank.
            //Format: right justified with 0 (zero) padding on the left.
            //16 1 Financial inclusion law indicator. Possible values:
            // 1: Law 17934
            // 2: Law 18099
            // 3: Asignaciones familiares (AFAM) (family allowance program)
            // 4: Real state law
            // 5: Law 19210
            //17-28 12 Financial inclusion amount. This value is the amount the bank returns to the cardholder.
            //29-35 7 Merchant-generated invoice number. 


            //digito del 1 - 6 credito

            //digito del 7 en adelante debito
            //<c:issuer>
            //    <c:additionalData>003000         60000000000681234567</c:additionalData>
            //</c:issuer>

            // no paso cedula en ningun caso. NO SE USA. Pedido ANDRES
            #endregion

            try
            {
                var plan = "00";
                var gracePeriod = "0";
                var quotas = payment.Quota.ToString().PadLeft(2, '0');
                var pos = "0";
                var userCi = string.Empty.PadLeft(9, ' ');

                var discountApplied = (int)payment.AdditionalInfo.DiscountLabelTypeDto;
                double billDiscountAmount = 0;
                var culture = CultureInfo.CreateSpecificCulture("en-US");

                if (!string.IsNullOrEmpty(cyberSourceMerchantDefinedData.Discount))
                {
                    double.TryParse(cyberSourceMerchantDefinedData.Discount, NumberStyles.AllowDecimalPoint, culture, out billDiscountAmount);
                }

                var discount = (billDiscountAmount * 100).ToString("####").PadLeft(12, '0');
                var billNumber = !string.IsNullOrEmpty(cyberSourceMerchantDefinedData.BillNumber)
                    ? cyberSourceMerchantDefinedData.BillNumber
                    : cyberSourceMerchantDefinedData.BillSucivePreBillNumber;

                billNumber = billNumber.PadLeft(7, '0');
                if (billNumber.Count() > 7)
                {
                    billNumber = billNumber.Substring(billNumber.Count() - 7, 7);
                }

                if (payment.AdditionalInfo.CardTypeDto == CardTypeDto.Debit || payment.AdditionalInfo.CardTypeDto == CardTypeDto.InternationalDebit ||
                    payment.AdditionalInfo.CardTypeDto == CardTypeDto.NationalPrepaid || payment.AdditionalInfo.CardTypeDto == CardTypeDto.InternationalPrepaid)
                {
                    result = string.Format("{0}{1}{2}{3}{4}",
                        "003000",
                        userCi,
                        discountApplied,
                        discount,
                        billNumber);
                }
                else
                {
                    result = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                        plan,
                        gracePeriod,
                        quotas,
                        pos,
                        userCi,
                        discountApplied,
                        discount,
                        billNumber);
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "Excepcion - GenerateField48");
                NLogLogger.LogEvent(exception);
            }

            NLogLogger.LogEvent(NLogType.Info, "FIELD48: " + result);
            return result;
        }

        private string GenerateField48(IGenerateToken generateToken)
        {
            var item = (KeysInfoForPayment)generateToken;

            var result = string.Empty;

            var max = Enum.GetValues(typeof(CardTypeDto)).Cast<int>().Max();
            var min = Enum.GetValues(typeof(CardTypeDto)).Cast<int>().Min();

            if ((int)item.CardTypeDto < min || (int)item.CardTypeDto > max)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("CARDTYPE INVALIDO. Plataforma {0}, CardType {1}, monto {2}",
                    item.Platform, item.CardTypeDto, item.CybersourceAmount));
                throw new BusinessException(CodeExceptions.PAYMENT_FIELD48_WRONG);
            }
            #region Como se arma el string
            //1-2 2 Plan type. Set this value to 00. Specifies that the transaction is an e-commerce transaction.
            //3 1 Grace period. Number of months that the issuer waits before charging customers.
            //4-5 2 Total number of installments. Possible values: 00 through 99.
            //6 1 POS entry mode. Set this value to 0. Specifies that the transaction is an e-commerce transaction.
            //7-15 9 Identity document number. Set this value to the number on the cardholder’s identity document or leave it blank.
            //Format: right justified with 0 (zero) padding on the left.
            //16 1 Financial inclusion law indicator. Possible values:
            // 1: Law 17934
            // 2: Law 18099
            // 3: Asignaciones familiares (AFAM) (family allowance program)
            // 4: Real state law
            // 5: Law 19210
            //17-28 12 Financial inclusion amount. This value is the amount the bank returns to the cardholder.
            //29-35 7 Merchant-generated invoice number. 


            //digito del 1 - 6 credito

            //digito del 7 en adelante debito
            //<c:issuer>
            //    <c:additionalData>003000         60000000000681234567</c:additionalData>
            //</c:issuer>

            // no paso cedula en ningun caso. NO SE USA. Pedido ANDRES
            #endregion

            try
            {
                const string plan = "00";
                const string gracePeriod = "0";
                var quotas = item.Quotas.ToString().PadLeft(2, '0');
                const string pos = "0";
                var userCi = string.Empty.PadLeft(9, ' ');

                var discountApplied = (int)item.Bill.DiscountType;
                var billDiscountAmount = item.Bill.DiscountAmount;

                var discount = (billDiscountAmount * 100).ToString("####").PadLeft(12, '0');
                var billNumber = !string.IsNullOrEmpty(item.Bill.BillNumber)
                    ? item.Bill.BillNumber
                    : item.Bill.BillSucivePreBillNumber;

                billNumber = billNumber.PadLeft(7, '0');
                if (billNumber.Count() > 7)
                {
                    billNumber = billNumber.Substring(billNumber.Count() - 7, 7);
                }

                if (item.CardTypeDto == CardTypeDto.Debit || item.CardTypeDto == CardTypeDto.InternationalDebit ||
                    item.CardTypeDto == CardTypeDto.NationalPrepaid || item.CardTypeDto == CardTypeDto.InternationalPrepaid)
                {
                    result = string.Format("{0}{1}{2}{3}{4}",
                        "003000",
                        userCi,
                        discountApplied,
                        discount,
                        billNumber);
                }
                else
                {
                    result = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                        plan,
                        gracePeriod,
                        quotas,
                        pos,
                        userCi,
                        discountApplied,
                        discount,
                        billNumber);
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "Excepcion - GenerateField48");
                NLogLogger.LogEvent(exception);
            }

            NLogLogger.LogEvent(NLogType.Info, "FIELD48: " + result);
            return result;
        }

        private void ExecuteVoidOrReverseIfNecessary(string responseReasonCode, CancelPayment voidObj, RefundPayment reverseObj,
            bool isVoidObjectReady, bool isReverseObjectReady)
        {
            if (responseReasonCode.Equals(CodePaymentSuccess) && isVoidObjectReady)
            {
                //Intento realizar Void
                ExecuteVoid(voidObj);
            }
            if (responseReasonCode.Equals(CodePaymentDecisionManagerDecline) && isReverseObjectReady)
            {
                //Intento realizar Reverse
                ExecuteReverse(reverseObj);
            }
        }

        private void ExecuteVoid(CancelPayment cancel)
        {
            //Intento realizar Void
            try
            {
                VoidPayment(cancel);
            }
            catch (Exception ex)
            {
                NLogLogger.LogEvent(NLogType.Error, "Cybersource - ExecuteVoid - Exception - Error al intentar Void");
                NLogLogger.LogEvent(ex);
            }
        }

        private void ExecuteReverse(RefundPayment refund)
        {
            //Intento realizar Reverese
            try
            {
                ReversePayment(refund);
            }
            catch (Exception ex)
            {
                NLogLogger.LogEvent(NLogType.Error, "Cybersource - ExecuteReverse - Exception - Error al intentar Reverse llamado por un 481");
                NLogLogger.LogEvent(ex);
            }
        }

        private CancelPayment InitializeVoidPaymentObject(GeneratePayment payment, CyberSourceMerchantDefinedDataDto cyberSourceMerchantDefinedData)
        {
            CancelPayment voidObj = null;
            try
            {
                voidObj = new CancelPayment
                {
                    Amount = payment.GrandTotalAmount,
                    UserId = payment.ApplicationUserId,
                    Currency = payment.Currency,
                    UserType = payment.UserType,
                    PaymentPlatform = (PaymentPlatformDto)(int)payment.PaymentPlatform,
                    RequestId = null, //se obtiene despues
                    ServiceId = cyberSourceMerchantDefinedData.ServiceId,
                    Token = payment.Token,
                    IdOperation = cyberSourceMerchantDefinedData.OperationId,
                    IdTransaccion = null, //se obtiene despues
                    ServiceDto = new ServiceDto
                    {
                        MerchantId = null, //se obtiene despues
                        CybersourceTransactionKey = payment.Key.Trim(),
                    }
                };
            }
            catch (Exception)
            {
            }
            return voidObj;
        }

        private RefundPayment InitializeReversePaymentObject(GeneratePayment payment, CyberSourceMerchantDefinedDataDto cyberSourceMerchantDefinedData)
        {
            RefundPayment reverseObj = null;
            try
            {
                reverseObj = new RefundPayment
                {
                    Amount = payment.GrandTotalAmount,
                    UserId = payment.ApplicationUserId,
                    Currency = payment.Currency,
                    UserType = payment.UserType,
                    PaymentPlatform = (PaymentPlatformDto)(int)payment.PaymentPlatform,
                    RequestId = null, //se obtiene despues
                    ServiceId = cyberSourceMerchantDefinedData.ServiceId,
                    Token = payment.Token,
                    IdOperation = cyberSourceMerchantDefinedData.OperationId,
                    IdTransaccion = null, //se obtiene despues
                    ServiceDto = new ServiceDto
                    {
                        MerchantId = null, //se obtiene despues
                        CybersourceTransactionKey = payment.Key.Trim(),
                    }
                };
            }
            catch (Exception)
            {
            }
            return reverseObj;
        }

    }
}