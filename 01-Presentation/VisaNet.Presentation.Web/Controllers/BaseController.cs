using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using HXCaptcha;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Utilities.Notifications;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.Web.Controllers
{
    [OutputCache(NoStore = true, Duration = 0)]
    public class BaseController : Controller
    {
        protected void ClearSessionVariables()
        {
            Session.Abandon();
        }

        protected void ClearSessionVariablesFromCallCenter()
        {
            Session[SessionConstants.CURRENT_SELECTED_USER] = null;
            Session[SessionConstants.CURRENT_SELECTED_USER_ID] = null;
        }

        protected virtual void ShowNotification(string notification, NotificationType? type = null)
        {
            if (TempData[TempDataConstants.SHOW_NOTIFICATION] == null)
                TempData[TempDataConstants.SHOW_NOTIFICATION] = new List<Notification>();

            var title = string.Empty;

            switch (type)
            {
                case NotificationType.Success: title = PresentationCoreMessages.Notification_Title_Success; break;
                case NotificationType.Info: title = PresentationCoreMessages.Notification_Title_Info; break;
                case NotificationType.Error: title = PresentationCoreMessages.Notification_Title_Error; break;
                case NotificationType.Alert: title = PresentationCoreMessages.Notification_Title_Alert; break;
            }

            ((List<Notification>)TempData[TempDataConstants.SHOW_NOTIFICATION]).Add(new Notification
            {
                Text = notification,
                Title = title,
                Type = (type == null) ? NotificationType.Success : type.Value
            });
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);

                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        [HttpGet]
        public ActionResult GetCaptcha()
        {
            var captcha = HxCaptchaProvider.GetCaptcha();
            Session["_captchaAnswer"] = captcha.Answer;
            return File(captcha.GetBuffer(), "image/Gif");
        }

        public ServiceGatewayDto GetBestGateway(IEnumerable<ServiceGatewayDto> list)
        {

            var sublist = list.Where(g => g.Active).ToList();
            if (!sublist.Any()) return null;
            if (sublist.Count() == 1) return sublist.FirstOrDefault();

            return sublist.Any(g =>
                g.Gateway.Enum == (int)GatewayEnumDto.Banred) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Banred)
                : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc)
                    : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Sucive) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Sucive)
                        : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Geocom) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Geocom)
                            : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Carretera) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Carretera)
                                : null;
        }

        public string ServiceType(ServiceDto service)
        {
            if (service == null || service.ServiceGatewaysDto == null || !service.ServiceGatewaysDto.Any()) return "OTRO";

            if (service.ServiceGatewaysDto.FirstOrDefault(x => x.Active && x.Gateway.Enum == (int)GatewayEnum.Apps) != null)
                return "Apps";

            var gateway = service.ServiceGatewaysDto.FirstOrDefault(x => x.Active && x.Gateway.Enum == (int)GatewayEnum.Geocom);
            if (gateway == null) return "OTRO";

            if (gateway.ReferenceId.Equals("CIU"))
                return "CIU";
            if (gateway.ReferenceId.Equals("CON"))
                return "CON";

            return "OTRO";
        }

        protected virtual async Task<ApplicationUserDto> CurrentSelectedUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Session[SessionConstants.CURRENT_SELECTED_USER] == null)
                {
                    var user = await NinjectRegister.Get<IWebApplicationUserClientService>().Find(User.Identity.Name);
                    if (user == null)
                    {
                        return null;
                    }
                    SetCurrentSelectedUser(user);
                }
            }

            return (ApplicationUserDto)Session[SessionConstants.CURRENT_SELECTED_USER];
        }

        protected virtual AnonymousUserDto CurrentAnonymousUser()
        {
            var user = Session[SessionConstants.PAYMENT_DATA_ANONYMOUS_USER];
            if (user != null)
                return (AnonymousUserDto)user;

            return null;
        }

        protected virtual void SetCurrentSelectedUser(ApplicationUserDto user)
        {
            Session[SessionConstants.CURRENT_SELECTED_USER] = user;
            Session[SessionConstants.CURRENT_SELECTED_USER_ID] = user.Id;
        }

        protected virtual void SetCurrentAnonymousUser(AnonymousUserDto user)
        {
            Session[SessionConstants.PAYMENT_DATA_ANONYMOUS_USER_ID] = user.Id;
            Session[SessionConstants.PAYMENT_DATA_ANONYMOUS_USER] = user;
        }

        protected virtual async Task<SystemUserDto> CurrentUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (Session[SessionConstants.CURRENT_CALLCENTER_USER] == null)
                {
                    var user = await NinjectRegister.Get<IWebSystemUserClientService>().Find(User.Identity.Name);
                    Session[SessionConstants.CURRENT_CALLCENTER_USER] = user;
                    Session[SessionConstants.CURRENT_CALLCENTER_USER_ID] = user.Id;
                }
            }
            return (SystemUserDto)Session[SessionConstants.CURRENT_CALLCENTER_USER];
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (User.Identity.IsAuthenticated && Session[SessionConstants.CURRENT_SELECTED_USER] != null)
            {
                ViewData.Add("PrivateHeader", true);
            }
            else
            {
                ViewData.Add("PrivateHeader", false);
            }
            base.OnActionExecuting(filterContext);
        }

        public IDictionary<string, string> GenerateDictionary(NameValueCollection form)
        {
            //NLogLogger.LogEvent(NLogType.Info, "ARMO EL DICCIONARIO CON EL FROM ");
            var data = form.AllKeys.ToDictionary(key => key, key => form[key]);
            //foreach (var d in data)
            //{
            //    NLogLogger.LogEvent(NLogType.Info, "KEY: "+ d.Key +"VALUE: " +d.Value);
            //}

            return data;
        }

        public string GenereCsReturnPath(string absolute)
        {
            if (Request == null || Request.Url == null) return string.Empty;
            return string.Format("{0}://{1}/{2}", Request.Url.Scheme, Request.Url.Authority, absolute);
        }

        public string GetImageForService(ServiceDto serviceDto)
        {
            var strImage = string.Empty;

            if (!string.IsNullOrEmpty(serviceDto.ImageUrl))
            {
                //Si el servicio tiene imagen se la asigno
                strImage = serviceDto.ImageUrl;
            }
            if (string.IsNullOrEmpty(strImage))
            {
                //Si no tiene imagenl le asigno la de su servicio contenedor
                strImage = serviceDto.ServiceContainerImageUrl;
            }
            return strImage;
        }

        private static List<T> GetAllPublicConstantValues<T>(Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
                .Select(x => (T)x.GetRawConstantValue())
                .ToList();
        }

    }
}