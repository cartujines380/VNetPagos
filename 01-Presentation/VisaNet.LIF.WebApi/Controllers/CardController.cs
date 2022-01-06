using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using VisaNet.Common.Logging.NLog;
using VisaNet.LIF.WebApi.ActionFilters;
using VisaNet.LIF.WebApi.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.LIF;

namespace VisaNet.LIF.WebApi.Controllers
{
    [RoutePrefix("api/v1/card")]
    public class CardController : BaseApiController
    {
        private readonly ICardClientService _cardService;

        public CardController(ICardClientService cardService)
        {
            _cardService = cardService;
        }

        /// <summary>
        /// Método para obtener la información de una tarjeta
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("data")]
        [LIFAuthorize(ModelType = typeof(CardDataInModel))]
        public async Task<HttpResponseMessage> CardData([FromBody]CardDataInModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
                var includeIssuingCompany = ConfigurationManager.AppSettings["IncludeIssuingCompany"].Split(';').Contains(model.AppId);
                var bin = await _cardService.GetCardInfo(model.Bin, includeIssuingCompany);
                return CreateHttpResponseMessage(HttpStatusCode.OK, new CardDataOutModel
                {
                    Bin = model.Bin,
                    CardType = bin.CardType,
                    IsLocal = bin.National,
                    Issuer = bin.IssuingCompany,
                    Installments = bin.Installments
                });
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
                NLogLogger.LogEvent(NLogType.Info, "CardController - CardData - Exception");
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, "Ha ocurrido un error inesperado.");
            }
        }

        /// <summary>
        /// Método para obtener el listado de las tarjetas nacionales
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("nationalData")]
        [LIFAuthorize(ModelType = typeof(NationalDataInModel))]
        public async Task<HttpResponseMessage> NationalData([FromBody]NationalDataInModel model)
        {
            if (model == null || !ModelState.IsValid)
            {
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
                var includeIssuingCompany = ConfigurationManager.AppSettings["IncludeIssuingCompany"].Split(';').Contains(model.AppId);
                var returnValue = (await _cardService.GetNationalBins(includeIssuingCompany)).Select(x => new CardDataOutModel
                {
                    Bin = x.Value,
                    CardType = x.CardType,
                    IsLocal = x.National,
                    Issuer = x.IssuingCompany,
                    Installments = x.Installments
                });
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
                NLogLogger.LogEvent(NLogType.Info, "CardController - NationalData - Exception");
                return CreateHttpResponseMessage(HttpStatusCode.BadRequest, "Ha ocurrido un error inesperado.");
            }
        }

    }
}