using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.CustomerSite.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Controllers
{
    public class ServiceProviderController : Controller
    {
        private readonly IWebServiceClientService _webServiceClientService;
        private readonly IWebDebitClientService _webDebitClientService;

        public ServiceProviderController(IWebServiceClientService webServiceClientService, IWebDebitClientService webDebitClientService)
        {
            _webServiceClientService = webServiceClientService;
            _webDebitClientService = webDebitClientService;
        }

        public async Task<ActionResult> Index()
        {
            var vm = new ServiceProviderViewModel
            {
                Services = await GetServices(null, null),
            };
            return View(vm);
        }

        public async Task<ActionResult> LoadServicesAjax(Guid? serviceId, string commerceName)
        {
            var services = await GetServices(serviceId, commerceName);
            return PartialView("_Services", services);
        }

        private async Task<List<ServiceModel>> GetServices(Guid? serviceId, string commerceName)
        {
            var final = new List<ServiceModel>();

            var services = await _webServiceClientService.GetServicesProvider(serviceId);
            var commerces = await _webDebitClientService.GetCommercesDebit(new CustomerSiteCommerceFilterDto() { Name = commerceName });
            foreach (var serviceDto in services)
            {
                var commerce = commerces.FirstOrDefault(x => !string.IsNullOrEmpty(x.ServiceId) && x.ServiceId.Equals(serviceDto.Id.ToString(), StringComparison.OrdinalIgnoreCase));
                var model = new ServiceModel()
                {
                    Id = serviceDto.Id,
                    Name = serviceDto.Name,
                    Description = serviceDto.Description,
                    ImgUrl = serviceDto.ImageUrl,
                    CommerceId = commerce != null ? commerce.Id : Guid.Empty,
                    Sort = serviceDto.Sort,
                    EnableBill = true,
                    EnableDebit = commerce != null
                };
                final.Add(model);
            }

            var commmerceNotIn = commerces.Where(x => !final.Contains(new ServiceModel() { CommerceId = x.Id })).ToList();

            foreach (var customerSiteCommerceDto in commmerceNotIn)
            {
                var model = new ServiceModel()
                {
                    Name = customerSiteCommerceDto.Name,
                    CommerceId = customerSiteCommerceDto.Id,
                    Sort = customerSiteCommerceDto.Sort,
                    Id = Guid.NewGuid(), //NO HAY ID DE SERVICIO, INVENTO UNO PARA LISTADO
                    EnableBill = false,
                    EnableDebit = true,
                    ImgUrl = customerSiteCommerceDto.ImageUrl
                };
                final.Add(model);
            }

            return final.OrderBy(s => s.Sort).ThenBy(s => s.Name).ToList();
        }

    }
}