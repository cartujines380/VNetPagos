using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.DependencyInjection;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Utilities.Notifications;
using NotificationType = VisaNet.Utilities.Notifications.NotificationType;

namespace VisaNet.Presentation.Web.Areas.CallCenter.Controllers
{
    public class BaseController : Controller
    {
        protected virtual async Task<SystemUserDto> CurrentUser()
        {
            if (Session[SessionConstants.CURRENT_CALLCENTER_USER] == null)
            {
                var user = await NinjectRegister.Get<IWebSystemUserClientService>().Find(User.Identity.Name);
                Session[SessionConstants.CURRENT_CALLCENTER_USER] = user;
                Session[SessionConstants.CURRENT_CALLCENTER_USER_ID] = user.Id;
                //Session[SessionConstants.CURRENT_USER_TYPE] = CurrentUserType.CallCenter;
            }

            return (SystemUserDto)Session[SessionConstants.CURRENT_CALLCENTER_USER];
        }

        protected virtual async Task<ApplicationUserDto> CurrentSelectedUser()
        {
            if (Session[SessionConstants.CURRENT_SELECTED_USER] == null && User.Identity.IsAuthenticated)
            {
                var user = await NinjectRegister.Get<IWebApplicationUserClientService>().Find(User.Identity.Name);
                if (user == null) return null;
                Session[SessionConstants.CURRENT_SELECTED_USER] = user;
            }
            return (ApplicationUserDto)Session[SessionConstants.CURRENT_SELECTED_USER];
        }

        protected virtual void SetCurrentSelectedUser(ApplicationUserDto user)
        {
            Session[SessionConstants.CURRENT_SELECTED_USER] = user;
        }

        protected void ClearSessionVariables()
        {
            Session[SessionConstants.CURRENT_CALLCENTER_USER] = null;
            Session[SessionConstants.CURRENT_USER_FUNCTIONALITY_GROUP] = null;
            Session[SessionConstants.CURRENT_USER_FUNCTIONALITIES] = null;
            Session[SessionConstants.CURRENT_USER_FUNCTIONALITIES_GROUP_ACTUAL] = null;
            Session[SessionConstants.CURRENT_USER_ENABLED_ACTIONS] = null;

            Session[SessionConstants.CURRENT_SELECTED_USER] = null;
            Session[SessionConstants.CURRENT_CALLCENTER_USER] = null;
            Session[SessionConstants.PAYMENT_DATA_ANONYMOUS_USER_ID] = null;
        }

        protected virtual void ShowNotification(string notification, NotificationType? type = null)
        {
            if (TempData[TempDataConstants.SHOW_NOTIFICATION] == null)
                TempData[TempDataConstants.SHOW_NOTIFICATION] = new List<Notification>();

            ((List<Notification>)TempData[TempDataConstants.SHOW_NOTIFICATION]).Add(new Notification
            {
                Text = notification,
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

        public ServiceGatewayDto GetBestGateway(IEnumerable<ServiceGatewayDto> list)
        {

            var sublist = list.Where(g => g.Active).ToList();
            if (!sublist.Any()) return null;
            if (sublist.Count() == 1) return sublist.FirstOrDefault();

            return sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Banred)
                ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Banred)
                : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc)
                    ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc)
                    : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Sucive)
                        ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Sucive)
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

        protected override ViewResult View(IView view, object model)
        {
            if (User.Identity.IsAuthenticated && Session[SessionConstants.CURRENT_SELECTED_USER] != null)
            {
                ViewData.Add("PrivateHeader", true);
            }
            else
            {
                ViewData.Add("PrivateHeader", false);
            }
            return base.View(view, model);
        }
    }
}