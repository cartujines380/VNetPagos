using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class PromotionController : BaseController
    {
        private readonly IPromotionClientService _promotionClientService;
        private readonly ISharingFileClientService _sharingFileClientService;
        private readonly string _ImageFolder = ConfigurationManager.AppSettings["AzurePromotionsImageFolder"];

        public PromotionController(IPromotionClientService promotionClientService, ISharingFileClientService sharingFileClientService)
        {
            _promotionClientService = promotionClientService;
            _sharingFileClientService = sharingFileClientService;
        }

        [HttpGet]
        [CustomAuthentication(Actions.PromotionList)]
        public async Task<ActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        [CustomAuthentication(Actions.PromotionCreate)]
        public async Task<ActionResult> Create()
        {
            try
            {
                return View(new PromotionModel());
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
        [CustomAuthentication(Actions.PromotionCreate)]
        public async Task<ActionResult> Create(PromotionModel promotion)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(promotion);
                }

                promotion.Id = Guid.NewGuid();
                HttpPostedFileBase imagePromo = null;
                
                foreach (string file in Request.Files)
                {
                    var image = Request.Files[file];
                    if (image != null && !String.IsNullOrEmpty(image.FileName))
                    {
                        if (!IsImage(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_type, NotificationType.Error);
                            return View(promotion);
                        }
                        if (!IsSizeCorrect(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_size_500000, NotificationType.Error);
                            return View(promotion);
                        }
                        if (file.Equals("ImageName"))
                        {
                            promotion.ImageName = image.FileName;                            
                            imagePromo = image;
                            CreateImageAzure(promotion.ImageBlobName, imagePromo.InputStream, imagePromo.ContentType, _ImageFolder);
                        }
                    }
                }
                if (string.IsNullOrEmpty(promotion.ImageName))
                {
                    ShowNotification(PresentationAdminStrings.Image_required, NotificationType.Alert);
                    return View(promotion);
                }

                await _promotionClientService.Create(promotion.ToDto());
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

            return View(promotion);
        }

        [HttpPost]
        [CustomAuthentication(Actions.PromotionDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var promo = await _promotionClientService.Find(id);
                await _promotionClientService.Delete(id);
                try
                {
                    if (!string.IsNullOrEmpty(promo.ImageName))
                    {
                        DeleteImageAzure(promo.ImageName, _ImageFolder);
                    }
                }
                catch (Exception)
                {
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
        [CustomAuthentication(Actions.PromotionEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var dto = await _promotionClientService.Find(id);
                var model = dto.ToModel();
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
        [CustomAuthentication(Actions.PromotionEdit)]
        public async Task<ActionResult> Edit(PromotionModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ShowNotification(PresentationAdminStrings.Error_General_Model, NotificationType.Error);
                    return View(model);
                }

                //si hay imagenes nuevas
                foreach (string file in Request.Files)
                {
                    var image = Request.Files[file];
                    if (image != null && !String.IsNullOrEmpty(image.FileName))
                    {
                        if (!IsImage(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_type, NotificationType.Error);
                            return View(model);
                        }
                        if (!IsSizeCorrect(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_size_500000, NotificationType.Error);
                            return View(model);
                        }
                        if (file.Equals("ImageName"))
                        {
                            model.ImageName = image.FileName;
                            DeleteImageAzure(model.ImageBlobName, _ImageFolder);
                            CreateImageAzure(model.ImageBlobName, image.InputStream, image.ContentType, _ImageFolder);
                        }
                    }
                }

                await _promotionClientService.Edit(model.ToDto());
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

        [CustomAuthentication(Actions.PromotionList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerPromotion(Request, param);

            var data = await _promotionClientService.FindAll(filter);

            var dataModel = data.Select(d => new
            {
                Id = d.Id,
                Name = d.Name,
                State = d.Active
            });

            if (filter.SortDirection == SortDirection.Desc) //invertido porque ordenaba primero al reves
            {
                dataModel = dataModel.OrderByDescending(p => filter.OrderBy == "0" ? p.Name :
                                                    filter.OrderBy == "2" ? (p.State ? "Activa" : "Inactiva") : "");
            }
            else
            {
                dataModel = dataModel.OrderBy(p => filter.OrderBy == "0" ? p.Name :
                                                    filter.OrderBy == "2" ? (p.State ? "Activa" : "Inactiva") : "");
            }

            var dataToShow = dataModel.Skip(filter.DisplayStart);

            if (filter.DisplayLength.HasValue)
                dataToShow = dataToShow.Take(filter.DisplayLength.Value);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = data.Count(),
                iTotalDisplayRecords = data.Count(),
                aaData = dataToShow.Select(d => new
                {
                    d.Id,
                    d.Name,
                    Active = d.State ? PresentationAdminStrings.Promotion_Active : PresentationAdminStrings.Promotion_Inactive
                }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CustomAuthentication(Actions.PromotionDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var dto = await _promotionClientService.Find(id);
                var model = dto.ToModel();
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

        private bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };

            // linq from Henrik Stenbæk
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsSizeCorrect(HttpPostedFileBase file)
        {
            int byteCount = file.ContentLength;
            return byteCount < 500000;
        }

    }
}