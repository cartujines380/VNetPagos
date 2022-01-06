using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Services.WebApi.Controllers.Web;

namespace VisaNet.Services.WebApi.Controllers.WebService
{
    public class WebServiceBankController : ApiController
    {
        private readonly IServiceWsBank _serviceWsBank;

        public WebServiceBankController(IServiceWsBank serviceWsBank)
        {
            _serviceWsBank = serviceWsBank;
        }

        [HttpPut]
        public HttpResponseMessage AllServices()
        {
            var result = _serviceWsBank.AllServices();
            if (result != null)
            {
                foreach (var serviceDto in result)
                {
                    WebControllerHelper.LoadServiceReferenceParams(serviceDto);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        public HttpResponseMessage GetBills(WsBankBillsInputDto dto)
        {
            var result = _serviceWsBank.GetBills(dto);
            if (result != null && result.Bills != null)
            {
                foreach (var bill in result.Bills)
                {
                    if (bill.Payment != null)
                    {
                        WebControllerHelper.LoadServiceReferenceParams(bill.Payment.ServiceDto);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        public HttpResponseMessage Payment(WsBankPaymentInputDto dto)
        {
            var result = _serviceWsBank.Payment(dto);
            if (result != null && result.Payment != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(result.Payment.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        public HttpResponseMessage ReversePayment(WsBankReverseInputDto dto)
        {
            var result = _serviceWsBank.ReversePayment(dto);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        public HttpResponseMessage PreprocessPayment(WsBankPreprocessPaymentInputDto dto)
        {
            var result = _serviceWsBank.PreprocessPayment(dto);
            if (result != null && result.Bills != null)
            {
                foreach (var bill in result.Bills)
                {
                    if (bill.Payment != null)
                    {
                        WebControllerHelper.LoadServiceReferenceParams(bill.Payment.ServiceDto);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        public HttpResponseMessage CalculateDiscount(WsBankPreprocessPaymentInputDto dto)
        {
            var result = _serviceWsBank.CalculateDiscount(dto);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        public HttpResponseMessage GetPayments(WsBankSearchPaymentsInputDto dto)
        {
            var result = _serviceWsBank.GetPayments(dto);
            if (result != null)
            {
                foreach (var payment in result)
                {
                    WebControllerHelper.LoadServiceReferenceParams(payment.ServiceDto);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

    }
}