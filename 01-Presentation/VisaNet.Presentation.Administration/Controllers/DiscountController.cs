using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Models;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class DiscountController : BaseController
    {
        private readonly IDiscountClientService _discountClientService;

        public DiscountController(IDiscountClientService discountClientService)
        {
            _discountClientService = discountClientService;
        }

        [HttpGet]
        [CustomAuthentication(Actions.DiscountList)]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthentication(Actions.DiscountDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            var discount = await _discountClientService.Find(id);

            return View(discount.ToModel());
        }

        [CustomAuthentication(Actions.DiscountEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                ViewBag.Discounts = GenerateDiscountTypeList();
                ViewBag.DiscountsLabels = GenerateDiscountLabelTypeList();

                var discount = await _discountClientService.Find(id);

                return View(discount.ToModel());
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
        [CustomAuthentication(Actions.DiscountEdit)]
        public async Task<ActionResult> Edit(DiscountModel discount)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _discountClientService.Edit(discount.ToDto());
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
            }

            ViewBag.Discounts = GenerateDiscountTypeList();
            ViewBag.DiscountsLabels = GenerateDiscountLabelTypeList();

            return View(discount);
        }

        [CustomAuthentication(Actions.DiscountList)]
        public async Task<ActionResult> AjaxHandler(JQueryDataTableParamModel param)
        {
            var data = await _discountClientService.FindAll();

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            var sortDirection = Request["sSortDir_0"]; // asc or desc

            switch (sortColumnIndex)
            {
                case 0:
                    data = sortDirection == "asc" ? data.OrderBy(x => x.CardType).ToList() : data.OrderByDescending(x => x.CardType).ToList();
                    break;
                case 1:
                    data = sortDirection == "asc" ? data.OrderBy(x => x.MaximumAmount).ToList() : data.OrderByDescending(x => x.MaximumAmount).ToList();
                    break;
                case 2:
                    data = sortDirection == "asc" ? data.OrderBy(x => x.From).ToList() : data.OrderByDescending(x => x.From).ToList();
                    break;
                case 3:
                    data = sortDirection == "asc" ? data.OrderBy(x => x.To).ToList() : data.OrderByDescending(x => x.To).ToList();
                    break;
                case 4:
                    data = sortDirection == "asc" ? data.OrderBy(x => x.Fixed).ToList() : data.OrderByDescending(x => x.Fixed).ToList();
                    break;
                case 5:
                    data = sortDirection == "asc" ? data.OrderBy(x => x.Additional).ToList() : data.OrderByDescending(x => x.Additional).ToList();
                    break;
                case 6:
                    data = sortDirection == "asc" ? data.OrderBy(x => x.DiscountType).ToList() : data.OrderByDescending(x => x.DiscountType).ToList();
                    break;
                case 7:
                    data = sortDirection == "asc" ? data.OrderBy(x => x.DiscountLabel).ToList() : data.OrderByDescending(x => x.DiscountLabel).ToList();
                    break;
                default:
                    data = sortDirection == "asc" ? data.OrderBy(x => x.CardType).ToList() : data.OrderByDescending(x => x.CardType).ToList();
                    break;
            }

            var dataToShow = data.Skip(param.iDisplayStart);
            dataToShow = dataToShow.Take(param.iDisplayLength);

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = data.Count(),
                iTotalDisplayRecords = data.Count(),
                aaData = dataToShow.Select(d => new
                {
                    d.Id,
                    CardType = EnumHelpers.GetName(typeof(CardTypeDto), (int)d.CardType, EnumsStrings.ResourceManager),
                    From = d.From.ToShortDateString(),
                    To = d.To.ToShortDateString(),
                    d.Fixed,
                    d.Additional,
                    d.MaximumAmount,
                    DiscountType = EnumHelpers.GetName(typeof(DiscountTypeDto), (int)d.DiscountType, EnumsStrings.ResourceManager),
                    DiscountLabel = EnumHelpers.GetName(typeof(DiscountLabelTypeDto), (int)d.DiscountLabel, EnumsStrings.ResourceManager)
                }).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        private List<SelectListItem> GenerateDiscountTypeList()
        {
            var rm = ModelsStrings.ResourceManager;

            var list = Enum.GetValues(typeof(DiscountType)).Cast<DiscountType>();

            return list.Select(discountType => new SelectListItem
            {
                Text = rm.GetString("DiscountType_" + discountType.ToString()),
                Value = (int)discountType + "",
            }).Where(x => !x.Value.Equals("0") && !x.Value.Equals("5")).ToList();
        }

        private List<SelectListItem> GenerateDiscountLabelTypeList()
        {
            var rm = ModelsStrings.ResourceManager;

            var list = Enum.GetValues(typeof(DiscountLabelType)).Cast<DiscountLabelType>();

            return list.Select(discountLabel => new SelectListItem
            {
                Text = rm.GetString("DiscountLabel_" + discountLabel.ToString()),
                Value = (int)discountLabel + "",
            }).Where(x => !x.Value.Equals("0") && !x.Value.Equals("5")).ToList();
        }

    }
}