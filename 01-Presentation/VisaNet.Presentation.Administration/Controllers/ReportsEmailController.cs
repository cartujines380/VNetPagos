using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsEmailController : BaseController
    {
        private readonly IEmailService _emailService;

        public ReportsEmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [CustomAuthentication(Actions.ReportsMailsList)]
        public ActionResult Index()
        {
            return View("Index", new ReportsEmailsFilterDto()
            {
                DateTo = DateTime.Today,
                DateFrom = DateTime.Today.AddDays(-1),
                EmailType = -1,
                Status = -1
            });
        }

        [CustomAuthentication(Actions.ReportsMailsList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerEmails(Request, param);

            var data = await _emailService.ReportsEmailsData(filter);
            var totalRecords = await _emailService.ReportsEmailsDataCount(filter);

            var dataModel = data.Select(d => new
            {
                Id = d.Id,
                Status = EnumHelpers.GetName(typeof(MailgunStatusDto), (int)d.Status, EnumsStrings.ResourceManager),
                StatusCancelable = (d.Status == MailgunStatusDto.DroppedOld || d.Status == MailgunStatusDto.FailureReachingMg),
                Date = d.CreationDateTime.ToString("dd/MM/yyyy HH:mm"),
                Type = EnumHelpers.GetName(typeof(EmailTypeDto), (int)d.EmailType, EnumsStrings.ResourceManager),
                EmailAdress = d.To,
                MailgunErrorDescription = d.MailgunErrorDescription
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthentication(Actions.ReportsMailsCancel)]
        public async Task<ActionResult> Cancel(Guid id)
        {
            try
            {
                await _emailService.CancelEmail(id);
                return Json(new JsonResponse(AjaxResponse.Success, "", PresentationAdminStrings.Email_CancelSuccess, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
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

        [CustomAuthentication(Actions.ReportsMailsResend)]
        public async Task<ActionResult> Resend(Guid id)
        {
            try
            {
                await _emailService.ResendEmail(id);
                return Json(new JsonResponse(AjaxResponse.Success, "", PresentationAdminStrings.Email_ResendSuccess, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
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
        [CustomAuthentication(Actions.ReportsMailsDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var model = await _emailService.Find(id);

                var content = model.Body;
                var title = EnumHelpers.GetName(typeof(EmailTypeDto), (int)model.EmailType, EnumsStrings.ResourceManager) + " (" + EnumHelpers.GetName(typeof(MailgunStatusDto), (int)model.Status, EnumsStrings.ResourceManager) + ")";
                var to = model.To;
                var hasAttachments = false;

                if (model.EmailType == EmailTypeDto.NewPayment || model.EmailType == EmailTypeDto.CopyPayment)
                {
                    hasAttachments = true;
                }
                else if (model.EmailType == EmailTypeDto.HighwayTransactionReportsOk || model.EmailType == EmailTypeDto.ExtractBanred ||
                    model.EmailType == EmailTypeDto.ExtractImporte || model.EmailType == EmailTypeDto.ExtractGeocom)
                {
                    var path = (string)JObject.Parse(model.DataByType)["FilePath"];
                    var fileName = (string)JObject.Parse(model.DataByType)["FileName"];
                    var mimeType = (string)JObject.Parse(model.DataByType)["MimeType"];
                    if (!string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(mimeType))
                    {
                        hasAttachments = true;
                    }
                }

                var mailId = id;
                return Json(new JsonResponse(AjaxResponse.Success, new { title, content, to, hasAttachments, mailId }, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
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

        public async Task<ActionResult> DownloadAttachment(Guid id)
        {
            var ticket = await _emailService.DownloadAttachment(id);

            return File(ticket.ArrBytes, ticket.MimeType, ticket.FileName);
        }

        [CustomAuthentication(Actions.ReportsMailsResendAll)]
        public async Task<ActionResult> ResendAll()
        {
            try
            {
                var count = await _emailService.ResendAll();
                return Json(new JsonResponse(AjaxResponse.Success, "", count > 0 ? PresentationAdminStrings.Email_ResendAllSuccess : PresentationAdminStrings.Email_ResendAllSuccess_NoEmails, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));

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

        [CustomAuthentication(Actions.ReportsMailsResendAll)]
        public ActionResult CheckStatus()
        {
            try
            {
                _emailService.CheckStatus();
                return Json(new JsonResponse(AjaxResponse.Success, "", PresentationAdminStrings.Email_CheckStatusSuccess, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
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

    }
}