using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebBillController : ApiController
    {
        private readonly IServiceBill _serviceBill;

        public WebBillController(IServiceBill serviceBill)
        {
            _serviceBill = serviceBill;
        }

        [HttpPost]
        public HttpResponseMessage GetBillsForRegisteredUser([FromBody] RegisteredUserBillFilterDto billFilterDto)
        {
            var userBills = _serviceBill.GetBillsForRegisteredUser(billFilterDto);
            if (userBills != null && userBills.Bills != null)
            {
                foreach (var bill in userBills.Bills)
                {
                    if (bill.Payment != null)
                    {
                        WebControllerHelper.LoadServiceReferenceParams(bill.Payment.ServiceDto);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, userBills);
        }

        [HttpPost]
        public HttpResponseMessage GetBillsForAnonymousUser([FromBody] AnonymousUserBillFilterDto billFilterDto)
        {
            var userBills = _serviceBill.GetBillsForAnonymousUser(billFilterDto);
            if (userBills != null && userBills.Bills != null)
            {
                foreach (var bill in userBills.Bills)
                {
                    if (bill.Payment != null)
                    {
                        WebControllerHelper.LoadServiceReferenceParams(bill.Payment.ServiceDto);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, userBills);
        }

        [HttpGet]
        public HttpResponseMessage GetBillsForDashboard(GatewayEnumDto gateway, Guid serviceId, string gatewayReference, string serviceType, string referenceNumber, string referenceNumber2, string referenceNumber3, string referenceNumber4, string referenceNumber5, string referenceNumber6, int serviceDepartament)
        {
            var bills = _serviceBill.GetBillsForDashboard(gateway, serviceId, gatewayReference, serviceType, referenceNumber, referenceNumber2, referenceNumber3, referenceNumber4,
                referenceNumber5, referenceNumber6, serviceDepartament);
            if (bills != null)
            {
                foreach (var bill in bills)
                {
                    if (bill.Payment != null)
                    {
                        WebControllerHelper.LoadServiceReferenceParams(bill.Payment.ServiceDto);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, bills);
        }

        [HttpGet]
        public HttpResponseMessage ChekBills(string lines, int idPadron, int depto, GatewayEnumDto gateway, string param)
        {
            var preBill = _serviceBill.ChekBills(lines, idPadron, depto, gateway, param);
            if (preBill != null && preBill.Payment != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(preBill.Payment.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, preBill);
        }

        [HttpPost]
        public HttpResponseMessage GeneratePreBill(GeneratePreBillDto generatePreBillDto)
        {
            var preBill = _serviceBill.GeneratePreBill(generatePreBillDto);
            if (preBill != null && preBill.Payment != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(preBill.Payment.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, preBill);
        }

        [HttpGet]
        public HttpResponseMessage GetBillsIdPadron(int idPadron, int depto, GatewayEnumDto gateway, string param)
        {
            var preBill = _serviceBill.GetBillsIdPadron(idPadron, depto, gateway, param);
            if (preBill != null)
            {
                foreach (var bill in preBill)
                {
                    if (bill.Payment != null)
                    {
                        WebControllerHelper.LoadServiceReferenceParams(bill.Payment.ServiceDto);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, preBill);
        }

        [HttpPost]
        public HttpResponseMessage CheckAccount([FromBody] RegisteredUserBillFilterDto billFilterDto)
        {
            var result = _serviceBill.CheckAccount(billFilterDto);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage GetInputAmountBillForAnonymousUser([FromBody] AnonymousUserInputAmountBillFilterDto billFilterDto)
        {
            var userBills = _serviceBill.GetInputAmountBillForAnonymousUser(billFilterDto);
            if (userBills != null && userBills.Bills != null)
            {
                foreach (var bill in userBills.Bills)
                {
                    if (bill.Payment != null)
                    {
                        WebControllerHelper.LoadServiceReferenceParams(bill.Payment.ServiceDto);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, userBills);
        }

        [HttpPost]
        public HttpResponseMessage GetInputAmountBillForRegisteredUser([FromBody] RegisteredUserInputAmountBillFilterDto billFilterDto)
        {
            var userBills = _serviceBill.GetInputAmountBillForRegisteredUser(billFilterDto);
            if (userBills != null && userBills.Bills != null)
            {
                foreach (var bill in userBills.Bills)
                {
                    if (bill.Payment != null)
                    {
                        WebControllerHelper.LoadServiceReferenceParams(bill.Payment.ServiceDto);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, userBills);
        }

        [HttpPost]
        public HttpResponseMessage GetInputAmountBill([FromBody] InputAmountBillFilterDto billFilterDto)
        {
            var userBills = _serviceBill.GetInputAmountBill(billFilterDto.Amount, billFilterDto.Currency);
            if (userBills != null && userBills.Payment != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(userBills.Payment.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, userBills);
        }

        [HttpGet]
        public HttpResponseMessage IsBillExlternalIdRepitedByServiceId(string billExternalId, Guid serviceId)
        {
            var isBillExlternalIdRepited = _serviceBill.IsBillExlternalIdRepitedByServiceId(billExternalId, serviceId);
            return Request.CreateResponse(HttpStatusCode.OK, isBillExlternalIdRepited);
        }

        [HttpGet]
        public HttpResponseMessage IsBillExlternalIdRepitedByMerchantId(string billExternalId, string merchantId)
        {
            var isBillExlternalIdRepited = _serviceBill.IsBillExlternalIdRepitedByMerchantId(billExternalId, merchantId);
            return Request.CreateResponse(HttpStatusCode.OK, isBillExlternalIdRepited);
        }

        [HttpPost]
        public HttpResponseMessage GenerateAnnualPatenteForRegisteredUser([FromBody] RegisteredUserBillFilterDto billFilterDto)
        {
            var userBills = _serviceBill.GenerateAnnualPatenteForRegisteredUser(billFilterDto);
            if (userBills != null && userBills.Bills != null)
            {
                foreach (var bill in userBills.Bills)
                {
                    if (bill.Payment != null)
                    {
                        WebControllerHelper.LoadServiceReferenceParams(bill.Payment.ServiceDto);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, userBills);
        }

        [HttpPost]
        public HttpResponseMessage GenerateAnnualPatenteForAnonymousUser([FromBody] AnonymousUserBillFilterDto billFilterDto)
        {
            var userBills = _serviceBill.GenerateAnnualPatenteForAnonymousUser(billFilterDto);
            if (userBills != null && userBills.Bills != null)
            {
                foreach (var bill in userBills.Bills)
                {
                    if (bill.Payment != null)
                    {
                        WebControllerHelper.LoadServiceReferenceParams(bill.Payment.ServiceDto);
                    }
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, userBills);
        }

    }
}