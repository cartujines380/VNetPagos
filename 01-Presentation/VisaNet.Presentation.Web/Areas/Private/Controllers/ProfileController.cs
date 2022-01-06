using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Areas.Private.Models;
using VisaNet.Presentation.Web.Controllers;
using VisaNet.Presentation.Web.Mappers;
using VisaNet.Presentation.Web.Models;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Web.Areas.Private.Controllers
{
    [CustomAuthorize]
    public class ProfileController : BaseController
    {

        private readonly IWebApplicationUserClientService _applicationUserClientService;
        private readonly IWebSubscriberClientService _subscriberClientService;

        public ProfileController(IWebApplicationUserClientService applicationUserClientService, IWebSubscriberClientService subscriberClientService)
        {
            _applicationUserClientService = applicationUserClientService;
            _subscriberClientService = subscriberClientService;
        }

        //
        // GET: /Privado/Profile/
        public async Task<ActionResult> Index()
        {
            var user = await CurrentSelectedUser();

            if (user == null) return View(new ProfileModel());

            var dto = await _applicationUserClientService.Find(user.Email);
            
            return View(dto.ToModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProfileModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var logOff = !model.Email.Equals(model.OldEmail);
                    await _applicationUserClientService.Edit(model.ToDto());

                    if (logOff)
                    {
                        await _subscriberClientService.DeleteByEmail(model.OldEmail); //si cambio el mail borro el anterior
                        ShowNotification(PresentationWebStrings.Profile_Edit_Success_EmilChanged, NotificationType.Success);
                        return RedirectToAction("LogOff", "Account", new RouteValueDictionary() { { "Area", "" } });
                    }
                    ShowNotification(PresentationWebStrings.Profile_Edit_Success, NotificationType.Success);
                    return RedirectToAction("Index", "Dashboard", new RouteValueDictionary() { { "Area", "Private" } });
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

            return View("Index", model);
        }

        #region Password
        public async Task<ActionResult> ChangePassword()
        {
            try
            {
                var currentUser = await CurrentSelectedUser();
                ViewBag.Email = currentUser.Email;

                return View();
            }
            catch (WebApiClientBusinessException exception)
            {
                ShowNotification(exception.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException exception)
            {
                ShowNotification(exception.Message, NotificationType.Error);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var currentUser = await CurrentSelectedUser();
                    await _applicationUserClientService.ChangePassword(currentUser.Id, currentUser.Email,
                        model.OldPassword, model.NewPassword);

                    ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);
                    return RedirectToAction("Index");
                }
            }
            catch (WebApiClientBusinessException exception)
            {
                ShowNotification(exception.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException exception)
            {
                ShowNotification(exception.Message, NotificationType.Error);
            }

            var user = await CurrentSelectedUser();
            ViewBag.Email = user.Email;
            return View();
        }
        #endregion

    }
}