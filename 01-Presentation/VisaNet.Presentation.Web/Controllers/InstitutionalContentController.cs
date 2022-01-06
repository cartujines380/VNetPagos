using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Controllers
{
    public class InstitutionalContentController : Controller
    {
        private readonly IWebPageClientService _pageClientService;

        public InstitutionalContentController(IWebPageClientService pageClientService)
        {
            _pageClientService = pageClientService;
        }


        //
        // GET: /InstitutionalContent/
        public async Task<ActionResult> Index()
        {
            var page = await _pageClientService.FindType(PageTypeDto.InstitutionalContent);

            return View(new PageModel() { Content = page.Content });
        }
    }
}