using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using VisaNet.Utilities.Cryptography;
using VisaNet.Utilities.Notifications;
using VisaNet.VONRegister.Constants;

namespace VisaNet.VONRegister.Controllers
{
    public class BaseController : Controller
    {

        protected void ShowToastr(string message, NotificationType type = NotificationType.Success)
        {
            if (TempData[TempDataConstants.ShowNotification] == null)
                TempData[TempDataConstants.ShowNotification] = new List<Notification>();

            ((List<Notification>)TempData[TempDataConstants.ShowNotification]).Add(new Notification
             {
                 Text = message,
                 Type = type
             });
        }

        protected void ClearSessionVariables()
        {
            Session[SessionConstants.CurrentService] = null;
            Session[SessionConstants.OperationId] = null;
            Session[SessionConstants.CurrentSelectedUser] = null;
            Session[SessionConstants.CallbackUrl] = null;
            Session[SessionConstants.RegisterModel] = null;
            Session[SessionConstants.HashedDomain] = null;
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

        protected IDictionary<string, string> GenerateDictionary(NameValueCollection form)
        {
            return form.AllKeys.ToDictionary(key => key, key => form[key]);
        }

        protected string GenerateHash(string domain)
        {
            return Md5Hash.GenerateHash(domain);
        }

        protected bool CheckHash(string domain, string hash)
        {
            return Md5Hash.ValidateHash(domain,hash);
        }
    }
}