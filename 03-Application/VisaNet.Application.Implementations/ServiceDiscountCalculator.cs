using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VisaNet.Application.Interfaces;
using VisaNet.Common.ApiClient;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Services;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.LIF.Domain;

namespace VisaNet.Application.Implementations
{
    public class ServiceDiscountCalculator : IServiceDiscountCalculator
    {
        private readonly IServiceService _serviceService;
        private readonly IServiceBin _serviceBin;
        private readonly IServiceDiscount _serviceDiscount;
        private readonly ILoggerService _loggerService;
        private readonly ITransactionContext _transactionContext;

        public ServiceDiscountCalculator(IServiceService serviceService, IServiceBin serviceBin, IServiceDiscount serviceDiscount, ILoggerService loggerService, ITransactionContext transactionContext)
        {
            _serviceService = serviceService;
            _serviceBin = serviceBin;
            _serviceDiscount = serviceDiscount;
            _loggerService = loggerService;
            _transactionContext = transactionContext;
        }

        public bool ValidateBin(int binNumber, Guid serviceId)
        {
            var service = _serviceService.GetById(serviceId);

            var acitveBin = GetActiveBin(binNumber, serviceId, service.Name);

            return acitveBin.Active;
        }

        public List<CyberSourceExtraDataDto> Calculate(DiscountQueryDto discountQuery)
        {
            var billsDiscount = new List<CyberSourceExtraDataDto>();

            var service = _serviceService.GetById(discountQuery.ServiceId);

            //Si el BIN pasado es válido y activo 
            var activeBin = GetActiveBin(discountQuery, service.Name);

            if (activeBin == null)
            {
                throw new BusinessException(CodeExceptions.BIN_NOTACTIVE);
            }

            //Validación de que el BIN esté asociado al servicio (si y solo si el servicio es especificado)
            if (discountQuery.ServiceId != Guid.Empty)
            {
                if (!IsBinAssociatedToSerice(discountQuery.ServiceId, activeBin.Value.ToString()))
                {
                    throw new BusinessException(CodeExceptions.BIN_NOTVALID_FOR_SERVICE);
                }
            }

            //var quotationUi = this.GetUIQuotationForCurrentDate();
            var discountDto = GetDiscountForBinAndService(activeBin, service);

            foreach (var bill in discountQuery.Bills)
            {
                bill.Discount = 0;
                bill.DiscountAmount = 0;

                var discountCalculation = LIFApiClient<DiscountCalculation>.GetDiscount(Mapper, bill, discountQuery.BinNumber.ToString(), discountQuery.ServiceId, (int)service.DiscountType, _transactionContext);

                bill.Discount = discountCalculation.Discount;
                bill.DiscountAmount = discountCalculation.DiscountAmount;

                var finalData = new CyberSourceExtraDataDto
                {
                    CybersourceAmount = discountCalculation.NetAmount,
                    DiscountDto = discountDto,
                    BinNumber = activeBin.Value,
                    BillDto = bill
                };

                billsDiscount.Add(finalData);
            }

            return billsDiscount;
        }

        private bool IsBinAssociatedToSerice(Guid serviceId, string binValue)
        {
            var intBin = int.Parse(binValue);
            return _serviceService.IsBinAssociatedToService(intBin, serviceId);
        }

        private DiscountDto GetDiscountForBinAndService(BinDto bin, ServiceDto service)
        {
            var discountDto = _serviceDiscount.GetDiscount(DateTime.Now.Date, bin.CardType, service.DiscountType);
            if (discountDto == null)
            {
                NLogLogger.LogEvent(NLogType.Error, "Calculo de descuento - DISCOUNT_NO_DISCOUNT_FOR_CARDTYPE__INTERNAL");
                NLogLogger.LogEvent(NLogType.Info, string.Format("DESCUNETO INEXISTENTE. TIPO TARJETA {0}. TIPO DESCUENTO {1}. FECHA {2}", bin.CardType, service.DiscountType, DateTime.Now.Date));

                this._loggerService.CreateLog(
                    LogType.Error,
                    LogOperationType.DiscountCalculation,
                    LogCommunicationType.VisaNet,
                    string.Empty,
                    string.Format(
                        ExceptionMessages.DISCOUNT_NO_DISCOUNT_FOR_CARDTYPE__INTERNAL,
                        DateTime.Now.ToString("d"),
                        bin.Value));

                throw new FatalException(
                    CodeExceptions.GENERAL_ERROR,
                    CodeExceptions.DISCOUNT_NO_DISCOUNT_FOR_CARDTYPE__INTERNAL,
                    DateTime.Now.Date.ToShortDateString(),
                    EnumHelpers.GetName(typeof(CardTypeDto), (int)bin.CardType, EnumsStrings.ResourceManager));
            }

            discountDto.DiscountLawDescription = String.Concat(EnumHelpers.GetName(typeof(DiscountLabelTypeDto), (int)discountDto.DiscountLabel, EnumsStrings.ResourceManager), " ");

            return discountDto;
        }

        private BinDto GetActiveBin(DiscountQueryDto discountQuery, string serviceName)
        {
            var bin = _serviceBin.AllNoTracking(null, b => b.Value == discountQuery.BinNumber).FirstOrDefault();

            if (bin == null)
            {
                //TODO GREYLOG AGREGAR PARA CUANDO SE INGRESA UN BIN QUE NO SE CONOCE

                bin = _serviceBin.GetDefaultBin();
                NLogLogger.LogEvent(NLogType.Info, string.Format("BIN INEXISTENTE {0}. OBTENGO POR DEFAULT {1}", discountQuery.BinNumber, bin != null ? bin.Value.ToString() : "null"));
            }

            if (!bin.Active)
            {
                NLogLogger.LogEvent(
                    NLogType.Error,
                    "Calculo de descuento - Se intento pagar con un bin inactivo. Bin: " + bin.Value);
                this._loggerService.CreateLog(
                    LogType.Error,
                    LogOperationType.DiscountCalculation,
                    LogCommunicationType.VisaNet,
                    string.Empty,
                    string.Format(ExceptionMessages.BIN_ERROR_NOSERVICE, serviceName, discountQuery.ServiceId, bin.Value));

            }

            return bin;
        }

        private BinDto GetActiveBin(int binNumber, Guid serviceId, string serviceName)
        {

            return GetActiveBin(new DiscountQueryDto
            {
                BinNumber = binNumber,
                ServiceId = serviceId
            }, serviceName);
           
        }

        private DiscountCalculation Mapper(string json)
        {
            var content = JsonConvert.DeserializeObject<dynamic>(json);
            return new DiscountCalculation()
            {
                Discount = (int)content.Discount,
                DiscountAmount = (double)content.DiscountAmount,
                NetAmount = (double)content.NetAmount,
            };
        }

    }
}