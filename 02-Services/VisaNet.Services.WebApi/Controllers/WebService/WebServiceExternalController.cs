using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Services.WebApi.Controllers.Web;

namespace VisaNet.Services.WebApi.Controllers.WebService
{
    public class WebServiceExternalController : ApiController
    {
        private readonly IServiceWsBillPaymentOnline _serviceWsBillPaymentOnline;
        private readonly IServiceWsBillQuery _serviceWsBillQuery;
        private readonly IServiceWsCommerceQuery _serviceWsCommerceQuery;
        private readonly IServiceWsPaymentCancellation _serviceWsPaymentCancellation;
        private readonly IServiceWsCardRemove _serviceWsCardRemove;

        public WebServiceExternalController(IServiceWsBillPaymentOnline serviceWsBillPaymentOnline, IServiceWsBillQuery serviceWsBillQuery,
            IServiceWsCommerceQuery serviceWsCommerceQuery, IServiceWsPaymentCancellation serviceWsPaymentCancellation, IServiceWsCardRemove serviceWsCardRemove)
        {
            _serviceWsBillPaymentOnline = serviceWsBillPaymentOnline;
            _serviceWsBillQuery = serviceWsBillQuery;
            _serviceWsCommerceQuery = serviceWsCommerceQuery;
            _serviceWsPaymentCancellation = serviceWsPaymentCancellation;
            _serviceWsCardRemove = serviceWsCardRemove;
        }

        [HttpGet]
        public HttpResponseMessage GetBillQueries()
        {
            var dtos = _serviceWsBillQuery.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpPut]
        public HttpResponseMessage CreateBillQuery([FromBody] WsBillQueryDto entity)
        {
            var dto = _serviceWsBillQuery.Create(entity, true);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpPost]
        public HttpResponseMessage EditBillQuery([FromBody] WsBillQueryDto entity)
        {
            _serviceWsBillQuery.Edit(entity);
            return Request.CreateResponse(HttpStatusCode.OK, entity);
        }

        [HttpGet]
        public HttpResponseMessage GetBillPaymentsOnline()
        {
            var dtos = _serviceWsBillPaymentOnline.GetDataForTable();
            if (dtos != null)
            {
                foreach (var wsBillPaymentOnline in dtos)
                {
                    if (wsBillPaymentOnline.Payment != null)
                    {
                        WebControllerHelper.LoadServiceReferenceParams(wsBillPaymentOnline.Payment.Service);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpPut]
        public HttpResponseMessage CreateBillPaymentOnline([FromBody] WsBillPaymentOnlineDto entity)
        {
            var dto = _serviceWsBillPaymentOnline.Create(entity, true);
            if (dto != null && dto.PaymentDto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(dto.PaymentDto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpPost]
        public HttpResponseMessage EditBillPaymentOnline([FromBody] WsBillPaymentOnlineDto entity)
        {
            _serviceWsBillPaymentOnline.Edit(entity);
            return Request.CreateResponse(HttpStatusCode.OK, entity);
        }

        [HttpGet]
        public HttpResponseMessage BillPaymentOnlineByOperationIdUsed(string idOperation, string idApp)
        {
            var dto = _serviceWsBillPaymentOnline.GetByIdOperation(idOperation, idApp);
            if (dto != null && dto.PaymentDto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(dto.PaymentDto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, dto != null);
        }

        [HttpGet]
        public HttpResponseMessage BillPaymentOnlineByOperationIdUsed(string idOperation)
        {
            var dto = _serviceWsBillPaymentOnline.GetByIdOperation(idOperation, string.Empty);
            if (dto != null && dto.PaymentDto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(dto.PaymentDto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, dto != null);
        }

        [HttpGet]
        public HttpResponseMessage GetCommerceQueries()
        {
            var dtos = _serviceWsCommerceQuery.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpPut]
        public HttpResponseMessage CreateCommerceQuery([FromBody] WsCommerceQueryDto entity)
        {
            var dto = _serviceWsCommerceQuery.Create(entity, true);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpPost]
        public HttpResponseMessage EditCommerceQuery([FromBody] WsCommerceQueryDto entity)
        {
            _serviceWsCommerceQuery.Edit(entity);
            return Request.CreateResponse(HttpStatusCode.OK, entity);
        }

        [HttpGet]
        public HttpResponseMessage GetPaymentCancellations()
        {
            var dtos = _serviceWsPaymentCancellation.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpPut]
        public HttpResponseMessage CreatePaymentCancellation([FromBody] WsPaymentCancellationDto entity)
        {
            var dto = _serviceWsPaymentCancellation.Create(entity, true);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpPost]
        public HttpResponseMessage EditPaymentCancellation([FromBody] WsPaymentCancellationDto entity)
        {
            _serviceWsPaymentCancellation.Edit(entity);
            return Request.CreateResponse(HttpStatusCode.OK, entity);
        }

        [HttpGet]
        public HttpResponseMessage PaymentCancellationsByOperationIdUsed(string idOperation)
        {
            var dto = _serviceWsPaymentCancellation.GetByIdOperation(idOperation);
            return Request.CreateResponse(HttpStatusCode.OK, dto != null);
        }

        [HttpGet]
        public HttpResponseMessage PaymentCancellationsIsOperationIdUsed(string idOperation, string idApp)
        {
            var dto = _serviceWsPaymentCancellation.IsOperationIdRepited(idOperation, idApp);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpGet]
        public HttpResponseMessage BillPaymentOnlineIsOperationIdUsed(string idOperation, string idApp)
        {
            var dto = _serviceWsBillPaymentOnline.IsOperationIdRepited(idOperation, idApp);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpGet]
        public HttpResponseMessage CardRemoveIsOperationIdUsed(string idOperation, string idApp)
        {
            var dto = _serviceWsCardRemove.IsOperationIdRepited(idOperation, idApp);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpPut]
        public HttpResponseMessage CreateCardRemove([FromBody] WsCardRemoveDto entity)
        {
            var dto = _serviceWsCardRemove.Create(entity, true);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpPost]
        public HttpResponseMessage EditCardRemove([FromBody] WsCardRemoveDto entity)
        {
            _serviceWsCardRemove.Edit(entity);
            return Request.CreateResponse(HttpStatusCode.OK, entity);
        }

    }
}