using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Services.WebApi.Controllers.Web;

namespace VisaNet.Services.WebApi.Controllers.WebService
{
    public class WebServiceExternalAppController : ApiController
    {
        private readonly IServiceHighway _serviceHighway;
        private readonly IServiceApplicationUser _serviceApplicationUser;
        private readonly IServiceService _serviceService;
        private readonly IServiceIntegration _serviceIntegration;

        public WebServiceExternalAppController(IServiceHighway serviceHighway, IServiceApplicationUser serviceApplicationUser,
            IServiceService serviceService, IServiceIntegration serviceIntegration)
        {
            _serviceHighway = serviceHighway;
            _serviceApplicationUser = serviceApplicationUser;
            _serviceService = serviceService;
            _serviceIntegration = serviceIntegration;
        }

        [HttpPut]
        public HttpResponseMessage AddNewBills([FromBody] WebServiceBillsInputDto entityfilter)
        {
            if (entityfilter != null)
            {
                var result = _serviceHighway.ProccesBillsFromWcf(entityfilter);
                if (result != null)
                {
                    foreach (var highwayBillDto in result)
                    {
                        WebControllerHelper.LoadServiceReferenceParams(highwayBillDto.ServiceDto);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "NO");
        }

        [HttpPut]
        public HttpResponseMessage DeleteBills([FromBody] WebServiceBillsDeleteDto billsfilter)
        {
            var result = _serviceHighway.DeleteBills(billsfilter);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        public HttpResponseMessage AssosiatedServiceClientUpdate([FromBody] WebServiceClientInputDto clientfilter)
        {
            var result = _serviceApplicationUser.AssosiatedServiceClientUpdate(clientfilter);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        public HttpResponseMessage BillsStatus([FromBody] WsBillQueryDto billsStatus)
        {
            //var result = _servicePayment.StatusBIlls(billsStatus);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPut]
        public HttpResponseMessage GetCertificateThumbprint([FromBody] WebServiceClientInputDto clientfilter)
        {
            var service = _serviceService.GetCertificateName(clientfilter.CodCommerce.ToString(), clientfilter.CodBranch.ToString());
            return Request.CreateResponse(HttpStatusCode.OK, service);
        }

        [HttpPut]
        public HttpResponseMessage GetCertificateThumbprintIdApp([FromBody] WebServiceClientInputDto clientfilter)
        {
            var service = _serviceService.GetCertificateNameIdApp(clientfilter.IdApp);
            return Request.CreateResponse(HttpStatusCode.OK, service);
        }

        [HttpPut]
        public HttpResponseMessage MakePayment([FromBody] WsBillPaymentOnlineDto dto)
        {
            var result = _serviceIntegration.MakePayment(dto);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        public HttpResponseMessage CancelPayment([FromBody] WsPaymentCancellationDto dto)
        {
            var result = _serviceIntegration.CancelPayment(dto);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        public HttpResponseMessage TransactionsHistory([FromBody] WsBillQueryDto billsStatus)
        {
            var result = _serviceIntegration.TransactionsHistory(billsStatus);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        public HttpResponseMessage GetServices([FromBody] WsCommerceQueryDto dto)
        {
            var result = _serviceIntegration.GetServices(dto);
            if (result != null && result.Commerces != null)
            {
                foreach (var commerce in result.Commerces)
                {
                    WebControllerHelper.LoadServiceReferenceParams(commerce);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        public HttpResponseMessage RemoveCard([FromBody] WsCardRemoveDto dto)
        {
            var result = _serviceIntegration.RemoveCard(dto);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        public HttpResponseMessage GetUrlTransacctionPosts([FromBody] WsUrlTransactionQueryDto dto)
        {
            var result = _serviceIntegration.GetUrlTransacctionPosts(dto);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

    }
}