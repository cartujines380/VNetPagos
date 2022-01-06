using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Web;

namespace VisaNet.Presentation.Administration.Hubs
{
    public class NotificationHub : Hub
    {
        public void Send(string userId, string message, string type)
        {
            Clients.Group(userId).notify(message, type);
        }

        public override Task OnConnected()
        {
            JoinGroups();
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

        private void JoinGroups()
        {
            var userId = Context.Request.QueryString["userId"];

            if (!string.IsNullOrEmpty(userId))
            {
                var connectionId = Context.ConnectionId;
                Groups.Add(connectionId, userId);
            }
        }
    }
}