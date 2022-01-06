using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.ChangeTracker;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Helpers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Exportation;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class AuditController : BaseController
    {
        private readonly IAuditClientService _auditClientService;
        private readonly IChangeTrackerClientService _changeTrackerClientService;

        public AuditController(IAuditClientService auditClientService, IChangeTrackerClientService changeTrackerClientService)
        {
            _auditClientService = auditClientService;
            _changeTrackerClientService = changeTrackerClientService;
        }


        [HttpGet]
        [CustomAuthentication(Actions.AuditList)]
        public ActionResult Index()
        {
            try
            {
                return View(new AuditFilterDto());
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

        [CustomAuthentication(Actions.AuditList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerAudit(Request, param);

            var data = await _auditClientService.FindAll(filter);

            var dataModel = data.Select(d => new
            {
                Id = d.TransactionIdentifier,
                d.IP,
                d.TransactionIdentifier,
                SystemUser = d.SystemUserId.HasValue ? d.LDAPUserName : "-",
                ApplicationUser = d.ApplicationUserId.HasValue ? d.ApplicationUserEmail : "-",
                AnonymousUser = d.AnonymousUserId.HasValue ? d.AnonymousUserEmail : "-",
                DateTime = d.TransactionDateTime.ToString("dd/MM/yyyy HH:mm:ss"),
                LogOperationType = EnumHelpers.GetName(typeof(LogOperationType), (int)d.LogOperationType, EnumsStrings.ResourceManager),
                LogUserType = EnumHelpers.GetName(typeof(LogUserType), (int)d.LogUserType, EnumsStrings.ResourceManager),
                d.TotalRows,
            });

            if (filter.SortDirection == SortDirection.Asc)
                dataModel = dataModel.OrderBy(p => filter.OrderBy == "0" ? DateTime.Parse(p.DateTime).ToString("yyyy/MM/dd HH:mm:ss") :
                                        filter.OrderBy == "1" ? p.IP :
                                        filter.OrderBy == "2" ? p.LogUserType :
                                        filter.OrderBy == "3" ? p.LogOperationType :
                                        filter.OrderBy == "4" ? p.SystemUser :
                                        filter.OrderBy == "5" ? p.ApplicationUser :
                                        filter.OrderBy == "6" ? p.AnonymousUser :
                                        "").ToList();
            else
                dataModel = dataModel.OrderByDescending(p => filter.OrderBy == "0" ? DateTime.Parse(p.DateTime).ToString("yyyy/MM/dd HH:mm:ss") :
                                        filter.OrderBy == "1" ? p.IP :
                                        filter.OrderBy == "2" ? p.LogUserType :
                                        filter.OrderBy == "3" ? p.LogOperationType :
                                        filter.OrderBy == "4" ? p.SystemUser :
                                        filter.OrderBy == "5" ? p.ApplicationUser :
                                        filter.OrderBy == "6" ? p.AnonymousUser :
                                        "").ToList();

            var totalRecords = 0;
            if (data.Any())
                totalRecords = dataModel.First().TotalRows;

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);

            //var dataToShow = dataModel.Skip(filter.DisplayStart);

            //if (filter.DisplayLength.HasValue)
            //    dataToShow = dataToShow.Take(filter.DisplayLength.Value);

            //return Json(new
            //{
            //    sEcho = param.sEcho,
            //    iTotalRecords = data.Count(),
            //    iTotalDisplayRecords = data.Count(),
            //    aaData = dataToShow.ToList()
            //}, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthentication(Actions.AuditDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var data = await _auditClientService.GetLogs(id);
                if (data == null)
                {
                    return Json(new JsonResponse(AjaxResponse.Error, "", "data es null", "data es null", NotificationType.Error));
                }
                if (!data.Any())
                {
                    return Json(new JsonResponse(AjaxResponse.Error, "", "Error", "No hay elementos", NotificationType.Error));
                }
                var isRegistred = data.Any(x => x.ApplicationUserId.HasValue);
                var result = data.OrderBy(d => d.DateTime).Select(d => new AuditLogDetailModel
                {
                    Date = d.DateTime.ToString("dd/MM/yyyy HH:mm:ss.ffff"),
                    IP = d.IP,

                    SystemUser = d.SystemUserId.HasValue && d.SystemUser != null ? d.SystemUser.LDAPUserName : string.Empty,
                    ApplicationUser = d.ApplicationUserId.HasValue && d.ApplicationUser != null ? d.ApplicationUser.Email : string.Empty,
                    AnonymousUser = d.AnonymousUserId.HasValue && d.AnonymousUser != null ? d.AnonymousUser.Email : string.Empty,
                    Registered = isRegistred,
                    LogType = EnumHelpers.GetName(typeof(LogType), (int)d.LogType, EnumsStrings.ResourceManager),
                    LogUserType = EnumHelpers.GetName(typeof(LogUserType), (int)d.LogUserType, EnumsStrings.ResourceManager),
                    LogCommunicationType = EnumHelpers.GetName(typeof(LogCommunicationType), (int)d.LogCommunicationType, EnumsStrings.ResourceManager),
                    LogOperationType = EnumHelpers.GetName(typeof(LogOperationType), (int)d.LogOperationType, EnumsStrings.ResourceManager),
                    Message = d.Message,//+" -- " + d.ExceptionMessage + " -- "+ d.InnerException,
                }).ToList();

                var content = RenderPartialViewToString("_DetailsLightbox", result);

                return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
            }
            catch (WebApiClientBusinessException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (WebApiClientFatalException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }

            //var result = data.OrderBy(d => d.TransactionIdentifier).ThenBy(d => d.DateTime).Select(d => new
            //{
            //    d.IP,
            //    d.TransactionIdentifier,
            //    SystemUser = d.SystemUserId.HasValue ? d.SystemUser.LDAPUserName : string.Empty,
            //    ApplicationUser = d.ApplicationUserId.HasValue ? d.ApplicationUser.Email : string.Empty,
            //    AnonymousUser = d.AnonymousUserId.HasValue ? d.AnonymousUser.Email : string.Empty,
            //    DateTime = d.DateTime.ToString("dd/MM/yyyy HH:mm:ss.ffff"),
            //    LogType = EnumHelpers.GetName(typeof(LogType), (int)d.LogType, EnumsStrings.ResourceManager),
            //    LogUserType = EnumHelpers.GetName(typeof(LogUserType), (int)d.LogUserType, EnumsStrings.ResourceManager),
            //    LogCommunicationType = EnumHelpers.GetName(typeof(LogCommunicationType), (int)d.LogCommunicationType, EnumsStrings.ResourceManager),
            //    d.Message,
            //}).ToList();


            //return Json(new
            //{
            //    sEcho = param.sEcho,
            //    iTotalRecords = data.Count(),
            //    iTotalDisplayRecords = data.Count(),
            //    aaData = result.ToList()
            //}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CustomAuthentication(Actions.AuditList)]
        public async Task<ActionResult> ExcelExport()
        {
            var filter = new AuditFilterDto
            {
                DisplayLength = 1000,
                DisplayStart = 1
            };
            DateTime tmpDateTime;

            if (DateTime.TryParse(Request["From"], out tmpDateTime))
                filter.From = tmpDateTime;

            filter.From = filter.From.AddHours(Convert.ToInt32(Request["HoursFrom"])); //ADENTRO DEL IF??
            filter.From = filter.From.AddMinutes(Convert.ToInt32(Request["MinutesFrom"]));

            if (DateTime.TryParse(Request["To"], out tmpDateTime))
                filter.To = tmpDateTime;

            filter.To = filter.To.AddHours(Convert.ToInt32(Request["HoursTo"]));
            filter.To = filter.To.AddMinutes(Convert.ToInt32(Request["MinutesTo"]));

            LogOperationType logOperationTypeTmp;
            if (Enum.TryParse(Request["LogOperationType"], out logOperationTypeTmp))
                filter.LogOperationType = logOperationTypeTmp;

            LogUserType logUserTypeTmp;
            if (Enum.TryParse(Request["LogUserType"], out logUserTypeTmp))
                filter.LogUserType = logUserTypeTmp;

            if (!string.IsNullOrEmpty(Request["User"]))
                filter.User = Request["User"].Trim();

            if (!string.IsNullOrEmpty(Request["Message"]))
                filter.Message = Request["Message"];

            var data = await _auditClientService.ExcelExport(filter);
            var headers = new[]
                {
                    PresentationAdminStrings.Audit_DateTime,
                    PresentationAdminStrings.Audit_Ip,
                    PresentationAdminStrings.Audit_LogUserType,
                    PresentationAdminStrings.Audit_LogOperationType,
                    PresentationAdminStrings.Audit_SystemUser,
                    PresentationAdminStrings.Audit_ApplicationUser,
                    PresentationAdminStrings.Audit_AnonymousUser,
                    PresentationAdminStrings.Audit_Message,
                    PresentationAdminStrings.Audit_LogType,
                    PresentationAdminStrings.Audit_LogCommunicationType
                };

            var memoryStream = ExcelExporter.ExcelExport("Auditoría", data, headers);
            return File(memoryStream, WebConfigurationManager.AppSettings["ExcelResponseType"],
                        string.Format("{0}.{1}", "Auditoría", "xlsx"));
        }

        [HttpGet]
        [CustomAuthentication(Actions.AuditChangeLog)]
        public async Task<ActionResult> ChangeLog()
        {
            try
            {
                var filters = new ChangeTrackerFilterDto
                {
                    From = DateTime.Today,
                    To = DateTime.Today.AddDays(1),
                    SortDirection = SortDirection.Desc
                };

                var email = Request["Email"];
                var tableName = Request["TableName"];

                if (!String.IsNullOrEmpty(email))
                {
                    filters.From = DateTime.Today.AddYears(-1);
                    filters.UserName = email;
                }
                if (!String.IsNullOrEmpty(tableName))
                    filters.TableName = tableName;


                ViewBag.Entities = (await _changeTrackerClientService.GetEntities()).Select(x => new SelectListItem { Text = AuditHelper.TranslateAuditTableName(x), Value = x }).OrderBy(x => x.Text).ToList();
                return View(filters);
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
                return RedirectToAction("ChangeLog");
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
                return RedirectToAction("ChangeLog");
            }
        }

        [CustomAuthentication(Actions.AuditChangeLog)]
        public async Task<ActionResult> AjaxHandlerChangeLog(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerChangeTracker(Request, param);

            var data = await _changeTrackerClientService.FindAll(filter);

            var dataModel = data.Select(d => new
            {
                d.AuditLogId,
                d.TransactionIdentifier,
                d.IP,
                d.UserName,
                DateTime = d.EventDate.ToString("dd/MM/yyyy HH:mm:ss"),
                EventType = EnumHelpers.GetName(typeof(EventTypeDto), (int)d.EventType, EnumsStrings.ResourceManager),
                TableName = AuditHelper.TranslateAuditTableName(d.TableName),
                d.RecordId,
                d.AditionalInfo
            });

            if (filter.SortDirection == SortDirection.Asc)
                dataModel = dataModel.OrderBy(p => filter.OrderBy == "0" ? DateTime.Parse(p.DateTime).ToString("yyyy/MM/dd HH:mm:ss") :
                                        filter.OrderBy == "1" ? p.IP :
                                        filter.OrderBy == "2" ? p.UserName :
                                        filter.OrderBy == "3" ? p.EventType :
                                        filter.OrderBy == "4" ? p.TableName :
                                        filter.OrderBy == "5" ? p.AditionalInfo :
                                        "").ToList();
            else
                dataModel = dataModel.OrderByDescending(p => filter.OrderBy == "0" ? DateTime.Parse(p.DateTime).ToString("yyyy/MM/dd HH:mm:ss") :
                                        filter.OrderBy == "1" ? p.IP :
                                        filter.OrderBy == "2" ? p.UserName :
                                        filter.OrderBy == "3" ? p.EventType :
                                        filter.OrderBy == "4" ? p.TableName :
                                        filter.OrderBy == "5" ? p.AditionalInfo :
                                        "").ToList();

            var records = new[] { 0, 0 };
            if (data.Any())
                records = await _changeTrackerClientService.Count(filter);

            return Json(new
            {
                param.sEcho,
                iTotalRecords = records[0],
                iTotalDisplayRecords = records[1],
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthentication(Actions.AuditChangeLog)]
        public async Task<ActionResult> ChangeLogDetail(int id)
        {
            try
            {
                var data = await _changeTrackerClientService.GetChangesDetails(id);
                if (data == null)
                {
                    return Json(new JsonResponse(AjaxResponse.Error, "", "data es null", "data es null", NotificationType.Error));
                }

                var content = RenderPartialViewToString("_ChangeLogDetailsLightbox", data);

                return Json(new JsonResponse(AjaxResponse.Success, content, "", "", NotificationType.Success), JsonRequestBehavior.AllowGet);
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

        [CustomAuthentication(Actions.AuditChangeLog)]
        public async Task<ActionResult> ChangeLogExcelExport()
        {
            var filter = new ChangeTrackerFilterDto
            {
                DisplayLength = 1000,
                DisplayStart = 1
            };
            DateTime tmpDateTime;

            if (DateTime.TryParse(Request["From"], out tmpDateTime))
                filter.From = tmpDateTime;

            filter.From = filter.From.AddHours(Convert.ToInt32(Request["HoursFrom"]));
            filter.From = filter.From.AddMinutes(Convert.ToInt32(Request["MinutesFrom"]));

            if (DateTime.TryParse(Request["To"], out tmpDateTime))
                filter.To = tmpDateTime;

            filter.To = filter.To.AddHours(Convert.ToInt32(Request["HoursTo"]));
            filter.To = filter.To.AddMinutes(Convert.ToInt32(Request["MinutesTo"]));

            EventTypeDto eventTypeTmp;
            if (Enum.TryParse(Request["EventType"], out eventTypeTmp))
                filter.EventType = eventTypeTmp;
            if (!string.IsNullOrEmpty(Request["UserName"]))
                filter.UserName = Request["UserName"].Trim();

            if (!string.IsNullOrEmpty(Request["TableName"]))
                filter.TableName = Request["TableName"].Trim();

            if (!string.IsNullOrEmpty(Request["AditionalInfo"]))
                filter.TableName = Request["AditionalInfo"].Trim();

            var data = await _auditClientService.ChangeLogExcelExport(filter);
            var headers = new[]
                {
                    PresentationAdminStrings.TrackChanges_DateTime,
                    PresentationAdminStrings.TrackChanges_Ip,
                    PresentationAdminStrings.TrackChanges_LogType,
                    PresentationAdminStrings.TrackChanges_UserName,
                    PresentationAdminStrings.TrackChanges_TableName,
                    PresentationAdminStrings.TrackChanges_AditionalInfo,
                    PresentationAdminStrings.TrackChanges_ColumnName,
                    PresentationAdminStrings.TrackChanges_OrginalValue,
                    PresentationAdminStrings.TrackChanges_NewValue
                };

            var memoryStream = ExcelExporter.ExcelExport("Auditoría de cambios", data, headers);
            return File(memoryStream, WebConfigurationManager.AppSettings["ExcelResponseType"], string.Format("{0}.{1}", "Auditoría de cambios", "xlsx"));
        }
    }
}