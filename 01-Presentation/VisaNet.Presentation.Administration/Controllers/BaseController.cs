using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using VisaNet.Common.AzureUpload;
using VisaNet.Common.Exceptions;
using VisaNet.Presentation.Administration.Constants;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class BaseController : Controller
    {
        protected void ClearSessionVariables()
        {
            Session[SessionConstants.CURRENT_USER] = null;
            Session[SessionConstants.CURRENT_USER_FUNCTIONALITY_GROUP] = null;
            Session[SessionConstants.CURRENT_USER_FUNCTIONALITIES] = null;
            Session[SessionConstants.CURRENT_USER_FUNCTIONALITIES_GROUP_ACTUAL] = null;
            Session[SessionConstants.CURRENT_USER_ENABLED_ACTIONS] = null;
        }

        protected virtual void ShowNotification(string notification, NotificationType? type = null, string title = null)
        {
            if (TempData[TempDataConstants.SHOW_NOTIFICATION] == null)
                TempData[TempDataConstants.SHOW_NOTIFICATION] = new List<Notification>();

            ((List<Notification>)TempData[TempDataConstants.SHOW_NOTIFICATION]).Add(new Notification
            {
                Text = notification,
                Type = (type == null) ? NotificationType.Success : type.Value,
                Title = title
            });
        }

        protected string RenderPartialViewToString(string viewName)
        {
            return RenderPartialViewToString(viewName, null);
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

        public string ShortenPath(string originalPath)
        {
            string fileName = Path.GetFileName(originalPath);
            return fileName;
        }

        public class NumericStringSort : IComparer<string>
        {
            public int Compare(string a, string b)
            {
                decimal aDec;
                decimal bDec;
                if (decimal.TryParse(a, out aDec) && decimal.TryParse(b, out bDec))
                {
                    return aDec.CompareTo(bDec);
                }
                return System.String.Compare(a, b, StringComparison.InvariantCultureIgnoreCase);
            }

            public static IComparer NumericStringSorter()
            {
                return (IComparer)new NumericStringSort();
            }
        }

        //Azure methods
        protected void CreateImageAzure(string filename, Stream image, string contentType, string folder = null)
        {
            try
            {
                FileStorage.Instance.UploadImage(image, folder, filename, contentType);
            }
            catch (BusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
        }

        protected void DeleteImageAzure(string filename, string folder = null)
        {
            try
            {
                FileStorage.Instance.DeleteImage(folder, filename);
            }
            catch (BusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
        }

        protected void CreateFileAzure(string folder, string filename, byte[] filebuffer, string contentType)
        {
            try
            {
                FileStorage.Instance.UploadFile(BlobAccessType.Blob, filebuffer, folder, filename, contentType);
            }
            catch (BusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
        }

        protected void DeleteFileAzure(string filename, string folder)
        {
            try
            {
                FileStorage.Instance.DeleteFile(folder, filename);
            }
            catch (BusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
        }

        protected FileStreamResult DownloadFileAzure(string filename, string folder, string newFileName = null)
        {
            newFileName = string.IsNullOrEmpty(newFileName) ? filename : newFileName;
            var stream = FileStorage.Instance.DownloadFile(folder, filename, BlobAccessType.Blob);
            return File(stream, "application/txt", newFileName);
        }

    }
}