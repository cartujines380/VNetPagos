using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Areas.Private.Mappers;
using VisaNet.Presentation.Web.Areas.Private.Models;
using VisaNet.Presentation.Web.Controllers;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.Web.Areas.Private.Controllers
{
    [CustomAuthorize]
    public class AutomaticPaymentController : BaseController
    {
        private readonly IWebServiceAssosiateClientService _webServiceAssosiateClientService;
        private readonly IWebApplicationUserClientService _webApplicationUserClientService;
        private readonly IWebServiceClientService _webServiceClientService;

        public AutomaticPaymentController(IWebServiceAssosiateClientService webServiceAssosiateClientService, IWebApplicationUserClientService webApplicationUserClientService, IWebServiceClientService webServiceClientService)
        {
            _webServiceAssosiateClientService = webServiceAssosiateClientService;
            _webApplicationUserClientService = webApplicationUserClientService;
            _webServiceClientService = webServiceClientService;
        }

        private async Task<List<ServiceListModel>> LoadServices(String name)
        {
            var currentUser = await CurrentSelectedUser();

            var list = await _webServiceAssosiateClientService.GetServicesWithAutomaticPayment(new ServiceFilterAssosiateDto()
            {
                UserId = currentUser.Id,
                GenericSearch = name
            });
            if (!list.Any())
            {
                var hasService = await _webServiceAssosiateClientService.HasAsosiatedService(currentUser.Id);
                if (hasService)
                {
                    TempData["LoadinError_AutomaticPayment"] = PresentationWebStrings.AutomaticPayment_WithoutActiveService;
                }
                else
                {
                    TempData["LoadinError_AutomaticPayment"] = PresentationWebStrings.AutomaticPayment_List_Empty;
                }
                return new List<ServiceListModel>();
            }

            var models = new List<ServiceListModel>();
            currentUser = await _webApplicationUserClientService.GetUserWithCards(currentUser.Id);
            foreach (var serviceAssociatedDto in list)
            {
                models.Add(new ServiceListModel()
                {
                    ServiceId = serviceAssociatedDto.Id,
                    ServiceName = serviceAssociatedDto.ServiceDto.Name,
                    ServiceDesc = serviceAssociatedDto.Description,
                    ServiceRefName = serviceAssociatedDto.ServiceDto.ReferenceParamName,
                    ServiceRefName2 = serviceAssociatedDto.ServiceDto.ReferenceParamName2,
                    ServiceRefName3 = serviceAssociatedDto.ServiceDto.ReferenceParamName3,
                    ServiceRefValue = serviceAssociatedDto.ReferenceNumber,
                    ServiceRefValue2 = serviceAssociatedDto.ReferenceNumber2,
                    ServiceRefValue3 = serviceAssociatedDto.ReferenceNumber3,
                    ServiceRefName4 = serviceAssociatedDto.ServiceDto.ReferenceParamName4,
                    ServiceRefName5 = serviceAssociatedDto.ServiceDto.ReferenceParamName5,
                    ServiceRefName6 = serviceAssociatedDto.ServiceDto.ReferenceParamName6,
                    ServiceRefValue4 = serviceAssociatedDto.ReferenceNumber4,
                    ServiceRefValue5 = serviceAssociatedDto.ReferenceNumber5,
                    ServiceRefValue6 = serviceAssociatedDto.ReferenceNumber6,
                    ServiceImageName = GetImageForService(serviceAssociatedDto.ServiceDto),
                    CardsMask = currentUser.CardDtos.Where(c => c.Active).Select(t => t.MaskedNumber).ToList(),
                    Active = serviceAssociatedDto.Active,
                    ServiceAutomaticPaymentId = serviceAssociatedDto.AutomaticPaymentDtoId,
                    DefaultMask = serviceAssociatedDto.DefaultCard.MaskedNumber,
                    EnableAutomaticPayment = serviceAssociatedDto.ServiceDto.EnableAutomaticPayment
                });
            }
            return models;
        }

        public async Task<ActionResult> GetServicesAssosiated(String name)
        {
            try
            {
                var models = await LoadServices(name);

                ViewBag.IsSearch = !String.IsNullOrEmpty(name);

                return PartialView("_ServiceList", models);
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Info);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            return null;
        }

        public ActionResult Index()
        {
            return View("Index");
        }

        [HttpGet]
        public async Task<ActionResult> Item(Guid serviceId, bool fromConfiguration)
        {
            var serviceAssosiated = await _webServiceAssosiateClientService.Find(serviceId);
            var service = await _webServiceClientService.Find(serviceAssosiated.ServiceId);

            var model = serviceAssosiated.ToAutomaticPaymentModel();
            model.FromConfiguration = fromConfiguration;
            model.Sucive = service.ServiceGatewaysDto.Any(s => s.Gateway.Enum == (int)GatewayEnum.Sucive && s.Active);
            return View("Item", model);
        }

        [HttpPost]
        public async Task<ActionResult> Item(AutomaticPaymentModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!model.UnlimitedAmount)
                    {
                        if (model.MaxAmount <= 0)
                        {
                            model.MaxAmountIsNullOrZero = true;
                            return View("Item", model);
                        }
                    }

                    var automaticPaymentDto = new AutomaticPaymentDto()
                    {
                        DaysBeforeDueDate = model.DayBeforeExpiration,
                        Maximum = model.MaxAmount,
                        Quotas = model.MaxCountPayments,
                        ServiceAssosiateId = model.ServiceId,
                        UnlimitedQuotas = model.UnlimitedQuotas,
                        UnlimitedAmount = model.UnlimitedAmount,
                        SuciveAnnualPatent = model.SuciveAnnualPatent
                    };

                    await _webServiceAssosiateClientService.AddPayment(automaticPaymentDto);

                    ShowNotification(PresentationWebStrings.Automatic_Payment_Updated, NotificationType.Success);
                    return RedirectToAction("Index", "Service");
                }
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Info);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            return View("Item", model);
        }

        //public async Task<ActionResult> Delete(Guid serviceId, String name)
        //{
        //    try
        //    {
        //        await _webServiceAssosiateClientService.DeleteAutomaticPayment(serviceId);
        //        ShowNotification(PresentationWebStrings.Automatic_Payment_Deleted, NotificationType.Success);
        //        var models = await LoadServices(name);

        //        ViewBag.IsSearch = !String.IsNullOrEmpty(name);

        //        return PartialView("_ServiceList", models);
        //    }
        //    catch (WebApiClientBusinessException ex)
        //    {
        //        ShowNotification(ex.Message, NotificationType.Info);
        //    }
        //    catch (WebApiClientFatalException ex)
        //    {
        //        ShowNotification(ex.Message, NotificationType.Error);
        //    }
        //    return RedirectToAction("Index");
        //}

    }
}