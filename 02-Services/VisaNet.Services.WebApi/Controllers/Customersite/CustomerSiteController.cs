using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.CustomerSite.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;

namespace VisaNet.Services.WebApi.Controllers.Customersite
{
    public class CustomerSiteController : ApiController
    {
        private readonly IServiceCustomerSite _serviceCustomerSite;

        public CustomerSiteController(IServiceCustomerSite serviceCustomerSite)
        {
            _serviceCustomerSite = serviceCustomerSite;
        }

        [HttpPost]
        public HttpResponseMessage SendAccessTokenByMail([FromBody] CustomerSiteGenerateAccessTokenDto dto)
        {
            var ent = _serviceCustomerSite.SendAccessTokenByMail(dto);
            return Request.CreateResponse(HttpStatusCode.OK, ent);
        }

        [HttpPost]
        public HttpResponseMessage SendAccessTokenBySms([FromBody] CustomerSiteGenerateAccessTokenDto dto)
        {
            var ent = _serviceCustomerSite.SendAccessTokenBySms(dto);
            return Request.CreateResponse(HttpStatusCode.OK, ent);
        }

        [HttpPost]
        public HttpResponseMessage SendAccessTokenByWhatsapp([FromBody] CustomerSiteGenerateAccessTokenDto dto)
        {
            var ent = _serviceCustomerSite.SendAccessTokenByWhatsapp(dto);
            return Request.CreateResponse(HttpStatusCode.OK, ent);
        }

        [HttpPost]
        public HttpResponseMessage GenerateAccessToken([FromBody] CustomerSiteGenerateAccessTokenDto dto)
        {
            var ent = _serviceCustomerSite.GenerateAccessToken(dto);
            return Request.CreateResponse(HttpStatusCode.OK, ent);
        }

        [HttpPost]
        public HttpResponseMessage ResetPasswordEmail([FromBody] ResetPasswordEmailDto dto)
        {
            _serviceCustomerSite.ResetPasswordEmail(dto);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage NewUserEmail([FromBody] NewUserEmailDto dto)
        {
            _serviceCustomerSite.NewUserEmail(dto);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage CancelAccessToken([FromBody] WebhookAccessTokenDto dto)
        {
            var canceled = _serviceCustomerSite.CancelAccessToken(dto.Id);
            return Request.CreateResponse(HttpStatusCode.OK,
                canceled ?
                new WebhookAccessTokenDto() { Id = dto.Id, StateDto = WebhookAccessStateDto.Cancelled } :
                null
                );
        }

        [HttpPost]
        public HttpResponseMessage CancelTransaction([FromBody] CustomerSiteCancelTransactionDto dto)
        {
            var canceled = _serviceCustomerSite.CancelTansaction(dto);
            return Request.CreateResponse(HttpStatusCode.OK, canceled);
        }
        [HttpGet]
        public HttpResponseMessage GetCommercesDebit(CustomerSiteCommerceFilterDto filterDto)
        {
            var debitsCommerces = _serviceCustomerSite.GetCommercesDebitFromCustomerSite(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, debitsCommerces);
        }

        [HttpGet]
        public HttpResponseMessage UpdateCommerceDebitCatche()
        {
            _serviceCustomerSite.UpdateCommerceDebitCatche();
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}