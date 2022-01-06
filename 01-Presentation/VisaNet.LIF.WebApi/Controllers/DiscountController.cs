using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.LIF;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.LIF.WebApi.ActionFilters;
using VisaNet.LIF.WebApi.Mappers;
using VisaNet.LIF.WebApi.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.LIF;

namespace VisaNet.LIF.WebApi.Controllers
{
    [RoutePrefix("api/v1/discount")]
    public class DiscountController : BaseApiController
    {
        private readonly IDiscountClientService _discountService;
        private readonly ICardClientService _cardService;
        private readonly ILifApiBillClientService _serviceLifApiBill;

        public DiscountController(IDiscountClientService discountService, ICardClientService cardService, ILifApiBillClientService serviceLifApiBill)
        {
            _discountService = discountService;
            _cardService = cardService;
            _serviceLifApiBill = serviceLifApiBill;
        }

        /// <summary>
        /// Método para calcular el descuento de una factura
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("app")]
        [LIFAuthorize(ModelType = typeof(DiscountCalculationAppInModel))]
        public async Task<HttpResponseMessage> DiscountCalculationApp([FromBody]DiscountCalculationAppInModel model)
        {
            var lifApiBill = new LifApiBillDto();

            if (model == null || !ModelState.IsValid)
            {
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, ModelState);
            }

            if (model.Bill == null)
            {
                ModelState.AddModelError("Bill", LIFStrings.Bill_Required);
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
                lifApiBill.BinValue = model.Bin;
                lifApiBill.AppId = model.AppId;
                lifApiBill.OperationId = model.OperationId;
                lifApiBill.Amount = model.Bill.Amount;
                lifApiBill.TaxedAmount = model.Bill.TaxedAmount;
                lifApiBill.IsFinalConsumer = model.Bill.IsFinalConsumer;
                lifApiBill.Currency = model.Bill.Currency;
                lifApiBill.LawId = model.Bill.LawId;

                NLogLogger.LogEvent(NLogType.Info, "DiscountController - DiscountCalculationApp - Comienza calcular descuento ");
                var discountCalculation = await _discountService.CalculateDiscount(model.Bill.ToDomainObject(), model.Bin);
                NLogLogger.LogEvent(NLogType.Info, "DiscountController - DiscountCalculationApp - Termina calcular descuento ");
                var includeIssuingCompany = ConfigurationManager.AppSettings["IncludeIssuingCompany"].Split(';').Contains(model.AppId);
                var cardData = await _cardService.GetCardInfo(model.Bin, includeIssuingCompany);

                lifApiBill.CardType = cardData.CardType.ToString();
                lifApiBill.DiscountAmount = discountCalculation.DiscountAmount;
                lifApiBill.AmountToCyberSource = discountCalculation.NetAmount;
                lifApiBill.IssuingCompany = cardData.IssuingCompany;

                NLogLogger.LogEvent(NLogType.Info, "DiscountController - DiscountCalculationApp - Comienza crear lif bill");
                await _serviceLifApiBill.Create(lifApiBill);
                NLogLogger.LogEvent(NLogType.Info, "DiscountController - DiscountCalculationApp - Termina crear lif bill");

                var returnValue = DiscountCalculationAppOutModel.Create(cardData.CardType,
                    discountCalculation.DiscountAmount, discountCalculation.NetAmount, cardData.IssuingCompany);

                return CreateHttpResponseMessage(HttpStatusCode.OK, returnValue);
            }
            catch (ArgumentException e)
            {
                lifApiBill.Error = e.ToString();
                _serviceLifApiBill.Create(lifApiBill);
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, e.Message);
            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogEvent(NLogType.Info, "DiscountController - DiscountCalculationApp - BusinessException: " + e.Message);
                lifApiBill.Error = e.Message;
                _serviceLifApiBill.Create(lifApiBill);
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, e.Message);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                NLogLogger.LogEvent(NLogType.Info, "DiscountController - DiscountCalculationApp - Exception");
                lifApiBill.Error = "Ha ocurrido un error inesperado";
                _serviceLifApiBill.Create(lifApiBill);
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, "Ha ocurrido un error inesperado.");
            }
        }

        /// <summary>
        /// Método para calcular el descuento de una factura indicando el servicio
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("service")]
        [LIFAuthorize(ModelType = typeof(DiscountCalculationServiceInModel))]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<HttpResponseMessage> DiscountCalculationService([FromBody]DiscountCalculationServiceInModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, ModelState);
            }

            if (model.Bill == null)
            {
                ModelState.AddModelError("Bill", LIFStrings.Bill_Required);
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
                var discountCalculation = await _discountService.CalculateDiscount(model.Bill.ToDomainObject(), model.Bin, model.ServiceId);
                var includeIssuingCompany = ConfigurationManager.AppSettings["IncludeIssuingCompany"].Split(';').Contains(model.AppId);
                var cardData = await _cardService.GetCardInfo(model.Bin, includeIssuingCompany);
                var returnValue = new DiscountCalculationServiceOutModel
                {
                    CardType = cardData.CardType,
                    DiscountAmount = discountCalculation.DiscountAmount,
                    AmountToCyberSource = discountCalculation.NetAmount,
                    IssuingCompany = cardData.IssuingCompany,
                    Discount = discountCalculation.Discount
                };

                return CreateHttpResponseMessage(HttpStatusCode.OK, returnValue);
            }
            catch (ArgumentException e)
            {
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, e.Message);
            }
            catch (WebApiClientBusinessException e)
            {
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, e.Message);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                NLogLogger.LogEvent(NLogType.Info, "DiscountController - DiscountCalculationService - Exception");
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, "Ha ocurrido un error inesperado.");
            }
        }

    }
}