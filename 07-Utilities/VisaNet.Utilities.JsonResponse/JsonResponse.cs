using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Utilities.JsonResponse
{
    public class JsonResponse
    {
        public AjaxResponse ResponseType { get; set; }
        public object Content { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public string NotificationType { get; set; }
        public JsonResult Chart { get; set; }
        public object Content2 { get; set; }

        public JsonResponse(AjaxResponse responseType, object content, string message, string title, NotificationType? notificationType = null, JsonResult chart = null, object content2 = null)
        {
            if (string.IsNullOrEmpty(title))
            {
                switch (notificationType)
                {
                    case Notifications.NotificationType.Success: title = PresentationCoreMessages.Notification_Title_Success; break;
                    case Notifications.NotificationType.Info: title = PresentationCoreMessages.Notification_Title_Info; break;
                    case Notifications.NotificationType.Error: title = PresentationCoreMessages.Notification_Title_Error; break;
                    case Notifications.NotificationType.Alert: title = PresentationCoreMessages.Notification_Title_Alert; break;
                    default: title = PresentationCoreMessages.Notification_Title_Success; break;
                }
            }

            ResponseType = responseType;
            Content = content;
            Message = message;
            Title = title;
            NotificationType = (notificationType == null) ? Notifications.NotificationType.Success.ToDescription() : notificationType.Value.ToDescription();
            Chart = chart;
            Content2 = content2;
        }
    }
}
