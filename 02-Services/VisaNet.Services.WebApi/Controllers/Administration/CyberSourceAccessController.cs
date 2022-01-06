using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using VisaNet.Components.CyberSource.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Services.WebApi.Models;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class CyberSourceAccessController : ApiController
    {
        private readonly ICybersourceAccessFacade _cyberSourceAccessFacade;
        private readonly ICyberSourceAccess _cyberSourceAccess;

        public CyberSourceAccessController(ICybersourceAccessFacade cyberSourceAccessFacade, ICyberSourceAccess cyberSourceAccess)
        {
            _cyberSourceAccessFacade = cyberSourceAccessFacade;
            _cyberSourceAccess = cyberSourceAccess;
        }

        [HttpPost]
        public HttpResponseMessage GenerateKeys([ModelBinder(typeof(CustomModelBinder))] IGenerateToken item)
        {
            var keys = _cyberSourceAccessFacade.GenerateKeys(item);
            return Request.CreateResponse(HttpStatusCode.OK, keys);
        }

        [HttpGet]
        public HttpResponseMessage GetCardNumberByToken([FromUri] CybersourceGetCardNameDto dto)
        {
            try
            {
                var cardNumber = _cyberSourceAccess.GetCardNumberByToken(dto);
                return Request.CreateResponse(HttpStatusCode.OK, cardNumber);
            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, string.Empty);
            }
        }
    }
}