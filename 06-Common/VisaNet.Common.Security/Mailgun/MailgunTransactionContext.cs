using System;
using System.Web;
using VisaNet.Common.Security.Helpers;

namespace VisaNet.Common.Security.Mailgun
{
    public class MailgunTransactionContext : IMailgunTransactionContext
    {
        const string CURRENT_USER = "SESSION_CURRENT_USER";
        const string CURRENT_USER_TYPE = "SESSION_CURRENT_USER_TYPE";
        const string CURRENT_USER_ID = "CURRENT_USER_ID";
        const string PAYMENT_DATA_ANONYMOUS_USER_ID = "PAYMENT_DATA_ANONYMOUS_USER_ID";

        public MailgunTransactionContext()
        {
            TransactionIdentifier = Guid.NewGuid();
            TransactionDateTime = DateTime.Now;
            UserName = "Mailgun";

            try { IP = HttpContext.Current.Request.GetUserIP(); }
            catch { IP = string.Empty; }

            try { RequestUri = HttpContext.Current != null ? HttpContext.Current.Request.Url.ToString() : string.Empty; }
            catch { RequestUri = string.Empty; }

            if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.Session != null)
            {
                if ((HttpContext.Current.Session[CURRENT_USER] != null) &&
                    (HttpContext.Current.Session[CURRENT_USER_TYPE] != null) &&
                    (HttpContext.Current.Session[CURRENT_USER_ID] != null))
                {
                    var userType = (int)HttpContext.Current.Session[CURRENT_USER_TYPE];

                    if (userType == 1) //callcenter
                    {
                        SystemUserId = ((Guid)HttpContext.Current.Session[CURRENT_USER_ID]).ToString();
                    }
                    else if (userType == 2) //public 
                    {
                        ApplicationUserId = ((Guid)HttpContext.Current.Session[CURRENT_USER_ID]).ToString();
                    }

                    SessionId = HttpContext.Current.Session.SessionID;
                }
            }
            else if (HttpContext.Current != null && (HttpContext.Current.User == null || HttpContext.Current.User.Identity.IsAuthenticated == false) && HttpContext.Current.Session != null)
            {
                if (HttpContext.Current.Session[PAYMENT_DATA_ANONYMOUS_USER_ID] != null)
                {
                    AnonymousUserId = ((Guid)HttpContext.Current.Session[PAYMENT_DATA_ANONYMOUS_USER_ID]).ToString();
                }
            }
        }

        public string UserName { get; private set; }
        public Guid TransactionIdentifier { get; private set; }
        public DateTime TransactionDateTime { get; set; }
        public string IP { get; private set; }
        public string RequestUri { get; set; }
        public string SystemUserId { get; set; }
        public string ApplicationUserId { get; set; }
        public string AnonymousUserId { get; set; }
        public string SessionId { get; private set; }
        public Guid TraceId { get; private set; }
    }
}
