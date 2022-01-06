using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Presentation.Web.Mappers;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IWebServiceClientService _serviceClientService;
        private readonly IWebPromotionClientService _promotionClientService;
        private readonly IWebHomePageClientService _webHomePageClientService;
        private readonly IWebDebitClientService _webDebitClientService;

        public HomeController(IWebServiceClientService serviceClientService,
                              IWebPromotionClientService promotionClientService,
            IWebHomePageClientService webHomePageClientService, IWebDebitClientService webDebitClientService)
        {
            _serviceClientService = serviceClientService;
            _promotionClientService = promotionClientService;
            _webHomePageClientService = webHomePageClientService;
            _webDebitClientService = webDebitClientService;
        }


        public async Task<ActionResult> Index()
        {
            Session[SessionConstants.PAYMENT_DATA] = null;

            if (User.Identity.IsAuthenticated && Session[SessionConstants.CURRENT_SELECTED_USER] != null)
            {
                return RedirectToAction("Index", "Dashboard", new RouteValueDictionary() { { "Area", "Private" } });
            }

            var dataPage = await _webHomePageClientService.Get();
            var services = await _serviceClientService.GetServicesPaymentPublic();

            ViewBag.DataPage = dataPage;
            ViewBag.Services = services.Select(s => new ServiceDto { Id = s.Id, Name = s.Name, Tags = s.Tags }).ToList().OrderBy(s => s.Name);

            ICollection<CustomerSiteCommerceDto> commerces = null;
            
            try
            {
                commerces = await _webDebitClientService.GetCommercesDebit();
            }
            catch (Exception exception)
            {
                
                
            }

            ViewBag.Commerces = commerces != null ? 
                commerces.Select(s => new CommerceModel { Id = s.Id, Name = s.Name, Tags = string.Empty }).ToList().OrderBy(s => s.Name) :
                (new List<CommerceModel>()).OrderBy(s => s.Name);

            return View();
        }

        public async Task<ActionResult> GetActivePromotion()
        {
            var actives = await _promotionClientService.FindActive();
            if (actives.Any())
            {
                var model = actives.FirstOrDefault().ToModel();
                return PartialView("_Promotion", model);
            }

            return Content("None");
        }

        public ActionResult Timeout()
        {
            Session[SessionConstants.PAYMENT_DATA] = null;
            //Session[SessionConstants.PAYMENT_DATA_ANONYMOUS_USER] = null;
            //Session[SessionConstants.PAYMENT_DATA_ANONYMOUS_USER_ID] = null;
            Session[SessionConstants.SERVICES_ASSOSIATION] = null;
            return View();
        }

    }
}