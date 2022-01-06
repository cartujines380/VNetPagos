using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Resource.Models;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.CustomerSite.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class CustomerSiteSystemUserController : BaseController
    {
        private readonly ICustomerSiteSystemUserClientService _customerSiteSystemUserClientService;
        private readonly ICustomerSiteCommerceClientService _customerSiteCommerceClientService;
        private readonly ICustomerSiteBranchClientService _customerSiteBranchClientService;
        private readonly IEmailService _emailService;

        public CustomerSiteSystemUserController(ICustomerSiteSystemUserClientService customerSiteSystemUserClientService,
            ICustomerSiteCommerceClientService customerSiteCommerceClientService, ICustomerSiteBranchClientService customerSiteBranchClientService, IEmailService emailService)
        {
            _customerSiteSystemUserClientService = customerSiteSystemUserClientService;
            _customerSiteCommerceClientService = customerSiteCommerceClientService;
            _customerSiteBranchClientService = customerSiteBranchClientService;
            _emailService = emailService;
        }

        [CustomAuthentication(Actions.CustomerSiteSystemUserList)]
        public async Task<ActionResult> Index()
        {
            try
            {
                await LoadViewBagData(string.Empty, CustomerSystemUserUserType.Master);
                return View();
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            return null;
        }

        [HttpGet]
        [CustomAuthentication(Actions.CustomerSiteSystemUserCreate)]
        public async Task<ActionResult> Create()
        {
            try
            {
                var model = new CustomerSiteSystemUserModel();
                await LoadViewBagData(null, CustomerSystemUserUserType.Master);
                return View(model);
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
        [CustomAuthentication(Actions.CustomerSiteSystemUserCreate)]
        public async Task<ActionResult> Create(CustomerSiteSystemUserModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBagData(model.CommerceId.ToString(), model.UserType);
                    return View(model);
                }

                if (model.CommerceId == Guid.Empty)
                {
                    ShowNotification("No se selecciono un comercio", NotificationType.Error);
                    await LoadViewBagData(model.CommerceId.ToString(), model.UserType);
                    return View(model);
                }

                if (model.UserType == CustomerSystemUserUserType.Alternative && model.BranchId == Guid.Empty)
                {
                    ShowNotification("No se selecciono una sucursal", NotificationType.Error);
                    await LoadViewBagData(model.CommerceId.ToString(), model.UserType);
                    return View(model);
                }

                //var service = await _serviceClientService.Find(Guid.Parse(model.ServiceId));
                var dto = model.ToDto();

                var updateDto = await _customerSiteSystemUserClientService.Create(dto);

                var successMsg = PresentationCoreMessages.NotificationSuccess;

                //envio email?
                if (dto.SendEmailActivation)
                {
                    try
                    {
                        await _emailService.SendCustomerSiteSystemUserCreationEmail(updateDto);
                        successMsg = successMsg + " Se envio el correo electrónico para activación de usuario";

                    }
                    catch (Exception exception)
                    {
                        NLogLogger.LogEvent(exception);
                        successMsg = successMsg + " NO SE PUDO ENVIAR el correo electrónico para activación de usuario";
                    }
                }
                ShowNotification(successMsg, NotificationType.Success);
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
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
                ShowNotification(PresentationAdminStrings.Error_General_Model, NotificationType.Error);
            }
            await LoadViewBagData(model != null ? model.CommerceId.ToString() : Guid.Empty.ToString(),
                 model != null ? model.UserType : CustomerSystemUserUserType.Master);
            return View(model);
        }

        [HttpGet]
        [CustomAuthentication(Actions.CustomerSiteSystemUserDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var user = await _customerSiteSystemUserClientService.Find(id);
                await LoadViewBagData(user.CommerceDto.Id.ToString(), CustomerSystemUserUserType.Master);
                return View(user.ToModel());
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

        [HttpGet]
        [CustomAuthentication(Actions.CustomerSiteSystemUserEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var systemUser = await _customerSiteSystemUserClientService.Find(id);
                var model = systemUser.ToModel();
                await LoadViewBagData(model.CommerceId.ToString(), CustomerSystemUserUserType.Master);
                return View(model);
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
        [CustomAuthentication(Actions.CustomerSiteSystemUserEdit)]
        public async Task<ActionResult> Edit(CustomerSiteSystemUserModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBagData(model.CommerceId.ToString(), model.UserType);
                    return View(model);
                }

                if (model.CommerceId == Guid.Empty)
                {
                    ShowNotification("No se selecciono un comercio", NotificationType.Error);
                    await LoadViewBagData(model.CommerceId.ToString(), model.UserType);
                    return View(model);
                }

                if (model.UserType == CustomerSystemUserUserType.Alternative && model.BranchId == Guid.Empty)
                {
                    ShowNotification("No se selecciono una sucursal", NotificationType.Error);
                    await LoadViewBagData(model.CommerceId.ToString(), model.UserType);
                    return View(model);
                }

                var dto = model.ToDto();
                await _customerSiteSystemUserClientService.Edit(dto);
                ShowNotification(PresentationAdminStrings.CustomerSite_Success, NotificationType.Success);
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            catch (Exception exception)
            {
                ShowNotification(PresentationAdminStrings.Error_General_Model, NotificationType.Error);
            }
            await LoadViewBagData(model.CommerceId.ToString(), CustomerSystemUserUserType.Master);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuthentication(Actions.CustomerSiteSystemUserDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _customerSiteSystemUserClientService.Delete(id);
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

        [HttpPost]
        [CustomAuthentication(Actions.CustomerSiteSystemUserEnable)]
        public async Task<ActionResult> ChangeState(Guid id)
        {
            try
            {
                await _customerSiteSystemUserClientService.ChangeState(id);
                return Json(new JsonResponse(AjaxResponse.Success, "", PresentationCoreMessages.Common_Status_Success, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
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

        [CustomAuthentication(Actions.CustomerSiteSystemUserList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerCustomerSiteSystemUser(Request, param);
            var data = await _customerSiteSystemUserClientService.GetDataForSystemUserTable(filter);
            var totalRecords = await _customerSiteSystemUserClientService.GetDataForSystemUserTableCount(filter);

            var dataModel = data.Select(d => new
            {
                Id = d.Id,
                Email = d.Email,
                CommerceName = d.CommerceDto != null ? d.CommerceDto.Name : string.Empty,
                Master = d.Master ? "1" : "0",
                SentEmailActivation = d.SendEmailActivation ? "1" : "0",
                Active = d.MembershipIdentifierObj != null && d.MembershipIdentifierObj.Active ? "1" : "0",
                Blocked = d.MembershipIdentifierObj != null && d.MembershipIdentifierObj.Blocked ? "1" : "0",
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalRecords,
                aaData = dataModel.ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetBranchesLigth(string commerceId, CustomerSystemUserUserType userType)
        {
            try
            {
                if (userType == CustomerSystemUserUserType.Master)
                {
                    return Json(new JsonResponse(AjaxResponse.Success, string.Empty,
                        PresentationCoreMessages.NotificationSuccess, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
                }
                var branches = await _customerSiteBranchClientService.GetBranchesLigth(new CustomerSiteBranchFilterDto
                {
                    CommerceId = Guid.Parse(commerceId),
                    IsDebitCommerce = false
                });
                ViewBag.BranchesLigthList = branches != null ? GetBranchLight(branches, Guid.Empty) : null;
                var content = RenderPartialViewToString("_BranchesDropDownList");
                return Json(new JsonResponse(AjaxResponse.Success, content, PresentationCoreMessages.NotificationSuccess, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e);
            }
            return Json(new JsonResponse(AjaxResponse.Error, "", "", PresentationCoreMessages.NotificationFail, NotificationType.Error));
        }

        public async Task<bool> LoadViewBagData(string commerceId, CustomerSystemUserUserType selectedUserType)
        {
            var listCommerces = await GetCommercesLightList();
            var list = GetCommercesLight(listCommerces, string.IsNullOrEmpty(commerceId) ? Guid.Empty : Guid.Parse(commerceId));
            ViewBag.CommercesLigthList = list;
            ViewBag.SystemUserTypeList = GetSystemUserTypeList(selectedUserType);
            return true;
        }

        private List<SelectListItem> GetSystemUserTypeList(CustomerSystemUserUserType selectedUserType)
        {
            var rm = ModelsStrings.ResourceManager;

            var list = new List<CustomerSystemUserUserType>() { CustomerSystemUserUserType.Master };
            return list.Select(discountType => new SelectListItem()
            {
                Text = rm.GetString(string.Concat("CustomerSystemUserUserType_", discountType.ToString())),
                Value = (int)discountType + "",
                Selected = selectedUserType == discountType
            }).Where(x => !x.Value.Equals("5")).ToList();
        }
        private async Task<ICollection<CustomerSiteCommerceDto>> GetCommercesLightList()
        {
            return await _customerSiteCommerceClientService.GetCommercesLigth();
        }
        private List<SelectListItem> GetCommercesLight(ICollection<CustomerSiteCommerceDto> commerces, Guid commerceId)
        {
            var aux = commerces.Select(dto => new SelectListItem() { Value = dto.Id.ToString(), Text = dto.Name, Selected = dto.Id == commerceId });
            var list = new List<SelectListItem>();
            list.AddRange(aux);
            return list;
        }
        private List<SelectListItem> GetBranchLight(ICollection<CustomerSiteBranchDto> branches, Guid branchId)
        {
            var aux = branches.Select(dto => new SelectListItem() { Value = dto.Id.ToString(), Text = dto.Name, Selected = dto.Id == branchId });
            var list = new List<SelectListItem>();
            list.AddRange(aux);
            return list;
        }

        public async Task<ActionResult> SendActivactionEmail(CustomerSiteSystemUserDto dto)
        {
            try
            {
                if (dto != null && string.IsNullOrEmpty(dto.Email))
                {
                    dto = await _customerSiteSystemUserClientService.Find(dto.Id);
                }
                await _emailService.SendCustomerSiteSystemUserCreationEmail(dto);
                return Json(new JsonResponse(AjaxResponse.Success, "", PresentationCoreMessages.Notification_Title_Success, PresentationCoreMessages.NotificationSuccess, NotificationType.Success));
            }
            catch (WebApiClientBusinessException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (WebApiClientFatalException ex)
            {
                return Json(new JsonResponse(AjaxResponse.Error, "", ex.Message, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
                return Json(new JsonResponse(AjaxResponse.Error, "", PresentationAdminStrings.Error_General_Model, PresentationCoreMessages.NotificationFail, NotificationType.Error));
            }
        }

    }
}