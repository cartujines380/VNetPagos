using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Areas.Private.Models;
using VisaNet.Presentation.Web.Controllers;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Web.Areas.Private.Controllers
{
    [CustomAuthorize]
    public class AutomaticPaymentScheduledController : BaseController
    {
        private readonly IWebServiceAssosiateClientService _webServiceAssosiateClientService;
        private readonly IWebBillClientService _webBillClientService;

        public AutomaticPaymentScheduledController(IWebServiceAssosiateClientService webServiceAssosiateClientService, IWebBillClientService webBillClientService)
        {
            _webServiceAssosiateClientService = webServiceAssosiateClientService;
            _webBillClientService = webBillClientService;
            
        }
        
	}
}