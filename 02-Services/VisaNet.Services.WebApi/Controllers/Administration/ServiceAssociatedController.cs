using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class ServiceAssociatedController : ApiController
    {
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;

        public ServiceAssociatedController(IServiceServiceAssosiate serviceServiceAssosiate)
        {
            _serviceServiceAssosiate = serviceServiceAssosiate;
        }

        //[HttpGet]
        //public HttpResponseMessage Get([FromUri] ReportsServicesAssociatedFilterDto filterDto)
        //{
        //    var services = _serviceServiceAssosiate.ReportsServicesAssociatedData(filterDto);
        //    return Request.CreateResponse(HttpStatusCode.OK, services);
        //}

        [HttpGet]
        public HttpResponseMessage ReportsServicesAssociatedDataFromDbView([FromUri] ReportsServicesAssociatedFilterDto filterDto)
        {
            var services = _serviceServiceAssosiate.ReportsServicesAssociatedDataFromDbView(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        public HttpResponseMessage ReportsServicesAssociatedDataCount([FromUri] ReportsServicesAssociatedFilterDto filterDto)
        {
            var services = _serviceServiceAssosiate.ReportsServicesAssociatedDataCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        public HttpResponseMessage ReportsAutomaticPaymentsDataFromDbView([FromUri] ReportsAutomaticPaymentsFilterDto filterDto)
        {
            var services = _serviceServiceAssosiate.ReportsAutomaticPaymentsDataFromDbView(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        public HttpResponseMessage ReportsAutomaticPaymentsDataCount([FromUri] ReportsAutomaticPaymentsFilterDto filterDto)
        {
            var services = _serviceServiceAssosiate.ReportsAutomaticPaymentsDataCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        public HttpResponseMessage Get(Guid id)
        {
            var cat = _serviceServiceAssosiate.GetById(id, s => s.Service, s => s.RegisteredUser, s => s.NotificationConfig, s => s.AutomaticPayment, s => s.DefaultCard);
            return Request.CreateResponse(HttpStatusCode.OK, cat);
        }

        public HttpResponseMessage GetByServiceId(Guid id)
        {
            var cat = _serviceServiceAssosiate.GetById(id, s => s.Service, s => s.RegisteredUser, s => s.NotificationConfig, s => s.AutomaticPayment, s => s.DefaultCard);
            return Request.CreateResponse(HttpStatusCode.OK, cat);
        }

    }
}