using System.Threading.Tasks;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IMailgunWebhookService
    {
        Task RegisterDelivery(string messageId, string recipient);
        Task RegisterBounce(string messageId, string recipient);
        Task RegisterFailure(string messageId, string recipient, string description);
    }
}