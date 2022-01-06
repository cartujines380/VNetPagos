using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Models;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Web.Controllers
{
    public class AppLocalController : BaseController
    {

        private readonly IWebServiceClientService _webServiceClientService;

        public AppLocalController(IWebServiceClientService webServiceClientService)
        {
            _webServiceClientService = webServiceClientService;
        }

        // GET: AppLocal
        public async Task<ViewResult> Index(string id)
        {
            var idOperacion = Request.Form["IdOperacion"];
            var codResultado = Request.Form["CodResultado"];
            var descResultado = Request.Form["DescResultado"];

            var serviceDto = await _webServiceClientService.GetServiceByUrlName(id);
            var model = new AppLocalModel()
                        {
                            ServiceName = serviceDto.Name,
                            IdOperacion = DateTime.Now.ToString("yyyyMMddhhmmss"),
                            Id = id,
                            ServiceIntroContent = serviceDto.IntroContent,
                            ResponseCodResultado = codResultado,
                            ResponseDescResultado = descResultado,
                            ResponseIdOperacion = idOperacion,
                        };

            if (!string.IsNullOrEmpty(model.ResponseCodResultado))
            {
                var cod = int.Parse(model.ResponseCodResultado);
                if (cod == 0)
                {
                    ShowNotification(PresentationWebStrings.Apps_AssociationOk, NotificationType.Success);
                }
                else
                {
                    AnalyzeErrorMsg(cod, model.ResponseDescResultado);    
                }
            }

            return View(model);
        }

        private void AnalyzeErrorMsg(int code, string desc)
        {

            NLogLogger.LogAppsEvent(NLogType.Info, string.Format("Se invoco la pagina AppLocalController con codigo: {0}, desc: {1}", code, desc));

            var msg = PresentationWebStrings.Apps_AssociationError;
            switch (code)
            {
                case 14:
                    msg = "Ha ocurrido un error. Si bien se creo el usuario no hemos pidido asociar el servicio. Intente nuevamente y si el error persiste comuniquese con el call center ";
                    break;
                case 15:
                    msg = "Ha ocurrido un error. Si bien se creo el usuario y la tarjeta ingresada no hemos pidido asociar el servicio. Intente nuevamente y si el error persiste comuniquese con el call center ";
                    break;
                case 16:
                    msg = string.Empty;
                    break;
            }
            if(!string.IsNullOrEmpty(msg))
            {
                ShowNotification(msg, NotificationType.Error);
            }
        }
    }


}