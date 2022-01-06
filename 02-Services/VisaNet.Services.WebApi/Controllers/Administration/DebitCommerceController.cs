using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.CustomerSite.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class DebitCommerceController : ApiController
    {
        private readonly IServiceCustomerSite _serviceCustomerSite;
        private readonly IServiceDebitRequest _serviceDebitRequest;

        public DebitCommerceController(IServiceCustomerSite serviceCustomerSite, IServiceDebitRequest serviceDebitRequest)
        {
            _serviceCustomerSite = serviceCustomerSite;
            _serviceDebitRequest = serviceDebitRequest;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.DebitCommerceEdit)]
        public HttpResponseMessage UpdateCommerceDebitCatche()
        {
            _serviceCustomerSite.UpdateCommerceDebitCatche();
            return Request.CreateResponse(HttpStatusCode.OK, "ok");
        }

        [HttpGet]
        [WebApiAuthentication(Actions.DebitSuscriptionList)]
        public HttpResponseMessage GetDebitSuscriptionListCount([FromUri] DebitRequestsFilterDto filters)
        {
            var list = _serviceDebitRequest.GetDebitSuscriptionListCount(filters);
            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.DebitSuscriptionList)]
        public HttpResponseMessage GetDebitSuscriptionList([FromUri] DebitRequestsFilterDto filters)
        {
            var list = _serviceDebitRequest.GetDebitSuscriptionList(filters);
            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.DebitSuscriptionDetails)]
        public HttpResponseMessage FindSuscription([FromUri] Guid id)
        {
            var list = _serviceDebitRequest.GetById(id, x => x.Card, x => x.References, x => x.User);
            return Request.CreateResponse(HttpStatusCode.OK, list);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.DebitCommerceList)]
        public HttpResponseMessage FindCommerce([FromUri] int debitProductId)
        {
            var list = _serviceCustomerSite.GetCommercesDebit(new List<int>() { debitProductId });
            return Request.CreateResponse(HttpStatusCode.OK, list.FirstOrDefault());
        }

        [HttpPost]
        [WebApiAuthentication(Actions.AuditList)]
        public HttpResponseMessage ExcelExportManualSynchronization()
        {
            var t = _serviceDebitRequest.ExcelExportManualSynchronization();
            return Request.CreateResponse(HttpStatusCode.OK, t);
        }
    }
}