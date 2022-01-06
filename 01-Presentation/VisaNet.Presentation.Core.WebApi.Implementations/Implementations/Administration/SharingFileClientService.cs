using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class SharingFileClientService : WebApiClientService, ISharingFileClientService
    {
        public SharingFileClientService(ITransactionContext transactionContext)
            : base("FileSharing", transactionContext)
        {

        }

        public Task DeleteFile(String fileName)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestGet(BaseUri + "/DeleteFile", TransactionContext, new Dictionary<string, string> { { "fileName", fileName } }));
        }

        public Task PostFile(HttpPostedFileBase file, string fileName)
        {
            var obj = new SharedFileDto()
                      {
                          File = ImageToBase64String(Image.FromStream(file.InputStream)),
                          FileName = fileName
                      };
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri + "/PostFile", TransactionContext, obj));
        }

        /// <summary>
        /// Returns the base64 encoded string representation of the given image.
        /// </summary>
        /// <param name="image">A System.Drawing.Image to encode as a string.</param>
        string ImageToBase64String(Image image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, image.RawFormat);
                return Convert.ToBase64String(stream.ToArray());
            }
        }

        //System Versions
        public Task<IDictionary<string, string>> GetAllVersions()
        {
            return WebApiClient.CallApiServiceAsync<IDictionary<string, string>>(new WebApiHttpRequestGet(
                BaseUri + "/GetAllVersions", TransactionContext));
        }

    }
}
