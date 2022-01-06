using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebCardController : ApiController
    {
        private readonly IServiceCard _serviceCard;
        private readonly IServiceDebitRequest _serviceDebitRequest;

        public WebCardController(IServiceCard serviceCard, IServiceDebitRequest serviceDebitRequest)
        {
            _serviceCard = serviceCard;
            _serviceDebitRequest = serviceDebitRequest;
        }

        [HttpGet]
        public HttpResponseMessage Get([FromUri] CardFilterDto filterDto)
        {
            var cards = _serviceCard.GetDataForTable(filterDto);
            if (cards != null)
            {
                foreach (var card in cards)
                {
                    if (card.ServicesAssociatedDto != null)
                    {
                        foreach (var serviceAssociatedDto in card.ServicesAssociatedDto)
                        {
                            WebControllerHelper.LoadServiceReferenceParams(serviceAssociatedDto.ServiceDto);
                        }
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, cards);
        }

        [HttpGet]
        public HttpResponseMessage Get(Guid id)
        {
            var cat = _serviceCard.GetById(id);
            if (cat != null && cat.ServicesAssociatedDto != null)
            {
                foreach (var serviceAssociatedDto in cat.ServicesAssociatedDto)
                {
                    WebControllerHelper.LoadServiceReferenceParams(serviceAssociatedDto.ServiceDto);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, cat);
        }

        [HttpGet]
        public HttpResponseMessage GetByToken(string token)
        {
            var cat = _serviceCard.GetByToken(token);
            if (cat != null && cat.ServicesAssociatedDto != null)
            {
                foreach (var serviceAssociatedDto in cat.ServicesAssociatedDto)
                {
                    WebControllerHelper.LoadServiceReferenceParams(serviceAssociatedDto.ServiceDto);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, cat);
        }

        [HttpPut]
        public HttpResponseMessage Put([FromBody] CardDto entity)
        {

            var dto = _serviceCard.Create(entity, true);
            if (dto != null && dto.ServicesAssociatedDto != null)
            {
                foreach (var serviceAssociatedDto in dto.ServicesAssociatedDto)
                {
                    WebControllerHelper.LoadServiceReferenceParams(serviceAssociatedDto.ServiceDto);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpPost]
        public HttpResponseMessage Post(Guid id, [FromBody] CardDto dto)
        {
            dto.Id = id;
            _serviceCard.Edit(dto);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        public HttpResponseMessage GenerateExternalId([FromBody] CardOperationDto dto)
        {
            var result = _serviceCard.GenerateExternalId(dto.CardId);
            if (result != null && result.ServicesAssociatedDto != null)
            {
                foreach (var serviceAssociatedDto in result.ServicesAssociatedDto)
                {
                    WebControllerHelper.LoadServiceReferenceParams(serviceAssociatedDto.ServiceDto);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage FindWithServices(Guid cardId)
        {
            var cat = _serviceCard.GetById(cardId, x => x.ServicesAssociated);
            if (cat != null && cat.ServicesAssociatedDto != null)
            {
                foreach (var serviceAssociatedDto in cat.ServicesAssociatedDto)
                {
                    WebControllerHelper.LoadServiceReferenceParams(serviceAssociatedDto.ServiceDto);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, cat);
        }

        [HttpPost]
        public HttpResponseMessage EliminateCard([FromBody] CardOperationDto dto)
        {
            _serviceCard.EliminateCard(dto);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        public HttpResponseMessage ActivateCard([FromBody] CardOperationDto dto)
        {
            _serviceCard.ActivateCard(dto);
            return Request.CreateResponse(HttpStatusCode.OK, "ok");
        }

        [HttpPost]
        public HttpResponseMessage DesactivateCard([FromBody] CardOperationDto dto)
        {
            _serviceCard.DesactivateCard(dto);
            return Request.CreateResponse(HttpStatusCode.OK, "ok");
        }

        [HttpPost]
        public HttpResponseMessage MigrateServices(CardMigrationServicesDto dto)
        {
            _serviceCard.MigrateServices(dto.ApplicationUserId, dto.OldCardId, dto.NewCardId);
            return Request.CreateResponse(HttpStatusCode.OK, "ok");
        }

        [HttpPost]
        public HttpResponseMessage TestMigration(CardMigrationServicesDto dto)
        {
            var testResult = _serviceCard.TestMigration(dto.OldCardId, dto.NewCardId, dto.ApplicationUserId);
            return Request.CreateResponse(HttpStatusCode.OK, testResult);
        }

        [HttpGet]
        public HttpResponseMessage GetAssociatedServices(Guid cardId)
        {
            var associatedServices = _serviceCard.GetAssociatedServices(cardId);
            if (associatedServices != null)
            {
                foreach (var serviceAssociatedDto in associatedServices)
                {
                    WebControllerHelper.LoadServiceReferenceParams(serviceAssociatedDto.ServiceDto);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, associatedServices);
        }

        [HttpGet]
        public HttpResponseMessage GetAssociatedDebits(Guid cardId)
        {
            var associatedServices = _serviceDebitRequest.GetAssociatedDebits(cardId);
            return Request.CreateResponse(HttpStatusCode.OK, associatedServices);
        }

        [HttpGet]
        public HttpResponseMessage GetQuotasForBin(int cardBin)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _serviceCard.GetQuotasForBin(cardBin));
        }

        [HttpGet]
        public HttpResponseMessage GetQuotasForBinAndService(int cardBin, Guid serviceId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _serviceCard.GetQuotasForBinAndService(cardBin, serviceId));
        }

        [HttpPost]
        public HttpResponseMessage EditCardDescription(CardDto dto)
        {
            _serviceCard.EditCardDescription(dto.Id, dto.Description);
            return Request.CreateResponse(HttpStatusCode.OK, "ok");
        }        
    }
}