using System.ComponentModel;

namespace VisaNet.Utilities.Notifications
{
    public enum NotificationType
    {
        [Description("success")]
        Success,
        [Description("error")]
        Error,
        [Description("")]
        Alert,
        [Description("info")]
        Info
    }
}
