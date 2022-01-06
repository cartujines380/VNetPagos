using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisaNet.Domain.EntitiesDtos.ExternalRequest
{
    public class AccessTokenFilterDto
    {
        public string Token { get; set; }
        public Guid AccessTokenId { get; set; }
        public Guid WebhookRegistrationId { get; set; }

        public IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"Token", Token},
                {"WebhookRegistrationId", WebhookRegistrationId.ToString()},
                {"AccessTokenId", AccessTokenId.ToString()},
            };
        } 
    }
}
