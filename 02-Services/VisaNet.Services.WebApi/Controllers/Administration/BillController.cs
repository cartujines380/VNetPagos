using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class BillController : ApiController
    {
        private readonly IServiceBill _serviceBill;

        public BillController(IServiceBill serviceBill)
        {
            _serviceBill = serviceBill;
        }

        [HttpPost]
        public HttpResponseMessage TestGatewayGetBills([FromBody] TestGatewaysFilterDto filterDto)
        {
            var bills = _serviceBill.TestGatewayGetBills(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, bills);
        }

    }
}