using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebServiceController : ApiController
    {
        private readonly IServiceService _serviceService;

        public WebServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        public HttpResponseMessage Get([FromUri] ServiceFilterDto filterDto)
        {
            //Obtengo solo los servicios activos
            var services = _serviceService.GetDataForTable(filterDto).Where(s => s.Active);
            foreach (var serviceDto in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(serviceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        public HttpResponseMessage Get(Guid id)
        {
            var serv = _serviceService.All(null, s => s.Id == id, s => s.ServiceContainer, s => s.ServiceCategory, s => s.ServiceGateways, s => s.ServiceGateways.Select(g => g.Gateway)).FirstOrDefault();
            WebControllerHelper.LoadServiceReferenceParams(serv);
            return Request.CreateResponse(HttpStatusCode.OK, serv);
        }

        [HttpGet]
        public HttpResponseMessage GetServiceByUrlName(string nameUrl)
        {
            nameUrl = nameUrl.ToLower();
            var serv = _serviceService.All(null, s => s.UrlName.Equals(nameUrl), s => s.ServiceContainer, s => s.ServiceCategory, s => s.ServiceGateways, s => s.ServiceGateways.Select(g => g.Gateway)).FirstOrDefault();
            WebControllerHelper.LoadServiceReferenceParams(serv);
            return Request.CreateResponse(HttpStatusCode.OK, serv);
        }

        [HttpGet]
        public HttpResponseMessage GetEnableCards(Guid userId, Guid serviceId)
        {
            var cards = _serviceService.GetEnableCards(userId, serviceId);
            return Request.CreateResponse(HttpStatusCode.OK, cards);
        }

        [HttpGet]
        public HttpResponseMessage GetCertificateName(string codcommerce, string codBranch)
        {
            var name = _serviceService.GetCertificateName(codcommerce, codBranch);
            return Request.CreateResponse(HttpStatusCode.OK, name);
        }

        [HttpGet]
        public HttpResponseMessage GetServicesProvider(Guid? serviceId = null)
        {
            //Obtengo solo los servicios activos
            var services = _serviceService.GetDataForTable(new ServiceFilterDto()
            {
                ServiceId = serviceId ?? Guid.Empty,
                Active = true,
                WithoutServiceInContainer = true,
            });
            foreach (var serviceDto in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(serviceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        public HttpResponseMessage GetServicesFromContainer()
        {
            //Obtengo solo los servicios activos
            var services = _serviceService.GetDataForTable(new ServiceFilterDto()
            {
                Active = true,
            });
            foreach (var serviceDto in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(serviceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }


        #region list for web and apps
        [HttpGet]
        public HttpResponseMessage GetServicesPaymentPrivate()
        {
            //Obtengo solo los servicios activos
            var services = _serviceService.GetDataForTable(new ServiceFilterDto()
            {
                Active = true,
                EnablePrivatePayment = true,
                WithoutServiceInContainer = true,
            });
            foreach (var serviceDto in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(serviceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }
        [HttpGet]
        public HttpResponseMessage GetServicesPaymentPublic()
        {
            //Obtengo solo los servicios activos
            var services = _serviceService.GetDataForTable(new ServiceFilterDto()
            {
                Active = true,
                EnablePublicPayment = true,
                WithoutServiceInContainer = true,
            });
            foreach (var serviceDto in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(serviceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }
        [HttpGet]
        public HttpResponseMessage GetServicesEnableAssociation()
        {
            //Obtengo solo los servicios activos
            var services = _serviceService.GetDataForTable(new ServiceFilterDto()
            {
                Active = true,
                EnableAssociation = true,
                WithoutServiceInContainer = true,
            });
            foreach (var serviceDto in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(serviceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        public HttpResponseMessage GetServiceByIdForApps([FromUri] Guid serviceId)
        {
            //Obtengo solo los servicios activos
            var services = _serviceService.GetServicesForApp(new ServiceFilterDto()
            {
                Active = true,
                ServiceId = serviceId,
            });
            foreach (var serviceDto in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(serviceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services.FirstOrDefault());
        }

        [HttpGet]
        public HttpResponseMessage GetServicesPaymentPrivateForApps()
        {
            //Obtengo solo los servicios activos
            var services = _serviceService.GetServicesForApp(new ServiceFilterDto()
            {
                Active = true,
                EnablePrivatePayment = true,
            });
            foreach (var serviceDto in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(serviceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        public HttpResponseMessage GetServicesPaymentPublicForApps()
        {
            //Obtengo solo los servicios activos
            var services = _serviceService.GetServicesForApp(new ServiceFilterDto()
            {
                Active = true,
                EnablePublicPayment = true,
            });
            foreach (var serviceDto in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(serviceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }
        [HttpGet]
        public HttpResponseMessage GetServicesEnableAssociationForApps()
        {
            //Obtengo solo los servicios activos
            var services = _serviceService.GetServicesForApp(new ServiceFilterDto()
            {
                Active = true,
                EnableAssociation = true,
            });
            foreach (var serviceDto in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(serviceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }
        #endregion

        [HttpGet]
        public HttpResponseMessage GetGateways()
        {
            //Obtengo solo los servicios activos
            var gateways = _serviceService.GetGateways();
            return Request.CreateResponse(HttpStatusCode.OK, gateways);
        }

        [HttpGet]
        public HttpResponseMessage GetCertificateThumbprintIdApp(string friendlyname)
        {
            var thumbprint = _serviceService.GetCertificateNameIdApp(friendlyname);
            return Request.CreateResponse(HttpStatusCode.OK, thumbprint);
        }

        [HttpGet]
        public HttpResponseMessage IsBinAssociatedToService(int binValue, Guid id)
        {
            var isAssociated = _serviceService.IsBinAssociatedToService(binValue, id);
            return Request.CreateResponse(HttpStatusCode.OK, isAssociated);
        }

        [HttpGet]
        public HttpResponseMessage GetServicesFromMerchand(string idApp, string merchandId, GatewayEnumDto gateway)
        {
            var services = _serviceService.GetServicesFromMerchand(idApp, merchandId, gateway);
            foreach (var serviceDto in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(serviceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        //[HttpGet]
        //public HttpResponseMessage FindAndValidateServiceForVisaNetOnAssociation(string idApp)
        //{
        //    var result = _serviceService.FindAndValidateServiceForVisaNetOnAssociation(idApp);
        //    return Request.CreateResponse(HttpStatusCode.OK, result);
        //}

        //[HttpGet]
        //public HttpResponseMessage FindAndValidateServiceForVisaNetOnPayment(string idApp, string merchantId)
        //{
        //    var result = _serviceService.FindAndValidateServiceForVisaNetOnPayment(idApp, merchantId);
        //    return Request.CreateResponse(HttpStatusCode.OK, result);
        //}

    }
}