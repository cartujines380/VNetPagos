using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VisaNet.Common.AzureUpload;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class InterpreterController : BaseController
    {
        private readonly IInterpreterClientService _interpreterClientService;

        public InterpreterController(IInterpreterClientService interpreterClientService)
        {
            _interpreterClientService = interpreterClientService;
        }

        [HttpGet]
        [CustomAuthentication(Actions.InterpreterList)]
        public async Task<ActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        [CustomAuthentication(Actions.InterpreterCreate)]
        public async Task<ActionResult> Create()
        {
            try
            {
                return View(new InterpreterModel());
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
                return RedirectToAction("Index");
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.InterpreterCreate)]
        public async Task<ActionResult> Create(InterpreterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var newdto = await _interpreterClientService.Create(model.ToDto());
                try
                {
                    foreach (string file in Request.Files)
                    {
                        var interpreterdll = Request.Files[file];
                        if (interpreterdll != null && !String.IsNullOrEmpty(interpreterdll.FileName))
                        {
                            if (!IsDll(interpreterdll))
                            {
                                ShowNotification(PresentationAdminStrings.Interpreter_BadFile, NotificationType.Error);
                                await _interpreterClientService.Delete(newdto.Id);
                                return View(model);
                            }

                            var path = Path.Combine(ConfigurationManager.AppSettings["InterpreterFilePath"], newdto.Id + ".dll");
                            interpreterdll.SaveAs(path);

                            CreateFileAzure(path, interpreterdll.ContentType);
                        }
                    }
                }
                catch (Exception exception)
                {
                    NLogLogger.LogEvent(exception);
                    _interpreterClientService.Delete(newdto.Id).Wait();
                    ShowNotification(PresentationAdminStrings.Interpreter_Error, NotificationType.Error);
                    return RedirectToAction("Index");
                }
                ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);
                return RedirectToAction("Index");
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            return View(model);
        }

        [CustomAuthentication(Actions.InterpreterDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _interpreterClientService.Delete(id);

                try
                {
                    DeleteFileAzure(id);
                }
                catch (Exception exception)
                {
                    NLogLogger.LogEvent(exception);
                    return Json(new JsonResponse(AjaxResponse.Error, "", PresentationAdminStrings.Interpreter_Delete_Error, PresentationCoreMessages.NotificationFail, NotificationType.Info));
                }
                return Json(new JsonResponse(AjaxResponse.Success, "", PresentationCoreMessages.Common_DeleteSuccess, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
            }
            catch (WebApiClientBusinessException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (WebApiClientFatalException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
        }

        [HttpGet]
        [CustomAuthentication(Actions.InterpreterEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var dto = await _interpreterClientService.Find(id);
                return View(dto.ToModel());
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.InterpreterEdit)]
        public async Task<ActionResult> Edit(InterpreterModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var dto = model.ToDto();
                await _interpreterClientService.Edit(dto);

                try
                {
                    foreach (string file in Request.Files)
                    {
                        var interpreterdll = Request.Files[file];
                        if (interpreterdll != null && !String.IsNullOrEmpty(interpreterdll.FileName))
                        {
                            if (!IsDll(interpreterdll))
                            {
                                ShowNotification(PresentationAdminStrings.Interpreter_BadFile, NotificationType.Error);
                                return View(model);
                            }

                            //ELIMINO EL ACHIVO VIEJO
                            DeleteFileAzure(model.Id);

                            //ELIMINO ARCHIVO LOCAL
                            var path = Path.Combine(ConfigurationManager.AppSettings["InterpreterFilePath"], dto.Id + ".dll");
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                                interpreterdll.SaveAs(path);
                            }
                            //CREO FILE EN AZURE
                            CreateFileAzure(path, interpreterdll.ContentType);
                        }
                    }
                }
                catch (Exception exception)
                {
                    NLogLogger.LogEvent(exception);
                    ShowNotification(PresentationAdminStrings.Interpreter_Edit_Error, NotificationType.Error);
                    return RedirectToAction("Index");
                }

                ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);
                return RedirectToAction("Index");
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            return View(model);
        }

        [CustomAuthentication(Actions.InterpreterList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerInterpreter(Request, param);

            var data = await _interpreterClientService.FindAll(filter);
            var totalRecords = await _interpreterClientService.GetDataForInterpreterCount(filter);

            var dataModel = data.Select(d => new
            {
                d.Id,
                d.Name,
                d.Description,
                FileName = d.Id + ".dll"
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);
            return null;
        }

        [HttpGet]
        [CustomAuthentication(Actions.InterpreterDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var dto = await _interpreterClientService.Find(id);
                return View(dto.ToModel());
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);

            }
            return RedirectToAction("Index");
        }

        private bool IsDll(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("dll"))
            {
                return true;
            }

            string[] formats = new string[] { ".dll" };

            // linq from Henrik Stenbæk
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
        private void CreateFileAzure(string path, string contentType)
        {
            //EL NOMBRE DEL ARCHIVO TIENE QUE SER EL MISMO QUE EL ID
            var storage = FileStorage.Instance;
            storage.UploadFile(BlobAccessType.Blob, path, contentType);

        }
        private void DeleteFileAzure(Guid id)
        {
            if (FileStorage.Instance.CheckIfFileExists(ConfigurationManager.AppSettings["InterpreterContainerName"], BlobAccessType.Blob, id + ".dll"))
            {
                FileStorage.Instance.DeleteFile(ConfigurationManager.AppSettings["InterpreterContainerName"], id + ".dll");
            }
        }

    }
}