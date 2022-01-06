using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Application.VisaNetOn.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebVisaNetOnIntegrationController : ApiController
    {
        private readonly IServiceVisaNetOnIntegration _serviceVisaNetOnIntegration;
        private readonly IServiceVonData _serviceVonData;

        public WebVisaNetOnIntegrationController(IServiceVisaNetOnIntegration serviceVisaNetOnIntegration, IServiceVonData serviceVonData)
        {
            _serviceVisaNetOnIntegration = serviceVisaNetOnIntegration;
            _serviceVonData = serviceVonData;
        }

        [HttpPost]
        public HttpResponseMessage ProcessOperation(ProcessOperationDto processOperationDto)
        {
            var result = _serviceVisaNetOnIntegration.ProcessOperation(processOperationDto.FormData, processOperationDto.Action);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage FindVonData(string idApp, string idUserExternal, string idCardExternal)
        {
            var result = _serviceVonData.Find(idApp, idUserExternal, idCardExternal);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage FindVonData(string idApp, Guid anonymousUserId, string idCardExternal)
        {
            var result = _serviceVonData.Find(idApp, anonymousUserId, idCardExternal);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage FindVonData(string idApp, string idUserExternal)
        {
            var result = _serviceVonData.Find(idApp, idUserExternal);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage FindVonData(string idApp, Guid anonymousUserId)
        {
            var result = _serviceVonData.Find(idApp, anonymousUserId);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage DownloadTicket([FromUri] string transactionNumber, [FromUri] Guid userId)
        {
            byte[] arrBytes;
            string mimeType;

            //CONTROL A ESTE NIVEL PORQUE EN EL SERVICIO SI PUEDE SER VACIO. LLAMADAS INTERNAS
            if (userId == Guid.Empty || userId == null)
                throw new FatalException(CodeExceptions.USER_NOT_EXIST);

            _serviceVisaNetOnIntegration.GeneratePaymentTicket(transactionNumber, userId, out arrBytes, out mimeType);
            return Request.CreateResponse(HttpStatusCode.OK, arrBytes);
        }

        [HttpGet]
        public HttpResponseMessage SendPaymentTicketByEmail([FromUri] string transactionNumber, [FromUri] Guid userId)
        {
            //CONTROL A ESTE NIVEL PORQUE EN EL SERVICIO SI PUEDE SER VACIO. LLAMADAS INTERNAS
            if (userId == Guid.Empty || userId == null)
                throw new FatalException(CodeExceptions.USER_NOT_EXIST);

            _serviceVisaNetOnIntegration.SendPaymentTicketByEmail(transactionNumber, userId);
            return Request.CreateResponse(HttpStatusCode.OK, "Ok");
        }

        [HttpGet]
        public HttpResponseMessage GetPaymentDto([FromUri] string transactionNumber, [FromUri] string idApp)
        {
            var dto = _serviceVisaNetOnIntegration.GetPaymentDto(transactionNumber, idApp);
            if (dto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(dto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpPost]
        public HttpResponseMessage CancelPayment([FromBody] string transactionNumber)
        {
            var canceled = _serviceVisaNetOnIntegration.CancelPayment(transactionNumber);
            return Request.CreateResponse(HttpStatusCode.OK, canceled);
        }

    }
}