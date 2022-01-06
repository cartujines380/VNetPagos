using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Controllers
{
    public class HelpController : Controller
    {
       private readonly IWebPageClientService _pageClientService;

       public HelpController(IWebPageClientService pageClientService)
        {
            _pageClientService = pageClientService;
        }


        //
        // GET: /InstitutionalContent/
        public async Task<ActionResult> Index()
        {
            var page = await _pageClientService.FindType(PageTypeDto.Help);
            return View(new PageModel() { Content = page.Content });
        }
    }
}