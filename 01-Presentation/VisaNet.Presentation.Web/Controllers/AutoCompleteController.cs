using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;

namespace VisaNet.Presentation.Web.Controllers
{
    public class AutoCompleteController : BaseController
    {
        private readonly IAutoCompleteClientService _autoCompleteClientService;

        public AutoCompleteController(IAutoCompleteClientService autoCompleteClientService)
        {
            _autoCompleteClientService = autoCompleteClientService;
        }

        //public async Task<ActionResult> AutoCompleteServices(string name_startsWith, int maxRows = 10)
        //{
        //    var data = await _autoCompleteClientService.AutoCompleteServices(name_startsWith);

        //    if (data.Any())
        //        return Json(data.Select(d => new { d.Id, d.Name }), JsonRequestBehavior.AllowGet);


        //    return Json(new List<dynamic> {new { Id = Guid.Empty, Name = "No se encontraron elementos" }}, JsonRequestBehavior.AllowGet);
        //}
    }
}
