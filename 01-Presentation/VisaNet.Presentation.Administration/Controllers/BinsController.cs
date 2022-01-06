using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Models;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Handlers;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Helpers;
using VisaNet.Utilities.JsonResponse;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class BinsController : BaseController
    {
        private readonly IGatewayClientService _gatewayClientService;
        private readonly IBinsClientService _binClientService;
        private readonly string _IsoUruguay = "UY";
        private readonly IAffiliationCardClientService _affiliationCardClientService;
        private readonly string _ImageFolder = ConfigurationManager.AppSettings["AzureBinsImageFolder"];

        public BinsController(IGatewayClientService gatewayClientService, IBinsClientService binClientService,
            IAffiliationCardClientService affiliationCardClientService)
        {
            _gatewayClientService = gatewayClientService;
            _binClientService = binClientService;
            _affiliationCardClientService = affiliationCardClientService;
        }

        private async Task LoadViewBag(Guid? selectedId, Guid? bankId, string countryIso, Guid? affiliationCardId)
        {
            var gateways = await _gatewayClientService.FindAll();
            var list = gateways.Select(dto => new SelectListItem() { Text = dto.Name, Value = dto.Id.ToString(), Selected = selectedId == dto.Id }).ToList();
            ViewBag.Gateway = list;

            var banks = await GetBanks();
            var listBanks = banks.Select(dto => new SelectListItem() { Text = dto.Name, Value = dto.Id.ToString(), Selected = bankId == dto.Id }).ToList();
            ViewBag.Banks = listBanks;

            ViewBag.Countries = GetCountriesIsoList(countryIso);

            var affiliationCardsList = await GetAffiliationCard();
            var listAffiliationCard = affiliationCardsList.OrderBy(x => x.Name).Select(dto => new SelectListItem()
            {
                Text = dto.Name + ", Emisor: " + (dto.BankDto != null ? dto.BankDto.Name : string.Empty) + ", Estado: " + (dto.Active ? "Activo" : "Inactivo"),
                Value = dto.Id.ToString(),
                Selected = affiliationCardId == dto.Id
            }).ToList();
            ViewBag.AffiliationCard = listAffiliationCard;
        }

        [HttpGet]
        [CustomAuthentication(Actions.BinsList)]
        public async Task<ActionResult> Index()
        {
            try
            {

                return View();
            }
            catch (WebApiClientBusinessException exception)
            {

                throw;
            }
            catch (WebApiClientFatalException exception)
            {
                throw;
            }
            return null;
        }

        [HttpGet]
        [CustomAuthentication(Actions.BinsCreate)]
        public async Task<ActionResult> Create()
        {
            try
            {
                await LoadViewBag(null, null, string.Empty, null);
                var model = new BinModel()
                {
                    BinAuthorizationAmountModelList = GetBinAuthorizationAmountModelList(),
                    Country = _IsoUruguay
                };
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
        [CustomAuthentication(Actions.BinsCreate)]
        public async Task<ActionResult> Create(BinModel service)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBag(null, null, service.Country, null);
                    return View(service);
                }

                var binId = Guid.NewGuid();
                HttpPostedFileBase imageBin = null;
                string imageName = null;

                foreach (string file in Request.Files)
                {
                    var image = Request.Files[file];
                    if (image != null && !string.IsNullOrEmpty(image.FileName))
                    {
                        if (!IsImage(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_type, NotificationType.Error);
                            await LoadViewBag(null, null, service.Country, service.AffiliationCardId);
                            return View(service);
                        }
                        if (!IsSizeCorrect(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_size, NotificationType.Error);
                            await LoadViewBag(null, null, service.Country, service.AffiliationCardId);
                            return View(service);
                        }
                        if (!IsProportionCorrect(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_proportion, NotificationType.Error);
                            await LoadViewBag(null, null, service.Country, service.AffiliationCardId);
                            return View(service);
                        }
                        if (file.Equals("ImageName"))
                        {
                            service.ImageName = image.FileName;
                            imageBin = image;
                        }
                    }
                }

                var dto = service.ToDto();
                dto.Id = binId;
                await _binClientService.Create(dto);

                if (imageBin != null)
                {
                    imageBin.InputStream.Seek(0, SeekOrigin.Begin);
                    CreateImageAzure(service.ImageBlobName, imageBin.InputStream, imageBin.ContentType, _ImageFolder);
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
            await LoadViewBag(null, null, service.Country, service.AffiliationCardId);
            return View(service);
        }

        [HttpPost]
        [CustomAuthentication(Actions.BinsDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var bin = await _binClientService.Find(id);
                await _binClientService.Delete(id);
                try
                {
                    if (!string.IsNullOrEmpty(bin.ImageName))
                    {
                        DeleteImageAzure(bin.ImageName, _ImageFolder);
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
        [CustomAuthentication(Actions.BinsEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var dto = await _binClientService.Find(id);
                await LoadViewBag(dto.GatewayId, dto.BankDtoId, dto.Country, dto.AffiliationCardId);
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
        [CustomAuthentication(Actions.BinsEdit)]
        public async Task<ActionResult> Edit(BinModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadViewBag(model.GatewayId, model.BankId, model.Country, model.AffiliationCardId);
                    return View(model);
                }

                //si quiso borrar la imagen
                if (model.DeleteImage)
                {
                    DeleteImageAzure(model.ImageBlobName, _ImageFolder);
                    model.ImageName = null;
                }

                //si hay imagenes nuevas
                foreach (string file in Request.Files)
                {
                    var image = Request.Files[file];
                    if (image != null && !string.IsNullOrEmpty(image.FileName))
                    {
                        if (!IsImage(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_type, NotificationType.Error);
                            await LoadViewBag(model.GatewayId, model.BankId, model.Country, model.AffiliationCardId);
                            return View(model);
                        }
                        if (!IsSizeCorrect(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_size, NotificationType.Error);
                            await LoadViewBag(model.GatewayId, model.BankId, model.Country, model.AffiliationCardId);
                            return View(model);
                        }
                        if (!IsProportionCorrect(image))
                        {
                            ShowNotification(PresentationAdminStrings.Image_wrong_proportion, NotificationType.Error);
                            await LoadViewBag(model.GatewayId, model.BankId, model.Country, model.AffiliationCardId);
                            return View(model);
                        }
                        if (file.Equals("ImageName"))
                        {

                            image.InputStream.Seek(0, SeekOrigin.Begin);
                            model.ImageName = image.FileName;
                            DeleteImageAzure(model.ImageBlobName, _ImageFolder);
                            CreateImageAzure(model.ImageBlobName, image.InputStream, image.ContentType, _ImageFolder);
                        }
                    }
                }
                var dto = model.ToDto();
                await _binClientService.Edit(dto);

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

            await LoadViewBag(model.GatewayId, model.BankId, model.Country, model.AffiliationCardId);
            return View(model);
        }

        [CustomAuthentication(Actions.BinsList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var filter = AjaxHandlers.AjaxHandlerBin(Request, param);

            var data = await _binClientService.GetDataForTable(filter);
            var count = await _binClientService.GetDataForTableCount(filter);

            var dataToShow = data.Select(d => new
            {
                Id = d.Id,
                Name = d.Name,
                Value = d.Value.ToString().PadLeft(6, '0'),
                GatewayName = d.GatewayName,
                CardType = d.CardType,
                StatusActive = d.Active,
                Country = d.Country,
                Bank = (d.BankDto == null) ? "" : d.BankDto.Name,
                BankId = (d.BankDto == null) ? new Guid() : d.BankDto.Id,
                AfiliationCardName = d.AffiliationCardDto != null ? d.AffiliationCardDto.Name : string.Empty,
                Status = d.Active ? "Habilitado" : "Bloqueado",
            });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = count,
                iTotalDisplayRecords = count,
                aaData = dataToShow.Select(b => new
                {
                    b.Id,
                    b.Name,
                    b.Value,
                    b.GatewayName,
                    b.Country,
                    b.StatusActive,
                    b.Status,
                    b.Bank,
                    CardType = EnumHelpers.GetName(typeof(CardTypeDto), (int)b.CardType, EnumsStrings.ResourceManager),
                    b.AfiliationCardName
                }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [CustomAuthentication(Actions.BinsDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            try
            {
                var dto = await _binClientService.Find(id);
                await LoadViewBag(dto.GatewayId, dto.BankDtoId, dto.Country, dto.AffiliationCardId);
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

        private async Task<List<BankDto>> GetBanks()
        {
            var banks = new List<BankDto>(){new BankDto()
            {
                Name = "OTRO"
            }};
            banks.AddRange(await _binClientService.GetBanks());
            return banks;
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

        private bool IsProportionCorrect(HttpPostedFileBase file)
        {
            var image = Image.FromStream(file.InputStream, true, true);
            return image.Width == 65 && image.Height == 35;
        }

        [HttpPost]
        [CustomAuthentication(Actions.BinsEdit)]
        public async Task<ActionResult> ChangeState(Guid id)
        {
            try
            {
                await _binClientService.ChangeStatus(id);
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

        public List<SelectListItem> GetCountriesIsoList(string key)
        {
            List<SelectListItem> result;
            var fileName = ConfigurationManager.AppSettings["CountriesIsoList"];
            var path = Path.Combine(CurrentDirectoryPath.AssemblyDirectory, "Models", "Json", fileName);

            using (var r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                var countries = JsonConvert.DeserializeObject<IEnumerable<CountryIsoModel>>(json);
                result = countries.Select(dto => new SelectListItem() { Text = dto.Value, Value = dto.Key.ToString(), Selected = key == dto.Key }).ToList();
            }
            return result;
        }

        private List<BinAuthorizationAmountModel> GetBinAuthorizationAmountModelList()
        {
            return new List<BinAuthorizationAmountModel>()
            {
                new BinAuthorizationAmountModel()
                {
                    LawDto = DiscountTypeDto.FinancialInclusion,
                    Label = ModelsStrings.Bin_AuthorizationAmountType + " " + ModelsStrings.Bin_FinancialInclusion,
                    Id = Guid.NewGuid(),
                },
                new BinAuthorizationAmountModel()
                {
                    LawDto = DiscountTypeDto.TourismOrTaxReintegration,
                    Label = ModelsStrings.Bin_AuthorizationAmountType + " " + ModelsStrings.Bin_TourismOrTaxReintegration,
                    Id = Guid.NewGuid(),
                },
            };
        }

        private async Task<IList<AffiliationCardDto>> GetAffiliationCard()
        {
            var list = await _affiliationCardClientService.GetDataForTable(new AffiliationCardFilterDto()
            {
                DisplayLength = 1000000,
                OrderBy = "Id"
            });
            return list.ToList();
        }

    }
}