using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class UploadHighwayEmailController : BaseController
    {
        //private IHighwayClientService _highwayClientService;

        //public UploadHighwayEmailController(IHighwayClientService highwayClientService)
        //{
        //    _highwayClientService = highwayClientService;
        //}

        // GET: UploadHighwayEmail
        [CustomAuthentication(Actions.ReportsHighwayEmails)]
        public async Task<ActionResult> Index()
        {
            return View();
        }

        [CustomAuthentication(Actions.ReportsHighwayEmails)]
        public async Task<ActionResult> ProccessFile()
        {
            var path = "";
            var httpPostedFileBase = Request.Files[0];
            if (httpPostedFileBase != null && httpPostedFileBase.ContentLength == 0)
            {
                ShowNotification("Debe seleccionar un archivo a procesar", NotificationType.Error);
            }
            else
            {
                var fileName = "";
                foreach (string fileToProccess in Request.Files)
                {
                    var input = Request.Files[fileToProccess];
                    fileName = ShortenPath(input.FileName);
                    if (input != null && !String.IsNullOrEmpty(input.FileName))
                    {
                        path = Path.Combine(ConfigurationManager.AppSettings["Tc33Path"], fileName);
                        if (System.IO.File.Exists(path))
                        {
                            //archivo ya existe en el directorio
                            NLogLogger.LogEvent(NLogType.Error, "UploadHighwayEmailController Error - Nombre de archivo repetido. Debe cambiar el nombre del archivo");
                            ShowNotification("Nombre de archivo repetido. Debe cambiar el nombre del archivo", NotificationType.Error);
                            return RedirectToAction("Index");
                        }
                        input.SaveAs(path);
                    }
                }
            }
            return RedirectToAction("Index");
        }

    }
}