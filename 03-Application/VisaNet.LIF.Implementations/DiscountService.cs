using System;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Lif.Domain.EntitesDtos;
using VisaNet.LIF.Interfaces;
using VisaNet.Utilities.ExtensionMethods;
using BillDto = VisaNet.Lif.Domain.EntitesDtos.BillDto;
using BinDto = VisaNet.Domain.EntitiesDtos.BinDto;

namespace VisaNet.LIF.Implementations
{
    public class DiscountService : IDiscountService
    {
        private readonly IServiceQuotation _exchangeRateService;
        private readonly IServiceBin _binService;
        private readonly IServiceService _serviceService;
        private readonly IServiceDiscount _discountService;

        public DiscountService(IServiceQuotation exchangeRateService, IServiceBin binService, IServiceService serviceService, IServiceDiscount discountService)
        {
            _exchangeRateService = exchangeRateService;
            _binService = binService;
            _serviceService = serviceService;
            _discountService = discountService;
        }

        /// <summary>
        /// Retorna el calculo del descuento para una factura dado un servicio
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="binValue"></param>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        public DiscountCalculationDto CalculateDiscount(BillDto billDto, string binValue, Guid serviceId)
        {
            //Si el BIN pasado es válido y activo 
            var activeBin = GetActiveBin(binValue, serviceId);
            var discountDto = GetDiscountForBinService(activeBin, serviceId);
            return Calculate(billDto, activeBin, discountDto);
        }

        /// <summary>
        /// Retorna el calculo del descuento para una factura
        /// </summary>
        /// <param name="bill"></param>
        /// <param name="binValue"></param>
        /// <returns></returns>
        public DiscountCalculationDto CalculateDiscount(BillDto billDto, string binValue)
        {
            //Si el BIN pasado es válido
            var bin = GetBin(binValue);
            var discountDto = GetDiscount(billDto.LawId, bin.CardType);
            return Calculate(billDto, bin, discountDto);
        }

        #region Private methods

        private BinDto GetBin(string binValue)
        {
            try
            {
                var intBin = int.Parse(binValue);
                var bin = _binService.AllNoTracking(null, x => x.Value == intBin, x => x.BinAuthorizationAmountTypeList).FirstOrDefault();

                if (bin == null)
                {
                    //TODO GREYLOG AGREGAR PARA CUANDO SE INGRESA UN BIN QUE NO SE CONOCE
                    bin = _binService.GetDefaultBin();
                }

                ValidateBin(bin, null);

                return bin;
            }
            catch (FormatException)
            {
                throw new BusinessException(CodeExceptions.BIN_VALUE_NOT_RECOGNIZED);
            }
        }

        private BinDto GetActiveBin(string binValue, Guid serviceId)
        {
            try
            {
                var intBinValue = int.Parse(binValue);
                var bin = _binService.AllNoTracking(null, b => b.Value == intBinValue, x => x.BinAuthorizationAmountTypeList).FirstOrDefault();

                if (bin == null)
                {
                    //TODO GREYLOG AGREGAR PARA CUANDO SE INGRESA UN BIN QUE NO SE CONOCE
                    bin = _binService.GetDefaultBin();
                }

                ValidateBin(bin, serviceId);

                if (!_serviceService.IsBinAssociatedToService(bin.Value, serviceId))
                {
                    throw new BusinessException(CodeExceptions.BIN_NOTVALID_FOR_SERVICE);
                }

                return bin;
            }
            catch (FormatException)
            {
                throw new BusinessException(CodeExceptions.BIN_VALUE_NOT_RECOGNIZED);
            }
        }

        private void ValidateBin(BinDto bin, Guid? serviceId)
        {
            if (!bin.Active)
            {
                if (!serviceId.HasValue)
                {
                    throw new BusinessException(CodeExceptions.BIN_NOTACTIVE2);
                }

                throw new BusinessException(CodeExceptions.BIN_NOTACTIVE);
            }
        }

        private int GetBillDiscount(Bill bill, QuotationDto quotationUi, DiscountDto discountDto)
        {
            var appliesDiscount = IsBillEligibleForDiscount(bill, discountDto, quotationUi);
            return appliesDiscount ? (discountDto.Fixed + discountDto.Additional) : 0;
        }

        private DiscountDto GetDiscount(int lawId, CardTypeDto cardType)
        {
            if (lawId <= (int)DiscountType.NoDiscount || lawId > (int)DiscountType.FinancialInclusion)
            {
                NLogLogger.LogEvent(NLogType.Info, "DiscountService - GetDiscount - Error - Indicador de ley invalido");
                throw new BusinessException(CodeExceptions.DISCOUNT_INVALID_LAW_ID);
            }

            var dtn = DateTime.Now;
            var discounts = _discountService.AllNoTracking(null, x =>
                x.DiscountType == (DiscountType)lawId
                && dtn >= x.From && dtn <= x.To
                && x.CardType == (CardType)cardType
                );

            //TODO: Agregar este error al sanity
            if (!discounts.Any())
            {
                NLogLogger.LogEvent(NLogType.Info, "DiscountService - GetDiscount - Error - No se encontro un descuento para el tipo de tarjeta y ley indicados");
                throw new FatalException(
                    CodeExceptions.GENERAL_ERROR,
                    CodeExceptions.DISCOUNT_NO_DISCOUNT_FOR_CARDTYPE__INTERNAL,
                    DateTime.Now.Date.ToShortDateString());
            }

            //TODO: Agregar este error al sanity
            if (discounts.Count() > 1)
            {
                NLogLogger.LogEvent(NLogType.Info, "DiscountService - GetDiscount - Error - Se encontraron multiples descuentos para el tipo de tarjeta y ley indicados");
                throw new FatalException(
                    CodeExceptions.GENERAL_ERROR,
                    CodeExceptions.DISCOUNT_MULTIPLE_DISCOUNT_FOR_CARDTYPE,
                    DateTime.Now.Date.ToShortDateString());
            }

            return discounts.First();
        }

        /// <summary>
        /// Si el monto es mayor al monto máximo del descuento, no aplica descuento.
        /// </summary>
        private bool IsBillEligibleForDiscount(Bill bill, DiscountDto discountDto, QuotationDto uiExchangeRate)
        {
            var isBillEligibleForDiscount = false;

            var totalAmount = bill.Amount;
            var billCurrency = bill.Currency.Trim();

            var isBillInLocalCurrency = billCurrency.Equals(CurrencyDto.UYU.ToString(), StringComparison.InvariantCultureIgnoreCase);
            var isBillCurrencyDollars = billCurrency.Equals(CurrencyDto.USD.ToString(), StringComparison.InvariantCultureIgnoreCase);

            if (isBillInLocalCurrency)
            {
                isBillEligibleForDiscount = totalAmount <= (discountDto.MaximumAmount * uiExchangeRate.ValueInPesos);
            }
            else if (isBillCurrencyDollars)
            {
                var dollarExchangeRate = _exchangeRateService.GetQuotationForDate(DateTime.Now.Date, CurrencyDto.USD);
                if (dollarExchangeRate == null)
                {
                    throw new Exception();
                }

                isBillEligibleForDiscount = totalAmount * dollarExchangeRate.ValueInPesos <= (discountDto.MaximumAmount * uiExchangeRate.ValueInPesos);
            }
            return isBillEligibleForDiscount;
        }

        private DiscountDto GetDiscountForBinService(BinDto activeBin, Guid serviceId)
        {
            var service = _serviceService.GetById(serviceId);
            var discountDto = _discountService.GetDiscount(DateTime.Now.Date, activeBin.CardType, service.DiscountType);
            if (discountDto == null)
            {
                //NLogLogger.LogEvent(NLogType.Error, "Calculo de descuento - DISCOUNT_NO_DISCOUNT_FOR_CARDTYPE__INTERNAL");
                //this._loggerService.CreateLog(
                //    LogType.Error,
                //    LogOperationType.DiscountCalculation,
                //    LogCommunicationType.VisaNet,
                //    string.Empty,
                //    string.Format(
                //        ExceptionMessages.DISCOUNT_NO_DISCOUNT_FOR_CARDTYPE__INTERNAL,
                //        DateTime.Now.ToString("d"),
                //        bin.Value));

                throw new FatalException(
                    CodeExceptions.GENERAL_ERROR,
                    CodeExceptions.DISCOUNT_NO_DISCOUNT_FOR_CARDTYPE__INTERNAL,
                    DateTime.Now.Date.ToShortDateString(),
                    EnumHelpers.GetName(typeof(CardTypeDto), (int)activeBin.CardType, EnumsStrings.ResourceManager));
            }
            return discountDto;
        }

        private QuotationDto GetTodaysUiExchangeRate()
        {
            var uiExchangeRate = _exchangeRateService.GetQuotationForDate(DateTime.Now.Date, CurrencyDto.UI);
            if (uiExchangeRate == null)
            {
                throw new FatalException(CodeExceptions.GENERAL_ERROR, CodeExceptions.QUOTATION_NO_CONFIGURED_FOR_UI__INTERNAL, DateTime.Now.Date.ToString());
            }
            return uiExchangeRate;
        }

        private DiscountCalculationDto Calculate(BillDto bill, BinDto bin, DiscountDto discountDto)
        {
            if (bill.TaxedAmount < 0)
            {
                throw new BillTaxedAmountNotAllow(CodeExceptions.BILL_TAXED_AMOUNT_NOT_ALLOW, new[] { string.Empty, bill.TaxedAmount.ToString() });
            }

            var uiExchangeRate = GetTodaysUiExchangeRate();
            var discountCalculation = new DiscountCalculationDto();

            //Si el indicador de ley es 0 => no aplica descuento
            if (!bill.IsFinalConsumer || bill.LawId == 0)
            {
                discountCalculation.DiscountAmount = 0;
                discountCalculation.NetAmount = bill.Amount;

                return discountCalculation;
            }

            var billDiscount = GetBillDiscount(Convert(bill), uiExchangeRate, discountDto);

            // Calculo y registro el descuento aplicado y el monto resultante
            var discountAmount = ((bill.TaxedAmount * billDiscount) / 100).SignificantDigits(2);

            var authorizationAmountTypeDto = bin.BinAuthorizationAmountTypeDtoList.First(x => x.LawDto == discountDto.DiscountType);

            var cyberSourceTotal = (authorizationAmountTypeDto.AuthorizationAmountTypeDto == AuthorizationAmountTypeDto.Gross) ? bill.Amount : (bill.Amount - discountAmount);

            discountCalculation.NetAmount = cyberSourceTotal;
            discountCalculation.DiscountAmount = discountAmount;
            discountCalculation.Discount = billDiscount;
            return discountCalculation;
        }

        private Bill Convert(BillDto billDto)
        {
            return new Bill
            {
                Amount = billDto.Amount,
                TaxedAmount = billDto.TaxedAmount,
                Currency = billDto.Currency,
                //LawId = billDto.LawId
            };
        }

        #endregion

    }
}