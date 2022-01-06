using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class LegalPagesController : BaseController
    {
        private readonly IPageClientService _pageClientService;

        public LegalPagesController(IPageClientService pageClientService)
        {
            _pageClientService = pageClientService;
        }

        [CustomAuthentication(Actions.LegalPagesDetails)]
        public async Task<ActionResult> Details()
        {
            var pages = await _pageClientService.FindAll();

            var page = pages.First(p => p.PageType == PageTypeDto.LegalPages);
            if (page.Content != null) page.Content = page.Content.Replace(Environment.NewLine, string.Empty);

            return View(page.ToModel());
        }

        [CustomAuthentication(Actions.LegalPagesEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var page = await _pageClientService.Find(id);

                if (page.Content != null) page.Content = page.Content.Replace(Environment.NewLine, string.Empty);

                return View(page.ToModel());
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            return RedirectToAction("Details");
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.LegalPagesEdit)]
        public async Task<ActionResult> Edit(PageModel page)
        {
            try
            {
                page.Content = page.Content.Replace("\"", "'");
                await _pageClientService.Edit(page.ToDto());
                ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);
                return RedirectToAction("Details");
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }

            if (page.Content != null) page.Content = page.Content.Replace(Environment.NewLine, string.Empty);
            return View(page);
        }

    }
}
