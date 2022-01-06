using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ParametersController : BaseController
    {
        private readonly IParametersClientService _parametersClientService;

        public ParametersController(IParametersClientService parametersClientService)
        {
            _parametersClientService = parametersClientService;
        }

        [CustomAuthentication(Actions.ParametersDetails)]
        public async Task<ActionResult> Details()
        {
            var parameters = await _parametersClientService.FindAll();

            return View(parameters.FirstOrDefault().ToModel());
        }

        [CustomAuthentication(Actions.ParametersEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var parameters = await _parametersClientService.Find(id);
                var model = parameters.ToModel();
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
            return RedirectToAction("Details");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.ParametersEdit)]
        public async Task<ActionResult> Edit(ParametersModel parameters)
        {
            try
            {
                await _parametersClientService.Edit(parameters.ToDto());
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

            return View(parameters);
        }

    }
}
