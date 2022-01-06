using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Repository;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Application.Implementations
{
    public class ServiceAnalyzeCsCall : IServiceAnalyzeCsCall
    {
        private readonly ILoggerService _loggerService;
        private readonly ILoggerRepository _repositoryLog;
        private readonly IRepositoryCard _repositoryCard;

        public ServiceAnalyzeCsCall(ILoggerService loggerService, ILoggerRepository repositoryLog, IRepositoryCard repositoryCard)
        {
            _loggerService = loggerService;
            _repositoryLog = repositoryLog;
            _repositoryCard = repositoryCard;
        }

        public CybersourceTransactionsDataDto ProcessCybersourceOperation(IDictionary<string, string> cybersourceData)
        {
            var csTransactionsDataDto = new CybersourceTransactionsDataDto();
            try
            {
                var code = int.Parse(cybersourceData["reason_code"]);
                var method = Int32.Parse(cybersourceData["req_merchant_defined_data11"]);
                var transactionId = cybersourceData.ContainsKey("transaction_id") ? cybersourceData["transaction_id"] : string.Empty;

                //ME FIJO SI YA EXISTE EL CODIGO EN BD: ES UN ERROR (no controla para tokenizaciones)
                //SI ME RECHAZO CS, NO TRAE NRO DE TRANSACCION
                if (!string.IsNullOrEmpty(transactionId))
                {
                    const string sql = "SELECT 1 FROM LogPaymentCyberSources WHERE CyberSourceLogData_TransactionId = @nroTrns";
                    var exists = _repositoryLog.ExecuteSQL<object>(sql, new[] { new SqlParameter("@nroTrns", transactionId) }).Any();
                    if (exists)
                    {
                        NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceAnalyzeCsCall - ProcessCybersourceOperation - Codigo {0} REPETIDO. ", transactionId));
                        return null;
                    }
                }

                var mdd = GenerateMdd(cybersourceData);

                LogTokenization(cybersourceData, code, transactionId, method, mdd);

                csTransactionsDataDto.CyberSourceMerchantDefinedData = mdd;
                csTransactionsDataDto.VerifyByVisaData = GenerateVbVisaDataDto(cybersourceData);
                csTransactionsDataDto.CyberSourceData = GenerateCyberSourceDataDto(cybersourceData);

                var errorDesc = EvaluateErrors(code);
                if (!string.IsNullOrEmpty(errorDesc))
                {
                    //CODIGO DISTINTO DE 100. ENVIAR ERROR 
                    return GenerateErrorInfo(csTransactionsDataDto, code, errorDesc, transactionId, method);
                }

                switch (method)
                {
                    case (int)RedirectEnums.Payment:
                    case (int)RedirectEnums.VisanetMobilePayment:
                    case (int)RedirectEnums.VisaNetOnPaymentAnonymous:
                    case (int)RedirectEnums.VisaNetOnPaymentRegisteredNewToken:
                    case (int)RedirectEnums.VisaNetOnPaymentRegisteredWithToken:
                        //Generar dato de pago nuevo
                        csTransactionsDataDto.PaymentData = GenerateCsResponseData(transactionId, code);
                        csTransactionsDataDto.PaymentDto = GeneratePaymentDto(cybersourceData, mdd, transactionId);
                        break;
                    case (int)RedirectEnums.VisaNetOnPaymentRecurrentNewToken:
                    case (int)RedirectEnums.VisaNetOnPaymentRecurrentWithToken:
                        //Generar dato de pago nuevo usuario recurrente VisaNetOn
                        csTransactionsDataDto.PaymentData = GenerateCsResponseData(transactionId, code);
                        csTransactionsDataDto.PaymentDto = GeneratePaymentDtoForRecurrentUser(cybersourceData, mdd, transactionId);
                        break;
                    case (int)RedirectEnums.VisaNetOnPaymentNewUser:
                        //Generar dato de pago nuevo y datos para registrar usuario recurrente VisaNetOn
                        csTransactionsDataDto.PaymentData = GenerateCsResponseData(transactionId, code);
                        csTransactionsDataDto.PaymentDto = GeneratePaymentDtoForNewUser(cybersourceData, mdd, transactionId);
                        break;
                    case (int)RedirectEnums.PrivateAssosiate:
                        //Generar dato de tarjeta nueva
                        csTransactionsDataDto.TokenizationData = GenerateCsResponseData(transactionId, code);
                        csTransactionsDataDto.PaymentDto = GeneratePaymentDtoForNewServiceAssociated(cybersourceData, mdd, transactionId);
                        break;
                    case (int)RedirectEnums.PrivateAddCardToUser:
                    case (int)RedirectEnums.VisanetMobileAddCard:
                        //Generar dato de tarjeta nueva
                        csTransactionsDataDto.TokenizationData = GenerateCsResponseData(transactionId, code);
                        csTransactionsDataDto.PaymentDto = GeneratePaymentDtoForTokenization(cybersourceData, mdd, transactionId);
                        break;
                    case (int)RedirectEnums.HighwayAdmission:
                    case (int)RedirectEnums.AppAdmission:
                        //Generar dato de tarjeta nueva
                        csTransactionsDataDto.TokenizationData = GenerateCsResponseData(transactionId, code);
                        csTransactionsDataDto.PaymentDto = GeneratePaymentDtoForApps(cybersourceData, mdd, transactionId);
                        break;
                    case (int)RedirectEnums.VisaNetOnTokenizationRegistered:
                        //Generar dato de tarjeta nueva
                        csTransactionsDataDto.TokenizationData = GenerateCsResponseData(transactionId, code);
                        csTransactionsDataDto.PaymentDto = GeneratePaymentDtoForApps(cybersourceData, mdd, transactionId);
                        break;
                    case (int)RedirectEnums.VisaNetOnTokenizationNewUser:
                    case (int)RedirectEnums.VisaNetOnTokenizationRecurrent:
                        //Generar dato de tarjeta nueva y datos para registrar usuario recurrente VisaNetOn
                        csTransactionsDataDto.TokenizationData = GenerateCsResponseData(transactionId, code);
                        csTransactionsDataDto.PaymentDto = GeneratePaymentDtoForRecurrentUserTokenization(cybersourceData, mdd, transactionId);
                        break;
                    case (int)RedirectEnums.Debit:
                        csTransactionsDataDto.TokenizationData = GenerateCsResponseData(transactionId, code);
                        csTransactionsDataDto.DebitRequestDto = GeneratePaymentDtoDebit(cybersourceData, mdd, transactionId);
                        break;
                }

                csTransactionsDataDto.PaymentDto.CyberSourceData = csTransactionsDataDto.CyberSourceData;
                csTransactionsDataDto.PaymentDto.VerifyByVisaData = csTransactionsDataDto.VerifyByVisaData;
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceAnalyzeCsCall - Exception");
                NLogLogger.LogEvent(exception);
            }
            return csTransactionsDataDto;
        }

        private string EvaluateErrors(int reasonCode)
        {
            var code = (CybersourceMsg)reasonCode;
            if (code != CybersourceMsg.Accepted)
            {
                #region ErrorCodes

                /* 100 : ok
                 * 102 : [msj datos inválidos] error de campos             
                 * 202 : [msj datos inválidos] tarjeta vencida
                 * 204 : [msj datos inválidos] fondos insuficientes 
                 * 205 : [msj datos inválidos] tarjeta declarada como robada o perdida             
                 * 207 : [msj datos inválidos] error de banco             
                 * 208 : [msj datos inválidos] tarjeta inactiva / no autorizada
                 * 210 : [msj datos inválidos] limite de crédito excedido
                 * 211 : [msj datos inválidos] CVN inválido
                 * 222 : [msj datos inválidos] cuenta congelada
                 * 231 : [msj datos inválidos] número de cuenta inválido
                 * 232 : [msj datos inválidos] el tipo de tarjeta no es aceptada por el procesador             
                 * 240 : [msj datos inválidos] El tipo de tarjeta es inválido o no correpsonde con el nro de cuenta
                 * 230 : [msj datos inválidos] Autorización aceptada, declinada por cybersource (CVN Check)
                 * 104 : [página error] error de keys
                 * 200 : [página error] Autorización aceptada, rechazada por cybersource por no pasar el AVS
                 * 201 : [página error] Autorización automatica rechazada
                 * 233 : [página error] Negación general
                 * 234 : [página error] Error en datos del usuario en cybersource
                 * 236 : [página error] Error general del procesador
                 * 475 : [página error] Payer authentication error
                 * 476 : [página error] Payer authentication error
                 * 520 : [página error] Autorización aceptada, declinada por cybersource
                 */

                #endregion

                switch (code)
                {
                    case CybersourceMsg.InvalidFields:
                    case CybersourceMsg.CardIssuer:
                    case CybersourceMsg.AuthorizationRejected:
                    case CybersourceMsg.ExpiredCard:
                    case CybersourceMsg.GeneralDecline:
                    case CybersourceMsg.InsufficientFunds:
                    case CybersourceMsg.StolenLostCard:
                    case CybersourceMsg.BankUnavailable:
                    case CybersourceMsg.InactiveUnAuthorizedCard:
                    case CybersourceMsg.CreditLimitReached:
                    case CybersourceMsg.InvalidCVN:
                    case CybersourceMsg.AccountFrozen:
                    case CybersourceMsg.CVNCheckInvalid:
                    case CybersourceMsg.InvalidAccountNumber:
                    case CybersourceMsg.CardTypeNotAccepted:
                    case CybersourceMsg.GeneralDeclineByProcessor:
                    case CybersourceMsg.ProcessorFailure:
                    case CybersourceMsg.InvalidCardTypeOrNotCorrelateWithCardNumber:
                    case CybersourceMsg.PayerAuthenticationError:
                        return InvalidCardData(code);

                    case CybersourceMsg.AVSCheckInvalid:
                    case CybersourceMsg.UserCybersourceError:
                    case CybersourceMsg.PayerAuthenticationNotAuthenticated:
                    case CybersourceMsg.AuthorizationDeclinedByCyberSourceSmartAuthorizationSettings:
                    case CybersourceMsg.ConfigurationKeysInvalids:
                        return PresentationWebStrings.Card_GeneralError;
                }
            }

            return null;
        }

        private string InvalidCardData(CybersourceMsg reasonCode)
        {
            var msg = string.Empty;
            switch (reasonCode)
            {
                case CybersourceMsg.InvalidFields:
                    msg = PresentationWebStrings.Card_InvalidFields;
                    break;
                case CybersourceMsg.CardIssuer:
                    msg = PresentationWebStrings.Card_Issuer;
                    break;
                case CybersourceMsg.ExpiredCard:
                    msg = PresentationWebStrings.Card_ExpiriedCard;
                    break;
                case CybersourceMsg.InsufficientFunds:
                    msg = PresentationWebStrings.Card_InsufficientFunds;
                    break;
                case CybersourceMsg.StolenLostCard:
                    msg = PresentationWebStrings.Card_StolenLostCard;
                    break;
                case CybersourceMsg.CreditLimitReached:
                    msg = PresentationWebStrings.Card_CreditLimitReached;
                    break;
                case CybersourceMsg.InvalidCVN:
                    msg = PresentationWebStrings.Card_InvalidCVN;
                    break;
                case CybersourceMsg.AccountFrozen:
                    msg = PresentationWebStrings.Card_AccountFrozen;
                    break;
                case CybersourceMsg.InvalidAccountNumber:
                    msg = PresentationWebStrings.Card_InvalidAccountNumber;
                    break;
                case CybersourceMsg.CardTypeNotAccepted:
                    msg = PresentationWebStrings.Card_CardTypeNotAccepted;
                    break;
                case CybersourceMsg.InvalidCardTypeOrNotCorrelateWithCardNumber:
                    msg = PresentationWebStrings.Card_InvalidCardTypeOrNotCorrelateWithCardNumber;
                    break;
                case CybersourceMsg.CVNCheckInvalid:
                    msg = PresentationWebStrings.Card_CVNCheckInvalid;
                    break;
                default:
                    msg = PresentationWebStrings.Card_GeneralError;
                    break;
            }
            return msg;
        }

        private CyberSourceMerchantDefinedDataDto GenerateMdd(IDictionary<string, string> csDictionary)
        {
            var mdd = new CyberSourceMerchantDefinedDataDto();
            mdd.ServiceType = !csDictionary.ContainsKey("req_merchant_defined_data1") ? string.Empty : csDictionary["req_merchant_defined_data1"];
            mdd.OperationTypeDto = !csDictionary.ContainsKey("req_merchant_defined_data2") ? string.Empty : csDictionary["req_merchant_defined_data2"];
            mdd.UserRegistered = !csDictionary.ContainsKey("req_merchant_defined_data3") ? string.Empty : csDictionary["req_merchant_defined_data3"];
            mdd.UserRegisteredDays = !csDictionary.ContainsKey("req_merchant_defined_data4") ? string.Empty : csDictionary["req_merchant_defined_data4"];
            mdd.ReferenceNumber1 = !csDictionary.ContainsKey("req_merchant_defined_data5") ? string.Empty : csDictionary["req_merchant_defined_data5"];
            mdd.ReferenceNumber2 = !csDictionary.ContainsKey("req_merchant_defined_data6") ? string.Empty : csDictionary["req_merchant_defined_data6"];
            mdd.ReferenceNumber3 = !csDictionary.ContainsKey("req_merchant_defined_data7") ? string.Empty : csDictionary["req_merchant_defined_data7"];
            mdd.ReferenceNumber4 = !csDictionary.ContainsKey("req_merchant_defined_data8") ? string.Empty : csDictionary["req_merchant_defined_data8"];
            mdd.ReferenceNumber5 = !csDictionary.ContainsKey("req_merchant_defined_data9") ? string.Empty : csDictionary["req_merchant_defined_data9"];
            mdd.ReferenceNumber6 = !csDictionary.ContainsKey("req_merchant_defined_data10") ? string.Empty : csDictionary["req_merchant_defined_data10"];
            mdd.RedirctTo = !csDictionary.ContainsKey("req_merchant_defined_data11") ? string.Empty : csDictionary["req_merchant_defined_data11"];
            mdd.DiscountApplyed = !csDictionary.ContainsKey("req_merchant_defined_data12") ? string.Empty : csDictionary["req_merchant_defined_data12"];
            mdd.TotalAmount = !csDictionary.ContainsKey("req_merchant_defined_data13") ? string.Empty : csDictionary["req_merchant_defined_data13"];
            mdd.TotalTaxedAmount = !csDictionary.ContainsKey("req_merchant_defined_data14") ? string.Empty : csDictionary["req_merchant_defined_data14"];
            mdd.Discount = !csDictionary.ContainsKey("req_merchant_defined_data15") ? string.Empty : csDictionary["req_merchant_defined_data15"];
            mdd.BillNumber = !csDictionary.ContainsKey("req_merchant_defined_data16") ? string.Empty : csDictionary["req_merchant_defined_data16"];
            mdd.AditionalNumberElectornicBill = !csDictionary.ContainsKey("req_merchant_defined_data17") ? string.Empty : csDictionary["req_merchant_defined_data17"];
            mdd.NameTh = !csDictionary.ContainsKey("req_merchant_defined_data18") ? string.Empty : csDictionary["req_merchant_defined_data18"];
            mdd.PaymentCount = !csDictionary.ContainsKey("req_merchant_defined_data19") || string.IsNullOrEmpty(csDictionary["req_merchant_defined_data19"]) ? 0 : int.Parse(csDictionary["req_merchant_defined_data19"]);
            mdd.UserMobile = !csDictionary.ContainsKey("req_merchant_defined_data20") ? string.Empty : csDictionary["req_merchant_defined_data20"];
            mdd.UserRegisteredAddress = !csDictionary.ContainsKey("req_merchant_defined_data21") ? string.Empty : csDictionary["req_merchant_defined_data21"];
            mdd.UserCi = !csDictionary.ContainsKey("req_merchant_defined_data22") ? string.Empty : csDictionary["req_merchant_defined_data22"];
            mdd.MerchandId = !csDictionary.ContainsKey("req_merchant_defined_data23") ? string.Empty : csDictionary["req_merchant_defined_data23"];
            mdd.CardBin = !csDictionary.ContainsKey("req_merchant_defined_data24") ? string.Empty : csDictionary["req_merchant_defined_data24"];
            mdd.ServiceName = !csDictionary.ContainsKey("req_merchant_defined_data25") ? string.Empty : csDictionary["req_merchant_defined_data25"];
            mdd.CallcenterUser = !csDictionary.ContainsKey("req_merchant_defined_data26") ? string.Empty : csDictionary["req_merchant_defined_data26"];
            mdd.OperationId = !csDictionary.ContainsKey("req_merchant_defined_data27") ? string.Empty : csDictionary["req_merchant_defined_data27"];
            mdd.Plataform = !csDictionary.ContainsKey("req_merchant_defined_data28") ? string.Empty : csDictionary["req_merchant_defined_data28"];
            mdd.TemporaryTransactionIdentifier = !csDictionary.ContainsKey("req_merchant_defined_data29") ? string.Empty : csDictionary["req_merchant_defined_data29"];
            mdd.ServiceId = !csDictionary.ContainsKey("req_merchant_defined_data30") ? Guid.Empty : Guid.Parse(csDictionary["req_merchant_defined_data30"]);
            mdd.UserId = !csDictionary.ContainsKey("req_merchant_defined_data31") ? Guid.Empty : Guid.Parse(csDictionary["req_merchant_defined_data31"]);
            mdd.CardId = !csDictionary.ContainsKey("req_merchant_defined_data32") ? Guid.Empty : Guid.Parse(csDictionary["req_merchant_defined_data32"]);
            mdd.GatewayId = !csDictionary.ContainsKey("req_merchant_defined_data33") ? Guid.Empty : Guid.Parse(csDictionary["req_merchant_defined_data33"]);
            mdd.DiscountObjId = !csDictionary.ContainsKey("req_merchant_defined_data34") ? Guid.Empty : Guid.Parse(csDictionary["req_merchant_defined_data34"]);
            mdd.BillExpirationDate = !csDictionary.ContainsKey("req_merchant_defined_data36") ? string.Empty : csDictionary["req_merchant_defined_data36"];
            mdd.BillDescription = !csDictionary.ContainsKey("req_merchant_defined_data37") ? string.Empty : csDictionary["req_merchant_defined_data37"];
            mdd.BillGatewayTransactionId = !csDictionary.ContainsKey("req_merchant_defined_data38") ? string.Empty : csDictionary["req_merchant_defined_data38"];
            mdd.BillSucivePreBillNumber = !csDictionary.ContainsKey("req_merchant_defined_data39") ? string.Empty : csDictionary["req_merchant_defined_data39"];
            mdd.BillFinalConsumer = !csDictionary.ContainsKey("req_merchant_defined_data40") ? string.Empty : csDictionary["req_merchant_defined_data40"];
            mdd.BillDiscount = !csDictionary.ContainsKey("req_merchant_defined_data41") ? string.Empty : csDictionary["req_merchant_defined_data41"];
            mdd.BillDateInitTransaccion = !csDictionary.ContainsKey("req_merchant_defined_data42") ? string.Empty : csDictionary["req_merchant_defined_data42"];
            mdd.BillGatewayTransactionBrouId = !csDictionary.ContainsKey("req_merchant_defined_data43") ? string.Empty : csDictionary["req_merchant_defined_data43"];
            mdd.NotificationConfig = !csDictionary.ContainsKey("req_merchant_defined_data44") ? string.Empty : csDictionary["req_merchant_defined_data44"];
            mdd.DescriptionService = !csDictionary.ContainsKey("req_merchant_defined_data45") ? string.Empty : csDictionary["req_merchant_defined_data45"];
            mdd.PasswordHashed = !csDictionary.ContainsKey("req_merchant_defined_data46") ? string.Empty : csDictionary["req_merchant_defined_data46"];
            mdd.Quota = !csDictionary.ContainsKey("req_merchant_defined_data47") ? 1 : int.Parse(csDictionary["req_merchant_defined_data47"]);
            mdd.CommerceAndProduct = !csDictionary.ContainsKey("req_merchant_defined_data50") ? string.Empty : csDictionary["req_merchant_defined_data50"];
            mdd.CommerceId = !csDictionary.ContainsKey("req_merchant_defined_data51") ? Guid.Empty : Guid.Parse(csDictionary["req_merchant_defined_data51"]);
            mdd.ProductId = !csDictionary.ContainsKey("req_merchant_defined_data52") ? Guid.Empty : Guid.Parse(csDictionary["req_merchant_defined_data52"]);

            var paymentTypeDto = PaymentTypeDto.AnonymousUser;
            if (csDictionary.ContainsKey("req_merchant_defined_data35"))
            {
                Enum.TryParse<PaymentTypeDto>(csDictionary["req_merchant_defined_data35"], true, out paymentTypeDto);
            }
            mdd.PaymentTypeDto = paymentTypeDto;

            var cardTypeDto = CardTypeDto.Credit;
            if (csDictionary.ContainsKey("req_merchant_defined_data48"))
            {
                Enum.TryParse<CardTypeDto>(csDictionary["req_merchant_defined_data48"], true, out cardTypeDto);
            }
            mdd.CardTypeDto = cardTypeDto;

            return mdd;
        }

        private CyberSourceDataDto GenerateCyberSourceDataDto(IDictionary<string, string> csDictionary)
        {
            var cyberSourceData = new CyberSourceDataDto
            {
                Decision = csDictionary.ContainsKey("decision") ? csDictionary["decision"] : string.Empty,
                ReasonCode = csDictionary.ContainsKey("reason_code") ? csDictionary["reason_code"] : string.Empty,
                TransactionId = csDictionary.ContainsKey("transaction_id") ? csDictionary["transaction_id"] : string.Empty,
                Message = csDictionary.ContainsKey("message") ? csDictionary["message"] : string.Empty,
                BillTransRefNo = csDictionary.ContainsKey("bill_trans_ref_no") ? csDictionary["bill_trans_ref_no"] : string.Empty,
                ReqCardNumber = csDictionary.ContainsKey("req_card_number") ? csDictionary["req_card_number"] : string.Empty,
                ReqCardExpiryDate = csDictionary.ContainsKey("req_card_expiry_date") ? csDictionary["req_card_expiry_date"] : string.Empty,
                ReqProfileId = csDictionary.ContainsKey("req_profile_id") ? csDictionary["req_profile_id"] : string.Empty,
                ReqCardType = csDictionary.ContainsKey("req_card_type") ? csDictionary["req_card_type"] : string.Empty,
                ReqPaymentMethod = csDictionary.ContainsKey("req_payment_method") ? csDictionary["req_payment_method"] : string.Empty,
                ReqTransactionType = csDictionary.ContainsKey("req_transaction_type") ? csDictionary["req_transaction_type"] : string.Empty,
                ReqTransactionUuid = csDictionary.ContainsKey("req_transaction_uuid") ? csDictionary["req_transaction_uuid"] : string.Empty,
                ReqCurrency = csDictionary.ContainsKey("req_currency") ? csDictionary["req_currency"] : string.Empty,
                ReqReferenceNumber = csDictionary.ContainsKey("req_reference_number") ? csDictionary["req_reference_number"] : string.Empty,
                ReqAmount = csDictionary.ContainsKey("req_amount") ? csDictionary["req_amount"] : string.Empty,
                AuthAvsCode = csDictionary.ContainsKey("auth_avs_code") ? csDictionary["auth_avs_code"] : string.Empty,
                AuthCode = csDictionary.ContainsKey("auth_code") ? csDictionary["auth_code"] : string.Empty,
                AuthAmount = csDictionary.ContainsKey("auth_amount") ? csDictionary["auth_amount"] : string.Empty,
                AuthTime = csDictionary.ContainsKey("auth_time") ? csDictionary["auth_time"] : string.Empty,
                AuthResponse = csDictionary.ContainsKey("auth_response") ? csDictionary["auth_response"] : string.Empty,
                AuthTransRefNo = csDictionary.ContainsKey("auth_trans_ref_no") ? csDictionary["auth_trans_ref_no"] : string.Empty,
                PaymentToken = csDictionary.ContainsKey("payment_token") ? csDictionary["payment_token"] : string.Empty,
            };
            return cyberSourceData;
        }
        private VerifyByVisaDataDto GenerateVbVisaDataDto(IDictionary<string, string> csDictionary)
        {
            //si el csDictionaryulario contiene la clave _xid es porque se utilizo verifybyvisa en el proceso
            //se guardan los datos relacionados a verifybyvisa
            var verifyByVisaData = new VerifyByVisaDataDto
            {
                PayerAuthenticationEci = csDictionary.ContainsKey("payer_authentication_eci") ? csDictionary["payer_authentication_eci"] : string.Empty,
                PayerAuthenticationXid = csDictionary.ContainsKey("payer_authentication_xid") ? csDictionary["payer_authentication_xid"] : string.Empty,
                PayerAuthenticationCavv = csDictionary.ContainsKey("payer_authentication_cavv") ? csDictionary["payer_authentication_cavv"] : string.Empty,
                PayerAuthenticationProofXml = csDictionary.ContainsKey("payer_authentication_proof_xml") ? csDictionary["payer_authentication_proof_xml"] : string.Empty,
            };
            return verifyByVisaData;
        }

        private CsResponseData GenerateCsResponseData(string transactionId, int code)
        {
            return new CsResponseData()
            {
                PaymentRequestId = transactionId,
                PaymentResponseCode = code,
            };
        }

        private PaymentDto GenerateBasePaymentDto(IDictionary<string, string> csDictionary, CyberSourceMerchantDefinedDataDto mdd, string transactionId)
        {
            var paymentDto = new PaymentDto
            {
                ServiceId = mdd.ServiceId,
                Date = DateTime.Now,
                GatewayId = mdd.GatewayId,
                ReferenceNumber = mdd.ReferenceNumber1,
                ReferenceNumber2 = mdd.ReferenceNumber2,
                ReferenceNumber3 = mdd.ReferenceNumber3,
                ReferenceNumber4 = mdd.ReferenceNumber4,
                ReferenceNumber5 = mdd.ReferenceNumber5,
                ReferenceNumber6 = mdd.ReferenceNumber6,
                PaymentStatus = PaymentStatusDto.Done,
                PaymentType = mdd.PaymentTypeDto,
                AmountTocybersource = double.Parse(csDictionary["req_amount"].ConvertLocal()),
                TransactionNumber = transactionId,
                Quotas = mdd.Quota
            };

            //SE TIENE QUE GENERAR UNA FACTURA SI ES TRUE
            if (csDictionary["req_transaction_type"].ToLower().Contains("sale"))
            {
                var billDto = new BillDto()
                {
                    BillExternalId = mdd.BillNumber,
                    SucivePreBillNumber = mdd.BillSucivePreBillNumber,
                    GatewayTransactionId = mdd.BillGatewayTransactionId,
                    GatewayTransactionBrouId = mdd.BillGatewayTransactionBrouId,
                    Description = mdd.BillDescription,
                    DateInitTransaccion = mdd.BillDateInitTransaccion,

                    FinalConsumer = mdd.BillFinalConsumer.ToLower().Equals("true"),
                    Currency = csDictionary["req_currency"],
                    Amount = double.Parse(mdd.TotalAmount.ConvertLocal()),
                    TaxedAmount = double.Parse(mdd.TotalTaxedAmount.ConvertLocal()),
                    DiscountAmount = double.Parse(mdd.Discount.ConvertLocal()),
                    Discount = int.Parse(mdd.BillDiscount.ConvertLocal()),
                    ExpirationDate = GetBillDueDateTime(mdd.BillExpirationDate),
                    GatewayId = mdd.GatewayId,
                };
                paymentDto.Bills = new List<BillDto>() { billDto };
                paymentDto.Currency = billDto.Currency;
                paymentDto.TotalAmount = billDto.Amount;
                paymentDto.TotalTaxedAmount = billDto.TaxedAmount;
                paymentDto.Discount = billDto.DiscountAmount;
                paymentDto.DiscountApplyed = billDto.DiscountAmount > 0;
            }

            PaymentPlatformDto plataform;
            Enum.TryParse(mdd.Plataform, true, out plataform);
            paymentDto.PaymentPlatform = plataform;

            if (mdd.DiscountObjId != Guid.Empty)
                paymentDto.DiscountObjId = mdd.DiscountObjId;

            return paymentDto;
        }
        private PaymentDto GeneratePaymentDto(IDictionary<string, string> csDictionary, CyberSourceMerchantDefinedDataDto mdd, string transactionId)
        {
            var paymentDto = GenerateBasePaymentDto(csDictionary, mdd, transactionId);

            if (mdd.UserRegistered.ToLower().Equals("y"))
            {
                paymentDto.RegisteredUserId = mdd.UserId;
                paymentDto.RegisteredUser = new ApplicationUserDto
                {
                    Id = mdd.UserId,
                    CyberSourceIdentifier = long.Parse(csDictionary["req_consumer_id"]),
                    Email = csDictionary["req_bill_to_email"],
                    Name = csDictionary.ContainsKey("req_bill_to_forename") ? csDictionary["req_bill_to_forename"] : string.Empty,
                    Surname = csDictionary.ContainsKey("req_bill_to_surname") ? csDictionary["req_bill_to_surname"] : string.Empty,
                    Address = csDictionary.ContainsKey("req_merchant_defined_data21") ? csDictionary["req_merchant_defined_data21"] : string.Empty,
                    MobileNumber = csDictionary.ContainsKey("req_merchant_defined_data20") ? csDictionary["req_merchant_defined_data20"] : string.Empty,
                    PhoneNumber = csDictionary.ContainsKey("req_bill_to_phone") ? csDictionary["req_bill_to_phone"] : string.Empty,
                    IdentityNumber = csDictionary.ContainsKey("req_merchant_defined_data22") ? csDictionary["req_merchant_defined_data22"] : string.Empty,
                };
            }
            else
            {
                paymentDto.AnonymousUserId = mdd.UserId;
                paymentDto.AnonymousUser = new AnonymousUserDto
                {
                    Id = mdd.UserId,
                    CyberSourceIdentifier = long.Parse(csDictionary["req_consumer_id"]),
                    Email = csDictionary["req_bill_to_email"],
                    Name = csDictionary.ContainsKey("req_bill_to_forename") ? csDictionary["req_bill_to_forename"] : string.Empty,
                    Surname = csDictionary.ContainsKey("req_bill_to_surname") ? csDictionary["req_bill_to_surname"] : string.Empty,
                    Address = csDictionary.ContainsKey("req_merchant_defined_data21") ? csDictionary["req_merchant_defined_data21"] : string.Empty,
                    MobileNumber = csDictionary.ContainsKey("req_merchant_defined_data20") ? csDictionary["req_merchant_defined_data20"] : string.Empty,
                    PhoneNumber = csDictionary.ContainsKey("req_bill_to_phone") ? csDictionary["req_bill_to_phone"] : string.Empty,
                    IdentityNumber = csDictionary.ContainsKey("req_merchant_defined_data22") ? csDictionary["req_merchant_defined_data22"] : string.Empty,
                };
            }

            //SE TIENE QUE GENERAR UNA TARJETA NUEVA SI ES TRUE
            if (mdd.CardId == Guid.Empty)
            {
                paymentDto.Card = GenerateCardDto(csDictionary, mdd, transactionId);
            }
            else
            {
                paymentDto.CardId = mdd.CardId;
                var card = _repositoryCard.GetById(mdd.CardId);
                paymentDto.Card = card != null ? new CardDto
                {
                    Id = card.Id,
                    Active = card.Active,
                    CybersourceTransactionId = card.CybersourceTransactionId,
                    Deleted = card.Deleted,
                    DueDate = card.DueDate,
                    ExternalId = card.ExternalId,
                    MaskedNumber = card.MaskedNumber,
                    Name = card.Name,
                    PaymentToken = card.PaymentToken
                } : null;
            }
            return paymentDto;
        }
        private PaymentDto GeneratePaymentDtoForNewUser(IDictionary<string, string> csDictionary, CyberSourceMerchantDefinedDataDto mdd, string transactionId)
        {
            var paymentDto = GenerateBasePaymentDto(csDictionary, mdd, transactionId);

            paymentDto.AnonymousUserId = mdd.UserId;
            paymentDto.AnonymousUser = new AnonymousUserDto
            {
                Id = mdd.UserId,
                CyberSourceIdentifier = long.Parse(csDictionary["req_consumer_id"]),
                Email = csDictionary["req_bill_to_email"],
                Name = csDictionary.ContainsKey("req_bill_to_forename") ? csDictionary["req_bill_to_forename"] : string.Empty,
                Surname = csDictionary.ContainsKey("req_bill_to_surname") ? csDictionary["req_bill_to_surname"] : string.Empty,
                Address = csDictionary.ContainsKey("req_merchant_defined_data21") ? csDictionary["req_merchant_defined_data21"] : string.Empty,
                MobileNumber = csDictionary.ContainsKey("req_merchant_defined_data20") ? csDictionary["req_merchant_defined_data20"] : string.Empty,
                PhoneNumber = csDictionary.ContainsKey("req_bill_to_phone") ? csDictionary["req_bill_to_phone"] : string.Empty,
                IdentityNumber = csDictionary.ContainsKey("req_merchant_defined_data22") ? csDictionary["req_merchant_defined_data22"] : string.Empty,
            };

            paymentDto.Card = GenerateCardDto(csDictionary, mdd, transactionId);
            return paymentDto;
        }
        private PaymentDto GeneratePaymentDtoForRecurrentUser(IDictionary<string, string> csDictionary, CyberSourceMerchantDefinedDataDto mdd, string transactionId)
        {
            var paymentDto = GenerateBasePaymentDto(csDictionary, mdd, transactionId);

            paymentDto.AnonymousUserId = mdd.UserId;
            paymentDto.AnonymousUser = new AnonymousUserDto
            {
                Id = mdd.UserId,
                CyberSourceIdentifier = long.Parse(csDictionary["req_consumer_id"]),
                Email = csDictionary["req_bill_to_email"],
                Name = csDictionary.ContainsKey("req_bill_to_forename") ? csDictionary["req_bill_to_forename"] : string.Empty,
                Surname = csDictionary.ContainsKey("req_bill_to_surname") ? csDictionary["req_bill_to_surname"] : string.Empty,
                Address = csDictionary.ContainsKey("req_merchant_defined_data21") ? csDictionary["req_merchant_defined_data21"] : string.Empty,
                MobileNumber = csDictionary.ContainsKey("req_merchant_defined_data20") ? csDictionary["req_merchant_defined_data20"] : string.Empty,
                PhoneNumber = csDictionary.ContainsKey("req_bill_to_phone") ? csDictionary["req_bill_to_phone"] : string.Empty,
                IdentityNumber = csDictionary.ContainsKey("req_merchant_defined_data22") ? csDictionary["req_merchant_defined_data22"] : string.Empty,
            };

            paymentDto.Card = GenerateCardDtoForRecurrentUser(csDictionary, mdd, transactionId);
            return paymentDto;
        }

        private DateTime GetBillDueDateTime(string dueDateString)
        {
            try
            {
                //dd-MM-YYYY
                var dueDateArray = dueDateString.Split('-');
                var dueDate = !string.IsNullOrEmpty(dueDateArray[2]) && !string.IsNullOrEmpty(dueDateArray[1]) &&
                              !string.IsNullOrEmpty(dueDateArray[0])
                    ? new DateTime(int.Parse(dueDateArray[2]), int.Parse(dueDateArray[1]), int.Parse(dueDateArray[0]))
                    : DateTime.Now;
                return dueDate;
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "ProcessCybersourceOperation - GetBillDueDateTime Exception");
                NLogLogger.LogEvent(exception);
            }
            return DateTime.Now;
        }

        private PaymentDto GeneratePaymentDtoForTokenization(IDictionary<string, string> csDictionary, CyberSourceMerchantDefinedDataDto mdd, string transactionId)
        {
            var paymentDto = new PaymentDto();
            paymentDto.RegisteredUserId = mdd.UserId;
            paymentDto.Card = GenerateCardDto(csDictionary, mdd, transactionId);

            paymentDto.ReferenceNumber = mdd.ReferenceNumber1;
            paymentDto.ReferenceNumber2 = mdd.ReferenceNumber2;
            paymentDto.ReferenceNumber3 = mdd.ReferenceNumber3;
            paymentDto.ReferenceNumber4 = mdd.ReferenceNumber4;
            paymentDto.ReferenceNumber5 = mdd.ReferenceNumber5;
            paymentDto.ReferenceNumber6 = mdd.ReferenceNumber6;

            return paymentDto;
        }
        private PaymentDto GeneratePaymentDtoForNewServiceAssociated(IDictionary<string, string> csDictionary, CyberSourceMerchantDefinedDataDto mdd, string transactionId)
        {
            var paymentDto = new PaymentDto();
            var card = GenerateCardDto(csDictionary, mdd, transactionId);
            var sAssociated = new ServiceAssociatedDto()
            {
                ServiceId = mdd.ServiceId,
                ReferenceNumber = mdd.ReferenceNumber1,
                ReferenceNumber2 = mdd.ReferenceNumber2,
                ReferenceNumber3 = mdd.ReferenceNumber3,
                ReferenceNumber4 = mdd.ReferenceNumber4,
                ReferenceNumber5 = mdd.ReferenceNumber5,
                ReferenceNumber6 = mdd.ReferenceNumber6,
                UserId = mdd.UserId,
                DefaultCard = card,
                NotificationConfigDto = GetNotificationConfigDto(mdd.NotificationConfig),
                Description = mdd.DescriptionService,
                OperationId = mdd.OperationId,
            };
            paymentDto.ServiceAssociatedDto = sAssociated;

            paymentDto.ReferenceNumber = mdd.ReferenceNumber1;
            paymentDto.ReferenceNumber2 = mdd.ReferenceNumber2;
            paymentDto.ReferenceNumber3 = mdd.ReferenceNumber3;
            paymentDto.ReferenceNumber4 = mdd.ReferenceNumber4;
            paymentDto.ReferenceNumber5 = mdd.ReferenceNumber5;
            paymentDto.ReferenceNumber6 = mdd.ReferenceNumber6;

            return paymentDto;
        }
        private PaymentDto GeneratePaymentDtoForApps(IDictionary<string, string> csDictionary, CyberSourceMerchantDefinedDataDto mdd, string transactionId)
        {
            var paymentDto = new PaymentDto();
            var card = GenerateCardDto(csDictionary, mdd, transactionId);
            var sAssociated = new ServiceAssociatedDto()
            {
                ServiceId = mdd.ServiceId,
                ReferenceNumber = mdd.ReferenceNumber1,
                ReferenceNumber2 = mdd.ReferenceNumber2,
                ReferenceNumber3 = mdd.ReferenceNumber3,
                ReferenceNumber4 = mdd.ReferenceNumber4,
                ReferenceNumber5 = mdd.ReferenceNumber5,
                ReferenceNumber6 = mdd.ReferenceNumber6,
                UserId = mdd.UserId,
                DefaultCard = card,
                NotificationConfigDto = GetNotificationConfigDto(mdd.NotificationConfig),
                Description = mdd.DescriptionService,
                OperationId = mdd.OperationId,
            };

            if (sAssociated.UserId == Guid.Empty)
            {
                sAssociated.RegisteredUserDto = GenerateApplicationUserDto(mdd, csDictionary);
            }

            paymentDto.ServiceAssociatedDto = sAssociated;

            paymentDto.ReferenceNumber = mdd.ReferenceNumber1;
            paymentDto.ReferenceNumber2 = mdd.ReferenceNumber2;
            paymentDto.ReferenceNumber3 = mdd.ReferenceNumber3;
            paymentDto.ReferenceNumber4 = mdd.ReferenceNumber4;
            paymentDto.ReferenceNumber5 = mdd.ReferenceNumber5;
            paymentDto.ReferenceNumber6 = mdd.ReferenceNumber6;

            return paymentDto;
        }
        private PaymentDto GeneratePaymentDtoForRecurrentUserTokenization(IDictionary<string, string> csDictionary, CyberSourceMerchantDefinedDataDto mdd, string transactionId)
        {
            var paymentDto = new PaymentDto();

            paymentDto.AnonymousUserId = mdd.UserId;
            paymentDto.AnonymousUser = new AnonymousUserDto
            {
                Id = mdd.UserId,
                CyberSourceIdentifier = long.Parse(csDictionary["req_consumer_id"]),
                Email = csDictionary["req_bill_to_email"],
                Name = csDictionary.ContainsKey("req_bill_to_forename") ? csDictionary["req_bill_to_forename"] : string.Empty,
                Surname = csDictionary.ContainsKey("req_bill_to_surname") ? csDictionary["req_bill_to_surname"] : string.Empty,
                Address = csDictionary.ContainsKey("req_merchant_defined_data21") ? csDictionary["req_merchant_defined_data21"] : string.Empty,
                MobileNumber = csDictionary.ContainsKey("req_merchant_defined_data20") ? csDictionary["req_merchant_defined_data20"] : string.Empty,
                PhoneNumber = csDictionary.ContainsKey("req_bill_to_phone") ? csDictionary["req_bill_to_phone"] : string.Empty,
                IdentityNumber = csDictionary.ContainsKey("req_merchant_defined_data22") ? csDictionary["req_merchant_defined_data22"] : string.Empty,
            };

            paymentDto.ServiceId = mdd.ServiceId;
            paymentDto.Card = GenerateCardDtoForRecurrentUser(csDictionary, mdd, transactionId);

            paymentDto.ReferenceNumber = mdd.ReferenceNumber1;
            paymentDto.ReferenceNumber2 = mdd.ReferenceNumber2;
            paymentDto.ReferenceNumber3 = mdd.ReferenceNumber3;
            paymentDto.ReferenceNumber4 = mdd.ReferenceNumber4;
            paymentDto.ReferenceNumber5 = mdd.ReferenceNumber5;
            paymentDto.ReferenceNumber6 = mdd.ReferenceNumber6;

            return paymentDto;
        }

        private CybersourceTransactionsDataDto GenerateErrorInfo(CybersourceTransactionsDataDto data, int code, string msg, string transactionId, int method)
        {
            var info = new CsResponseData
            {
                PaymentRequestId = transactionId,
                PaymentResponseCode = code,
                PaymentResponseMsg = msg,
            };
            if (method == (int)RedirectEnums.Payment ||
                method == (int)RedirectEnums.VisanetMobilePayment ||
                method == (int)RedirectEnums.VisaNetOnPaymentAnonymous ||
                method == (int)RedirectEnums.VisaNetOnPaymentNewUser ||
                method == (int)RedirectEnums.VisaNetOnPaymentRegisteredNewToken ||
                method == (int)RedirectEnums.VisaNetOnPaymentRegisteredWithToken ||
                method == (int)RedirectEnums.VisaNetOnPaymentRecurrentNewToken ||
                method == (int)RedirectEnums.VisaNetOnPaymentRecurrentWithToken)
            {
                //Generar dato de pago nuevo
                data.PaymentData = info;
            }
            else
            {
                data.TokenizationData = info;
            }

            return data;
        }

        private void LogTokenization(IDictionary<string, string> csDictionary, int code, string transactionId, int method, CyberSourceMerchantDefinedDataDto mdd)
        {
            var oldLog = csDictionary.ContainsKey("req_merchant_defined_data29") ? csDictionary["req_merchant_defined_data29"] : string.Empty;
            var userId = Guid.Parse(csDictionary["req_merchant_defined_data31"]);
            var strLog = string.Format(LogStrings.Cybersource_Analize_Init, code, transactionId);
            Guid tempGuid;
            Guid? tempGuid2 = null;
            tempGuid2 = Guid.TryParse(oldLog, out tempGuid) ? tempGuid : tempGuid2;

            var operationType = LogOperationType.BillPayment;

            PaymentPlatformDto plataform;
            Enum.TryParse(mdd.Plataform, true, out plataform);

            switch (method)
            {
                case (int)RedirectEnums.Payment:
                case (int)RedirectEnums.VisanetMobilePayment:
                case (int)RedirectEnums.VisaNetOnPaymentAnonymous:
                case (int)RedirectEnums.VisaNetOnPaymentRegisteredNewToken:
                case (int)RedirectEnums.VisaNetOnPaymentRegisteredWithToken:
                case (int)RedirectEnums.VisaNetOnPaymentNewUser:
                case (int)RedirectEnums.VisaNetOnPaymentRecurrentNewToken:
                case (int)RedirectEnums.VisaNetOnPaymentRecurrentWithToken:
                    operationType = LogOperationType.BillPayment;
                    break;
                case (int)RedirectEnums.PrivateAssosiate:
                case (int)RedirectEnums.HighwayAdmission:
                case (int)RedirectEnums.AppAdmission:
                    operationType = LogOperationType.ServiceAssociated;
                    break;
                case (int)RedirectEnums.PrivateAddCardToUser:
                case (int)RedirectEnums.VisanetMobileAddCard:
                case (int)RedirectEnums.VisaNetOnTokenizationRegistered:
                case (int)RedirectEnums.VisaNetOnTokenizationNewUser:
                case (int)RedirectEnums.VisaNetOnTokenizationRecurrent:
                case (int)RedirectEnums.Debit:
                    operationType = LogOperationType.NewCardAdded;
                    break;
            }
            if (csDictionary["req_merchant_defined_data3"].ToLower().Equals("y"))
            {
                _loggerService.CreateLog(LogType.Info, operationType, LogCommunicationType.VisaNet, userId, strLog, null, null, null, tempGuid2);
            }
            else
            {
                _loggerService.CreateLogForAnonymousUser(LogType.Info, operationType, LogCommunicationType.VisaNet, userId, strLog, null, null, null, tempGuid2);
            }
        }

        private CardDto GenerateCardDto(IDictionary<string, string> csDictionary, CyberSourceMerchantDefinedDataDto mdd, string transactionId)
        {
            //SE TIENE QUE GENERAR UNA TARJETA NUEVA SI ES TRUE
            if (mdd.CardId == Guid.Empty)
            {
                var dueDateString = csDictionary["req_card_expiry_date"];
                var splitedExpiry = dueDateString.Split('-');
                var dueDate = new DateTime(Convert.ToInt16(splitedExpiry[1]), Convert.ToInt16(splitedExpiry[0]), 1);
                var cardDto = new CardDto()
                {
                    Active = true,
                    CybersourceTransactionId = transactionId,
                    PaymentToken = csDictionary.ContainsKey("payment_token") ? csDictionary["payment_token"] : string.Empty,
                    MaskedNumber = csDictionary["req_card_number"],
                    Name = mdd.NameTh,
                    DueDate = dueDate,
                    ExternalId = Guid.NewGuid(),
                    Description = csDictionary.ContainsKey("card_description") ? csDictionary["card_description"] : string.Empty,
                };
                return cardDto;
            }
            return null;
        }

        private CardDto GenerateCardDtoForRecurrentUser(IDictionary<string, string> csDictionary, CyberSourceMerchantDefinedDataDto mdd, string transactionId)
        {
            //SE TIENE QUE GENERAR UNA TARJETA NUEVA SI ES TRUE
            if (mdd.CardId == Guid.Empty)
            {
                //Tarjeta nueva
                var dueDateString = csDictionary["req_card_expiry_date"];
                var splitedExpiry = dueDateString.Split('-');
                var dueDate = new DateTime(Convert.ToInt16(splitedExpiry[1]), Convert.ToInt16(splitedExpiry[0]), 1);
                var cardDto = new CardDto()
                {
                    Active = true,
                    CybersourceTransactionId = transactionId,
                    PaymentToken = csDictionary.ContainsKey("payment_token") ? csDictionary["payment_token"] : string.Empty,
                    MaskedNumber = csDictionary["req_card_number"],
                    Name = mdd.NameTh,
                    DueDate = dueDate,
                    ExternalId = Guid.NewGuid(),
                };
                return cardDto;
            }
            else
            {
                //Tarjeta existente en tabla VonData
                var dueDateString = csDictionary["req_card_expiry_date"];
                var splitedExpiry = dueDateString.Split('-');
                var dueDate = new DateTime(Convert.ToInt16(splitedExpiry[1]), Convert.ToInt16(splitedExpiry[0]), 1);
                var cardDto = new CardDto()
                {
                    Active = true,
                    CybersourceTransactionId = transactionId,
                    PaymentToken = csDictionary.ContainsKey("req_payment_token") ? csDictionary["req_payment_token"] : string.Empty,
                    MaskedNumber = csDictionary["req_card_number"],
                    Name = mdd.NameTh,
                    DueDate = dueDate,
                    ExternalId = mdd.CardId,
                };
                return cardDto;
            }
            return null;
        }

        private NotificationConfigDto GetNotificationConfigDto(string jsonData)
        {
            if (!string.IsNullOrEmpty(jsonData))
            {
                var info = Newtonsoft.Json.JsonConvert.DeserializeObject<object[]>(jsonData);
                return ToDto(info);
            }
            return new NotificationConfigDto()
            {
                DaysBeforeDueDate = 5,
                SuccessPaymentDto = new SuccessPaymentDto() { Email = true },
                NewBillDto = new NewBillDto(),
                BeforeDueDateConfigDto = new DaysBeforeDueDateConfigDto(),
                ExpiredBillDto = new ExpiredBillDto(),
                FailedAutomaticPaymentDto = new FailedAutomaticPaymentDto(),
            };
        }

        private NotificationConfigDto ToDto(object[] data)
        {
            var notificationConfig = new NotificationConfigDto
            {
                DaysBeforeDueDate = int.Parse(data[0].ToString()),
                BeforeDueDateConfigDto = DeserializeObject(data[1].ToString(), new DaysBeforeDueDateConfigDto()) as DaysBeforeDueDateConfigDto,
                ExpiredBillDto = DeserializeObject(data[2].ToString(), new ExpiredBillDto()) as ExpiredBillDto,
                FailedAutomaticPaymentDto = DeserializeObject(data[3].ToString(), new FailedAutomaticPaymentDto()) as FailedAutomaticPaymentDto,
                NewBillDto = DeserializeObject(data[4].ToString(), new NewBillDto()) as NewBillDto,
                SuccessPaymentDto = DeserializeObject(data[5].ToString(), new SuccessPaymentDto()) as SuccessPaymentDto
            };
            return notificationConfig;
        }

        private IBasicNotifConfig DeserializeObject(string info, IBasicNotifConfig data)
        {
            var infoDeserialize = Newtonsoft.Json.JsonConvert.DeserializeObject<object[]>(info);
            data.Email = infoDeserialize[0].ToString().Equals("1");
            data.Sms = infoDeserialize[1].ToString().Equals("1");
            data.Web = infoDeserialize[2].ToString().Equals("1");
            return data;
        }

        private ApplicationUserDto GenerateApplicationUserDto(CyberSourceMerchantDefinedDataDto mdd, IDictionary<string, string> csDictionary)
        {
            return new ApplicationUserDto
            {
                Email = csDictionary["req_bill_to_email"],
                Name = csDictionary.ContainsKey("req_bill_to_forename") ? csDictionary["req_bill_to_forename"] : string.Empty,
                Surname = csDictionary.ContainsKey("req_bill_to_surname") ? csDictionary["req_bill_to_surname"] : string.Empty,
                PhoneNumber = csDictionary.ContainsKey("req_bill_to_phone") ? csDictionary["req_bill_to_phone"] : string.Empty,
                MobileNumber = mdd.UserMobile,
                Address = mdd.UserRegisteredAddress,
                Password = mdd.PasswordHashed,
                IdentityNumber = mdd.UserCi,
            };
        }

        private DebitRequestDto GeneratePaymentDtoDebit(IDictionary<string, string> csDictionary, CyberSourceMerchantDefinedDataDto mdd, string transactionId)
        {
            var debit = new DebitRequestDto()
            {
                UserId = mdd.UserId,
                ApplicationUserDto = GenerateApplicationUserDto(mdd, csDictionary),
                State = DebitRequestStateDto.Pending,
                Type = DebitRequestTypeDto.High,
                References = new List<DebitRequestReferenceDto>(),
                CardDto = GenerateCardDto(csDictionary, mdd, transactionId)
            };

            return debit;
        }

    }
}