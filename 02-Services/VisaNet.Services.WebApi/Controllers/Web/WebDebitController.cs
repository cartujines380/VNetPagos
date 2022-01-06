using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.CustomerSite.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebDebitController : ApiController
    {
        private readonly IServiceCustomerSite _serviceCustomerSite;
        private readonly IServiceDebitRequest _serviceDebitRequest;

        public WebDebitController(IServiceCustomerSite serviceCustomerSite, IServiceDebitRequest serviceDebitRequest)
        {
            _serviceCustomerSite = serviceCustomerSite;
            _serviceDebitRequest = serviceDebitRequest;
        }

        [HttpGet]
        public HttpResponseMessage GetCommercesDebit([FromUri] CustomerSiteCommerceFilterDto filter)
        {
            var decoded = filter != null ? HttpUtility.UrlDecode(filter.Name) : null;
            var debitsCommerces = _serviceCustomerSite.GetCommercesDebit(null, decoded);
            return Request.CreateResponse(HttpStatusCode.OK, debitsCommerces);
        }

        [HttpGet]
        public HttpResponseMessage FindCommerceDebit(Guid id)
        {
            var debitsCommerces = _serviceCustomerSite.FindCommerceDebit(id);
            return Request.CreateResponse(HttpStatusCode.OK, debitsCommerces);
        }

        [HttpGet]
        public HttpResponseMessage GetDebitRequestByUserId(Guid userId)
        {
            var debitRequests = _serviceDebitRequest.GetByUserId(userId);
            return Request.CreateResponse(HttpStatusCode.OK, debitRequests);
        }

        [HttpPost]
        public HttpResponseMessage GetDataForFromList([FromBody] DebitRequestsFilterDto filterDto)
        {
            var requests = _serviceDebitRequest.GetDataForFromList(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, requests);
        }

        [HttpGet]
        public HttpResponseMessage ValidateCardType(int binValue)
        {
            var requests = _serviceDebitRequest.ValidateCardType(binValue);
            return Request.CreateResponse(HttpStatusCode.OK, requests);
        }

        [HttpPut]
        public HttpResponseMessage Create([FromBody] DebitRequestDto dto)
        {
            var requests = _serviceDebitRequest.Create(dto, true);
            return Request.CreateResponse(HttpStatusCode.OK, requests);
        }

        [HttpPost]
        public HttpResponseMessage CancelDebitRequest([FromBody] Guid id)
        {
            var requests = _serviceDebitRequest.CancelDebitRequest(id);
            return Request.CreateResponse(HttpStatusCode.OK, requests);
        }

        [HttpPut]
        public HttpResponseMessage ProccesDataFromCybersource(IDictionary<string, string> csDictionary)
        {
            var result = _serviceDebitRequest.ProccesDataFromCybersource(csDictionary);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

    }
}