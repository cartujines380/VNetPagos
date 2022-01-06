using System.Threading.Tasks;
using VisaNet.Common.Security.Mailgun;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class MailgunWebhookService : WebApiClientService, IMailgunWebhookService
    {
        public MailgunWebhookService(IMailgunTransactionContext transactionContext)
            : base("Email", transactionContext)
        {
        }

        public Task RegisterDelivery(string messageId, string recipient)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/RegisterDelivery", TransactionContext, new { MessageId = messageId, Recipient = recipient }));
        }

        public Task RegisterBounce(string messageId, string recipient)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/RegisterBounce", TransactionContext, new { MessageId = messageId, Recipient = recipient }));
        }

        public Task RegisterFailure(string messageId, string recipient, string description)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/RegisterFailure", TransactionContext, new { MessageId = messageId, Recipient = recipient, Description = description }));
        }
    }
}