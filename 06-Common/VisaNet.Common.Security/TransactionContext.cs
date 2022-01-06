using System;
using System.Web;
using VisaNet.Common.Security.Helpers;

namespace VisaNet.Common.Security
{
    public class TransactionContext : ITransactionContext
    {
        const string PAYMENT_DATA_ANONYMOUS_USER_ID = "PAYMENT_DATA_ANONYMOUS_USER_ID";

        const string CURRENT_SELECTED_USER_ID = "CURRENT_SELECTED_USER_ID";
        const string CURRENT_CALLCENTER_USER_ID = "CURRENT_CALLCENTER_USER_ID";
        const string CURRENT_SYS_USER_ID = "CURRENT_SYS_USER_ID";
        const string TRACE_ID = "TRACE_ID";

        public TransactionContext()
        {
            TransactionIdentifier = Guid.NewGuid();
            TransactionDateTime = DateTime.Now;
            UserName = (HttpContext.Current == null || HttpContext.Current.User == null) ? string.Empty : HttpContext.Current.User.Identity.Name;

            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Application[TRACE_ID] == null)
                {
                    TraceId = Guid.NewGuid();
                    HttpContext.Current.Application.Add(TRACE_ID, TraceId);
                }
                else
                {
                    TraceId = Guid.Parse(HttpContext.Current.Application[TRACE_ID].ToString());
                }
            }

            try { IP = HttpContext.Current.Request.GetUserIP(); }
            catch { IP = string.Empty; }

            try { RequestUri = HttpContext.Current != null ? HttpContext.Current.Request.Url.ToString() : string.Empty; }
            catch { RequestUri = string.Empty; }

            if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.Session != null)
            {
                if ((HttpContext.Current.Session[CURRENT_SYS_USER_ID] != null))
                {
                    SystemUserId = ((Guid)HttpContext.Current.Session[CURRENT_SYS_USER_ID]).ToString();
                    SessionId = HttpContext.Current.Session.SessionID;
                }
                if ((HttpContext.Current.Session[CURRENT_CALLCENTER_USER_ID] != null))
                {
                    SystemUserId = ((Guid)HttpContext.Current.Session[CURRENT_CALLCENTER_USER_ID]).ToString();
                    SessionId = HttpContext.Current.Session.SessionID;
                }
                if ((HttpContext.Current.Session[CURRENT_SELECTED_USER_ID] != null))
                {
                    ApplicationUserId = ((Guid)HttpContext.Current.Session[CURRENT_SELECTED_USER_ID]).ToString();
                    SessionId = HttpContext.Current.Session.SessionID;
                }
                if ((HttpContext.Current.Session[PAYMENT_DATA_ANONYMOUS_USER_ID] != null))
                {
                    AnonymousUserId = ((Guid)HttpContext.Current.Session[PAYMENT_DATA_ANONYMOUS_USER_ID]).ToString();
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


    public class TestTransactionContext : ITransactionContext
    {
        public TestTransactionContext()
        {
            TransactionIdentifier = Guid.NewGuid();
            UserName = "Debug_Transaction_Context";
            IP = "";
            TraceId = Guid.NewGuid();
        }

        public string UserName { get; private set; }
        public Guid TransactionIdentifier { get; private set; }
        public DateTime TransactionDateTime { get; set; }
        public string IP { get; private set; }
        public string RequestUri { get; private set; }
        public string SystemUserId { get; private set; }
        public string ApplicationUserId { get; private set; }
        public string AnonymousUserId { get; set; }
        public string SessionId { get; private set; }
        public Guid TraceId { get; private set; }
    }

}