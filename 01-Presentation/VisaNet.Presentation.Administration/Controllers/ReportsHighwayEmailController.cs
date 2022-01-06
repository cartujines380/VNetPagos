using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VisaNet.Common.AzureUpload;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsHighwayEmailController : BaseController
    {
        private readonly IReportsHighwayService _reportsHighwayService;

        private readonly string _highwayBlobFolder = ConfigurationManager.AppSettings["AzureHighwayUnprocessedFolder"];

        public ReportsHighwayEmailController(IReportsHighwayService reportsHighwayService)
        {
            _reportsHighwayService = reportsHighwayService;
        }

        [CustomAuthentication(Actions.ReportsHighwayEmails)]
        public ActionResult Index()
        {
            return View(new ReportsHighwayEmailFilterDto()
            {
                SortDirection = SortDirection.Desc,
            });
        }

        [CustomAuthentication(Actions.ReportsHighwayEmails)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerHighwayEmail(Request, param);

            var data = await _reportsHighwayService.GetHighwayEmailsReports(filter);
            var totalRecords = await _reportsHighwayService.GetHighwayEmailsReportsCount(filter);

            var dataToShow = data.Select(d => new
            {
                Id = d.Id,
                d.CodCommerce,
                d.CodBranch,
                d.Sender,
                ServiceName = d.Service != null ? d.Service.Name : "",
                d.AttachmentInputName,
                d.AttachmentOutputName,
                CreationDate = d.CreationDate.ToString("dd/MM/yyyy HH:mm:ss"),
                StatusValue = (int)d.Status,
                Status = EnumHelpers.GetName(typeof(HighwayEmailStatusDto), (int)d.Status, EnumsStrings.ResourceManager),
                Transaccion = d.Transaccion
            }).ToList();

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataToShow
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CustomAuthentication(Actions.ReportsHighwayEmails)]
        public async Task<ActionResult> DownloadFile(Guid id, string name)
        {
            return DownloadFileAzure(name, _highwayBlobFolder);
        }

        [CustomAuthentication(Actions.ReportsHighwayEmails)]
        public async Task<ActionResult> ProccessFileBlob()
        {
            //Validations
            if (Request.Files == null)
            {
                ShowNotification("Debe seleccionar un archivo a procesar.", NotificationType.Error);
                return RedirectToAction("Index");
            }
            var httpPostedFileBase = Request.Files[0];
            if (httpPostedFileBase == null)
            {
                ShowNotification("Debe seleccionar un archivo a procesar.", NotificationType.Error);
                return RedirectToAction("Index");
            }
            if (httpPostedFileBase.ContentLength == 0)
            {
                ShowNotification("El archivo no puede estar vacío.", NotificationType.Error);
                return RedirectToAction("Index");
            }

            try
            {
                var email = new HighwayEmailDto
                {
                    Sender = "",
                    RecipientEmail = "",
                    Subject = "",
                    TimeStampSeconds = "",
                    Status = HighwayEmailStatusDto.Processing
                };

                var files = 0;
                foreach (string fileToProccess in Request.Files)
                {
                    var input = Request.Files[fileToProccess];
                    if (input != null)
                    {
                        var name = input.FileName.Split('_');

                        //EL ARCHIVO TIENE QUE TENER 5 PARTES
                        if (name.Count() == 5)
                        {
                            //NO PUEDE HABER MAS DE UN ARCHIVO
                            if (files == 0)
                            {
                                files++;
                                email.AttachmentInputName = input.FileName;
                                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "    El archivo (" + input.FileName + ") es de tipo correcto (" + input.ContentType + ")");

                                //Subir archivo a Blob
                                UploadFileToBlob(input);

                                //Inicia procesamiento
                                _reportsHighwayService.ProccessEmailFileExternalSoruce(email);

                                ShowNotification("El archivo se está procesando. El estado final estará disponible en unos momentos.", NotificationType.Success);
                            }
                            else
                            {
                                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "    Mas de un Archivo");
                                email.Status = HighwayEmailStatusDto.RejectedFileNotOne;
                                ShowNotification("No debe seleccionar más de un archivo.", NotificationType.Error);
                            }
                        }
                        else
                        {
                            NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "    Nombre de Archivo mal formado");
                            email.Status = HighwayEmailStatusDto.RejectedFileNameBadlyFormed;
                            ShowNotification("Nombre de archivo mal formado.", NotificationType.Error);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogHighwayFileProccessEvent(e);
                ShowNotification("Error al procesar archivo.", NotificationType.Error);
            }
            return RedirectToAction("Index");
        }

        [CustomAuthentication(Actions.ReportsHighwayEmails)]
        public async Task<ActionResult> ErrorsModal(Guid id)
        {
            var highwayEmail = await _reportsHighwayService.GetHighwayEmail(id);
            return PartialView("_ModalErrors", highwayEmail.Errors);
        }

        private void UploadFileToBlob(HttpPostedFileBase input)
        {
            var fileName = input.FileName;
            byte[] buffedFile;
            using (var reader = new BinaryReader(input.InputStream))
            {
                buffedFile = reader.ReadBytes(input.ContentLength);
            }

            DeleteFileAzure(fileName, _highwayBlobFolder);
            CreateFileAzure(_highwayBlobFolder, fileName, buffedFile, "text/plain");
        }

    }
}