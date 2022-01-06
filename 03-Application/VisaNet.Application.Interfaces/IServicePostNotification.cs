using System.Collections.Generic;
using System.Net;

namespace VisaNet.Application.Interfaces
{
    public interface IServicePostNotification
    {
        HttpStatusCode NotifyExternalSourcePostWithSignature(string url, string signature,
            string sigantureField, IDictionary<string, string> signedFields);
    }
}