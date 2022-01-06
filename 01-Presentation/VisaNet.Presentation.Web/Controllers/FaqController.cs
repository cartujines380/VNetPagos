using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Mappers;

namespace VisaNet.Presentation.Web.Controllers
{
    public class FaqController : Controller
    {
        private readonly IWebFaqClientService _faqClientService;

        public FaqController(IWebFaqClientService faqClientService)
        {
            _faqClientService = faqClientService;
        }


        //
        // GET: /Faq/
        public async Task<ActionResult> Index()
        {
            var faqs = await _faqClientService.FindAll();

            return View(faqs.Select(f => f.ToModel()));
        }
    }
}