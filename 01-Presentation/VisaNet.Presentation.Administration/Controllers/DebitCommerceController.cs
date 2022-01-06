using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VisaNet.Common.AzureUpload;
using VisaNet.Common.Exceptions;
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
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class DebitCommerceController : BaseController
    {
        private readonly IServiceClientService _serviceClientService;
        private readonly IDebitCommerceClientService _debitCommerceClientService;
        private readonly ICustomerSiteCommerceClientService _customerSiteCommerceClientService;
        private readonly string _ImageFolder = ConfigurationManager.AppSettings["AzureCommerceImageFolder"];

        public DebitCommerceController(IDebitCommerceClientService debitCommerceClientService, ICustomerSiteCommerceClientService customerSiteCommerceClientService,
            IServiceClientService serviceClientService)
        {
            _debitCommerceClientService = debitCommerceClientService;
            _customerSiteCommerceClientService = customerSiteCommerceClientService;
            _serviceClientService = serviceClientService;
        }

        [HttpGet]
        [CustomAuthentication(Actions.DebitCommerceList)]
        public ActionResult Index()
        {
            var commerce = Request["CommerceName"];
            return View("Index", new CustomerSiteCommerceFilterDto()
            {
                Name = string.IsNullOrEmpty(commerce) ? string.Empty : commerce,
            });
        }

        [HttpGet]
        [CustomAuthentication(Actions.DebitCommerceDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var commerce = await _customerSiteCommerceClientService.Find(id);
                await LoadViewBagData(commerce.ServiceId);

                var model = commerce.ToCommerceModel();

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

        [HttpGet]
        [CustomAuthentication(Actions.DebitCommerceEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            var model = new CommerceModel();
            try
            {
                var commerce = await _customerSiteCommerceClientService.Find(id);
                await LoadViewBagData(commerce.ServiceId);

                model = commerce.ToCommerceModel();

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
            catch (BusinessException e)
            {
                if (e.Code == CodeExceptions.CONNECTION_FAILED)
                {
                    model.UploadImageDisabled = true;
                    ShowNotification(e.Message, NotificationType.Error);
                    return View(model);
                }
                ShowNotification(e.Message, NotificationType.Error);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.DebitCommerceEdit)]
        public async Task<ActionResult> Edit(CommerceModel model)
        {
            try
            {
                if (model.DeleteImage)
                {
                    DeleteImageAzure(model.ImageBlobName, _ImageFolder);
                    model.ImageName = null;
                }

                if (Request.Files.Count > 0)
                {
                    var image = Request.Files[0];

                    if (!IsImage(image))
                    {
                        throw new BusinessException(CodeExceptions.INVALID_IMAGE_FORMAT);
                    }
                    if (!IsSizeCorrect(image))
                    {
                        throw new BusinessException(CodeExceptions.INVALID_IMAGE_FORMAT);
                    }

                    model.ImageName = image.FileName;
                    DeleteImageAzure(model.ImageBlobName, _ImageFolder);

                    CreateImageAzure(model.ImageBlobName, image.InputStream, image.ContentType, _ImageFolder);
                }

                await _customerSiteCommerceClientService.EditDebitCommerceServiceId(new CustomerSiteCommerceDto
                {
                    Id = model.Id,
                    ServiceId = model.ServiceId,
                    ImageName = model.ImageName
                });

                ShowNotification(PresentationAdminStrings.CustomerSite_Success, NotificationType.Success);

                //REMOVE COMMERCES FROM CATCHE
                await _debitCommerceClientService.UpdateCommerceDebitCatche();

                return RedirectToAction("Index");
            }
            catch (WebApiClientBusinessException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException ex)
            {
                ShowNotification(ex.Message, NotificationType.Error);
            }
            catch (BusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (Exception)
            {
                ShowNotification(PresentationAdminStrings.Error_General_Model, NotificationType.Error);
            }
            var commerce = await _customerSiteCommerceClientService.Find(model.Id);
            await LoadViewBagData(model.ServiceId);
            return View(commerce.ToCommerceModel());
        }

        [CustomAuthentication(Actions.DebitCommerceList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerCommercesDebit(Request, param);

            var commerces = await _customerSiteCommerceClientService.GetCommercesDebit(filter);
            var commercesCount = await _customerSiteCommerceClientService.GetCommercesDebitCount(filter);
            var servicesVnp = await _serviceClientService.GetServicesLigthWithoutChildens(Guid.Empty);

            var dataModel = commerces.Select(d => new
            {
                Id = d.Id,
                Name = d.Name,
                ServiceId = d.ServiceId,
                Container = d.ServiceId != null && d.ServiceId != Guid.Empty.ToString() ?
                    servicesVnp.FirstOrDefault(x => x.Id == Guid.Parse(d.ServiceId)) != null ?
                        servicesVnp.FirstOrDefault(x => x.Id == Guid.Parse(d.ServiceId)).Container ? 1 : 0 : 0 : 0,
                ProductsCount = d.ProductosListDto != null ? d.ProductosListDto.Count : 0,
                CommerceCathegoryName = d.CommerceCathegoryDto != null ? d.CommerceCathegoryDto.Name : string.Empty,
                ServiceName = d.ServiceId != null && d.ServiceId != Guid.Empty.ToString() ?
                    servicesVnp.FirstOrDefault(x => x.Id == Guid.Parse(d.ServiceId)) != null ?
                        servicesVnp.FirstOrDefault(x => x.Id == Guid.Parse(d.ServiceId)).Name : string.Empty : string.Empty,
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = commercesCount,
                iTotalDisplayRecords = commercesCount,
                aaData = dataModel
            }, JsonRequestBehavior.AllowGet);
        }

        public async Task<bool> LoadViewBagData(string serviceId)
        {
            ViewBag.ServicesLigthList = await GetServicesLightWithoutChildens(serviceId);
            return true;
        }

        private async Task<List<SelectListItem>> GetServicesLightWithoutChildens(string serviceId)
        {
            var services = await _serviceClientService.GetServicesLigthWithoutChildens(Guid.Empty);
            var id = string.IsNullOrEmpty(serviceId) ? Guid.Empty : Guid.Parse(serviceId);
            var aux = services.Select(dto => new SelectListItem() { Value = dto.Id.ToString(), Text = dto.Name, Selected = dto.Id == id });
            var list = new List<SelectListItem>() { new SelectListItem() { Value = Guid.Empty.ToString(), Text = string.Empty } };
            list.AddRange(aux);
            return list;
        }

        private bool IsImage(HttpPostedFileBase file)
        {
            var formats = new[] { ".jpg", ".png", ".gif", ".jpeg" };
            return file.ContentType.Contains("image") && formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsSizeCorrect(HttpPostedFileBase file)
        {
            int byteCount = file.ContentLength;
            return byteCount < 5242880; //5MB
        }

    }
}