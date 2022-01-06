using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Ninject;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.WebService;
using VisaNet.WebService.VisaWCF.EntitiesDto;
using VisaNet.WebService.VisaWCF.NLog;
using VisaNet.WebService.VisaWCF.Mappers;

namespace VisaNet.WebService.VisaWCF
{
    public class VisaNetAccess : IVisaNetAccess
    {
        #region Attributes & Properties

        private IWsBankClientService _wsBankClientService;

        private IWsBankClientService Service
        {
            get
            {
                if (_wsBankClientService == null)
                {
                    NinjectRegister.Register(new StandardKernel(), true);

                    _wsBankClientService = NinjectRegister.Get<IWsBankClientService>();
                }

                return _wsBankClientService;
            }
        }

        #endregion

        #region Public Methods

        public ServicesResponse GetServices(ServicesData data)
        {
            CustomLogger.LogEvent(NLogType.Info, "WCF  GetServices - " + DateTime.Now.ToString("G"));

            ServicesResponse response;
            try
            {
                CustomLogger.LogGetServicesEvent(data);

                var inputDataValidationResult = ValidateInputData(data, new[] { data.PaymentPlatform });

                if (inputDataValidationResult.ResponseCode != VisaNetAccessResponseCode.Error)
                {
                    var services = this.Service.AllServices().GetAwaiter().GetResult();
                    response = new ServicesResponse(VisaNetAccessResponseCode.Ok) { Response = services.Select(s => s.ToVisaNetServiceResult()).ToList() };
                }
                else
                {
                    response = new ServicesResponse(VisaNetAccessResponseCode.Error) { ResponseError = inputDataValidationResult.ResponseError };
                }
            }
            catch (Exception exception)
            {
                CustomLogger.LogEvent(exception);
                response = new ServicesResponse(VisaNetAccessResponseCode.Error) { ResponseError = new ResponseError { ResponseText = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.ERROR_GENERAL, EnumsStrings.ResourceManager), ResponseType = (int)ErrorCodeDto.ERROR_GENERAL } };
            }

            return response;
        }

        public BillsResponse SearchBills(BillsData data)
        {
            CustomLogger.LogEvent(NLogType.Info, "WCF  SearchBills - " + DateTime.Now.ToString("G"));

            BillsResponse response;
            try
            {
                CustomLogger.LogSearchBillsEvent(data);

                var inputDataValidationResult = ValidateInputData(data, data.ToParamsArray());

                if (inputDataValidationResult.ResponseCode != VisaNetAccessResponseCode.Error)
                {
                    var result = Service.GetBills(data.ToBillDto()).GetAwaiter().GetResult();

                    if (result.ErrorCode == ErrorCodeDto.OK)
                    {
                        var merchantReferenceCode = Guid.NewGuid().ToString();
                        response = new BillsResponse(VisaNetAccessResponseCode.Ok) { Response = result.Bills.Select(x => x.ToVisaNetBillResult(data, result, merchantReferenceCode)).ToList() };
                    }
                    else
                    {
                        response = new BillsResponse(VisaNetAccessResponseCode.Error) { ResponseError = new ResponseError { ResponseText = result.ErrorMessage, ResponseType = (int)result.ErrorCode } };   
                    }
                }
                else
                {
                    response = new BillsResponse(VisaNetAccessResponseCode.Error) { ResponseError = inputDataValidationResult.ResponseError };
                }
            }
            catch (Exception exception)
            {
                CustomLogger.LogEvent(exception);
                response = new BillsResponse(VisaNetAccessResponseCode.Error) { ResponseError = new ResponseError { ResponseText = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.ERROR_GENERAL, EnumsStrings.ResourceManager), ResponseType = (int)ErrorCodeDto.ERROR_GENERAL } };
            }

            return response;
        }

        public PaymentResponse Payment(PaymentData data)
        {
            CustomLogger.LogEvent(NLogType.Info, "WCF  Payment - " + DateTime.Now.ToString("G"));

            PaymentResponse response;

            var validationResult = ValidatePayment(data);
            if (validationResult.ResponseCode == VisaNetAccessResponseCode.Ok)
            {
                response = ProcessPayment(data);
            }
            else
            {
                response = new PaymentResponse(VisaNetAccessResponseCode.Error) { ResponseError = validationResult.ResponseError };
            }

            return response;
        }

        public PreprocessPaymentResponse PreprocessPayment(PreprocessPaymentData data)
        {
            CustomLogger.LogEvent(NLogType.Info, "WCF  PreprocessPayment - " + DateTime.Now.ToString("G"));

            PreprocessPaymentResponse response;
            try
            {
                CustomLogger.LogPreprocessPaymentEvent(data);

                var inputDataValidationResult = ValidateInputData(data, data.ToParamsArray());

                if (inputDataValidationResult.ResponseCode != VisaNetAccessResponseCode.Error)
                {
                    var payment = Service.PreprocessPayment(data.ToPreprocessPaymentDto()).GetAwaiter().GetResult();

                    if (payment.ErrorCode == ErrorCodeDto.OK)
                    {
                        var billsDtos = payment.Bills;
                        var cybersourceExtraDataList = Service.CalculateDiscount(new WsBankPreprocessPaymentInputDto
                        {
                            Bills = new List<BillDto>(billsDtos),
                            ServiceId = data.Bills.FirstOrDefault().ServiceId,
                            CardBinNumbers = data.Bills.FirstOrDefault().CardBinNumbers
                        }).GetAwaiter().GetResult();

                        var result = new List<VisaNetBillResponse>();
                        foreach (var cs in cybersourceExtraDataList)
                        {
                            result.Add(cs.BillDto.ToVisaNetPreprocessPaymentResult(data, cs, payment));
                        }

                        response = new PreprocessPaymentResponse(VisaNetAccessResponseCode.Ok) { Response = result };
                    }
                    else
                    {
                        response = new PreprocessPaymentResponse(VisaNetAccessResponseCode.Error)
                        {
                            ResponseError = new ResponseError()
                            {
                                ResponseText = payment.ErrorMessage,
                                ResponseType = (int)payment.ErrorCode
                            }
                        };
                    }
                }
                else
                {
                    response = new PreprocessPaymentResponse(VisaNetAccessResponseCode.Error) { ResponseError = inputDataValidationResult.ResponseError };
                }
            }
            catch (Exception exception)
            {
                CustomLogger.LogEvent(exception);
                response = new PreprocessPaymentResponse(VisaNetAccessResponseCode.Error)
                {
                    ResponseError = new ResponseError()
                    {
                        ResponseText = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.ERROR_GENERAL, EnumsStrings.ResourceManager),
                        ResponseType = (int)ErrorCodeDto.ERROR_GENERAL
                    }
                };
            }

            return response;
        }

        public SearchPaymentsResponse SearchPayments(SearchPaymentsData data)
        {
            CustomLogger.LogEvent(NLogType.Info, "WCF  SearchPayments - " + DateTime.Now.ToString("G"));

            SearchPaymentsResponse response;
            try
            {
                CustomLogger.LogSearchPaymentsEvent(data);
                
                var inputDataValidationResult = ValidateInputData(data, data.ToParamsArray());
                if (inputDataValidationResult.ResponseCode != VisaNetAccessResponseCode.Error)
                {
                    var payments = Service.GetPayments(data.ToSearchPaymentsDto()).GetAwaiter().GetResult();

                    response = new SearchPaymentsResponse(VisaNetAccessResponseCode.Ok) { Response = payments.Select(p => p.ToVisaNetSearchPaymentsResult()).ToList() };
                }
                else
                {
                    response = new SearchPaymentsResponse(VisaNetAccessResponseCode.Error) { ResponseError = inputDataValidationResult.ResponseError };
                }
            }
            catch (Exception exception)
            {
                CustomLogger.LogEvent(exception);
                response = new SearchPaymentsResponse(VisaNetAccessResponseCode.Error) { ResponseError = new ResponseError { ResponseText = EnumHelpers.GetName(typeof (ErrorCodeDto), (int) ErrorCodeDto.ERROR_GENERAL, EnumsStrings.ResourceManager), ResponseType = (int) ErrorCodeDto.ERROR_GENERAL } };
            }

            return response;
        }

        public NotificationResponse NotifyOperationResult(NotificationData data)
        {
            NotificationResponse response;
            try
            {
                var inputDataValidationResult = ValidateInputData(data, data.ToParamsArray());
                if (inputDataValidationResult.ResponseCode != VisaNetAccessResponseCode.Error)
                {
                    CustomLogger.LogEvent(data.NotificationType.ToNLogType(), "WCF NotifyOperationResult - " + "Operation: '" + data.Operation + "', Message: '" + data.Message + "'");

                    response = new NotificationResponse(VisaNetAccessResponseCode.Ok);
                }
                else
                {
                    response = new NotificationResponse(VisaNetAccessResponseCode.Error) { ResponseError = inputDataValidationResult.ResponseError };
                }
            }
            catch (Exception )
            {
                response = new NotificationResponse(VisaNetAccessResponseCode.Error) { ResponseError = new ResponseError { ResponseText = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.ERROR_GENERAL, EnumsStrings.ResourceManager), ResponseType = (int)ErrorCodeDto.ERROR_GENERAL } };
            }

            return response;
        }

        #endregion

        #region Private Methods

        private VisaNetAccessBaseResponse ValidateInputData(VisaNetAccessBaseData data, string[] paramsArray)
        {
            var validateInputResponse = new VisaNetAccessBaseResponse(VisaNetAccessResponseCode.Ok);

            var validationResult = ValidateRequiredFields(data);
            if (validationResult.ResponseCode == VisaNetAccessResponseCode.Error)
            {
                validateInputResponse = new VisaNetAccessBaseResponse(VisaNetAccessResponseCode.Error) { ResponseError = validationResult.ResponseError };
                return validateInputResponse;
            }

            var checkResult = CheckDigitalSignature(paramsArray, data.PaymentPlatform, data.DigitalSignature);
            if (checkResult.ResponseCode == VisaNetAccessResponseCode.Error)
            {
                validateInputResponse = new VisaNetAccessBaseResponse(VisaNetAccessResponseCode.Error) { ResponseError = checkResult.ResponseError };
            }

            return validateInputResponse;
        }

        private PaymentResponse ValidatePayment(PaymentData data)
        {
            PaymentResponse response = new PaymentResponse(VisaNetAccessResponseCode.Ok);
            try
            {
                CustomLogger.LogPaymentEvent(data);

                var inputDataValidationResult = ValidateInputData(data, data.ToParamsArray());
                if (inputDataValidationResult.ResponseCode != VisaNetAccessResponseCode.Error)
                {
                    //Valido que el CardBinNumbers de la factura se corresponda con los datos de la tarjeta ingresados
                    if (data.Bill.CardBinNumbers != data.CardData.CardBinNumbers)
                    {
                        response = new PaymentResponse(VisaNetAccessResponseCode.Error) { ResponseError = new ResponseError { ResponseText = EnumHelpers.GetName(typeof (ErrorCodeDto), (int) ErrorCodeDto.EL_BIN_DE_LA_FACTURA_NO_SE_CORRESPONDE_CON_LOS_DATOS_DE_LA_TARJETA_INGRESADOS, EnumsStrings.ResourceManager), ResponseType = (int) ErrorCodeDto.EL_BIN_DE_LA_FACTURA_NO_SE_CORRESPONDE_CON_LOS_DATOS_DE_LA_TARJETA_INGRESADOS } };
                    }
                }
                else
                {
                    return new PaymentResponse(VisaNetAccessResponseCode.Error) { ResponseError = inputDataValidationResult.ResponseError };
                }
            }
            catch (Exception exception)
            {
                CustomLogger.LogEvent(exception);

                response = new PaymentResponse(VisaNetAccessResponseCode.Error) { ResponseError = new ResponseError { ResponseText = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.ERROR_GENERAL, EnumsStrings.ResourceManager), ResponseType = (int)ErrorCodeDto.ERROR_GENERAL } };
            }

            return response;
        }

        private PaymentResponse ProcessPayment(PaymentData data)
        {
            PaymentResponse response;
            try
            {
                IProcessPayment processPayment;

                var reasonCode = (ErrorCodeDto)Int32.Parse(data.CyberSourceData.ReasonCode);

                if (reasonCode == ErrorCodeDto.CYBERSOURCE_OK)
                {
                    processPayment = new ProcessPayment();
                }
                else if (reasonCode == ErrorCodeDto.DECISIONMANAGER)
                {
                    processPayment = new ProcessReverse();
                }
                else
                {
                    processPayment = new ProcessError();
                }

                response = processPayment.Process(Service, data);
            }
            catch (Exception exception)
            {
                CustomLogger.LogEvent(exception);
                response = new PaymentResponse(VisaNetAccessResponseCode.Error) { ResponseError = new ResponseError { ResponseText = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.ERROR_GENERAL_LUEGO_DE_REALIZADO_EL_PAGO_O_REVERSO_DE_LA_TRANSACCION, EnumsStrings.ResourceManager), ResponseType = (int)ErrorCodeDto.ERROR_GENERAL_LUEGO_DE_REALIZADO_EL_PAGO_O_REVERSO_DE_LA_TRANSACCION } };
            }

            return response;
        }

        private VisaNetAccessBaseResponse CheckDigitalSignature(string[] paramsArray, string paymentPlatform, string digitalSignature)
        {
            VisaNetAccessBaseResponse response;

            var certificateThumbprint = ConfigurationManager.AppSettings[paymentPlatform];
            if (!String.IsNullOrEmpty(certificateThumbprint))
            {
                var isDigitalSignatureValid = DigitalSignature.CheckSignature(paramsArray, digitalSignature, certificateThumbprint);

                if (isDigitalSignatureValid)
                {
                    response = new VisaNetAccessBaseResponse(VisaNetAccessResponseCode.Ok);    
                }
                else
                {
                    var errorMsg = string.Format("Error al validar los campos firmados. PaymentPlatform: {0} ", paymentPlatform);
                    CustomLogger.LogEvent(NLogType.Error, errorMsg);

                    response = new VisaNetAccessBaseResponse(VisaNetAccessResponseCode.Error) { ResponseError = new ResponseError { ResponseType = (int)ErrorCodeDto.FIRMA_INVALIDA, ResponseText = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.FIRMA_INVALIDA, EnumsStrings.ResourceManager) } };
                }
            }
            else
            {
                response = new VisaNetAccessBaseResponse(VisaNetAccessResponseCode.Error) { ResponseError = new ResponseError { ResponseType = (int)ErrorCodeDto.CERTIFICADO_NO_ENCONTRADO, ResponseText = EnumHelpers.GetName(typeof(ErrorCodeDto), (int)ErrorCodeDto.CERTIFICADO_NO_ENCONTRADO, EnumsStrings.ResourceManager) } };
            }

            return response;
        }

        private VisaNetAccessBaseResponse ValidateRequiredFields(VisaNetAccessBaseData data)
        {
            VisaNetAccessBaseResponse response;

            var errors = "";
            var valid = DataValidation.AreInputParametersValid(data, out errors);

            if (valid)
            {
                response = new VisaNetAccessBaseResponse(VisaNetAccessResponseCode.Ok);
            }
            else
            {
                var errorMsg = string.Format("Error al validar los campos requeridos. Mensaje:{0}.", errors);
                CustomLogger.LogEvent(NLogType.Error, errorMsg);
                response = new VisaNetAccessBaseResponse(VisaNetAccessResponseCode.Error) { ResponseError = new ResponseError { ResponseText = errors, ResponseType = (int)ErrorCodeDto.ERRORES_EN_LOS_CAMPOS_ENVIADOS } };
            }

            return response;
        }
    }

        #endregion
}
