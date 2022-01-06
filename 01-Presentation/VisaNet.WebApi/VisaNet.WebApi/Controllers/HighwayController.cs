using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using VisaNet.Common.AzureUpload;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;

namespace VisaNet.WebApi.Controllers
{
    public class HighwayController : ApiController
    {
        private readonly IWebHighwayClientService _highwayClientService;

        private readonly string _highwayBlobFolder = "highway";

        public HighwayController(IWebHighwayClientService highwayClientService)
        {
            _highwayClientService = highwayClientService;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "Alguien invoco metodo save file POST");
            try
            {
                var fileName = string.Empty;

                HttpRequestMessage request = this.Request;

                if (!request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                var provider = new MultipartMemoryStreamProvider();

                await Request.Content.ReadAsMultipartAsync(provider);

                var timestpam = await provider.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Trim('\"').Equals("timestamp")).ReadAsStringAsync();
                var token = await provider.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Trim('\"').Equals("token")).ReadAsStringAsync();
                var signature = await provider.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Trim('\"').Equals("signature")).ReadAsStringAsync();

                var signatureOk = VerifySignature(ConfigurationManager.AppSettings["MailgunApiKey"], token, timestpam, signature);

                if (!signatureOk)
                {
                    NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, string.Format("Error en la firma. Timestpam: {0}, Token: {1}, Signature:{2}", timestpam, token, signature));
                    //return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable };
                }

                var email = new HighwayEmailDto
                {
                    Sender = await provider.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Trim('\"').Equals("sender")).ReadAsStringAsync(),
                    RecipientEmail = await provider.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Trim('\"').Equals("recipient")).ReadAsStringAsync(),
                    Subject = await provider.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Trim('\"').Equals("subject")).ReadAsStringAsync(),
                    TimeStampSeconds = await provider.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Trim('\"').Equals("timestamp")).ReadAsStringAsync(),
                    Status = HighwayEmailStatusDto.RejectedFileNotFound
                };

                var attachmentCount = int.Parse(await provider.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Trim('\"').Equals("attachment-count")).ReadAsStringAsync());
                byte[] highwayFile = null;

                for (int i = 1; i <= attachmentCount; i++)
                {
                    var file = provider.Contents.FirstOrDefault(x => x.Headers.ContentDisposition.Name.Trim('\"').Equals("attachment-" + i));

                    if (file != null)
                    {
                        fileName = file.Headers.ContentDisposition.FileName.Trim('\"');
                        NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "Llega el archivo " + fileName);

                        email.AttachmentInputName = fileName;

                        var name = fileName.Split('_');

                        if (!name[0].ToUpper().Equals("ENVIO", StringComparison.InvariantCultureIgnoreCase) ||
                            !name[1].ToUpper().Equals("VNP", StringComparison.InvariantCultureIgnoreCase) ||
                            name.Count() != 5)
                        {
                            email.Status = HighwayEmailStatusDto.RejectedFileNameBadlyFormed;
                        }
                        else
                        {
                            highwayFile = await file.ReadAsByteArrayAsync();

                            FileStorage.Instance.DeleteFile(_highwayBlobFolder, fileName);
                            FileStorage.Instance.UploadFile(BlobAccessType.Blob, highwayFile, _highwayBlobFolder, fileName, "text/plain");

                            NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "Se subio al Blob el archivo " + fileName);
                            email.Status = HighwayEmailStatusDto.Processing;
                        }
                    }
                }

                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "Notifico al servicio el archivo " + fileName);
                _highwayClientService.ProccessEmail(email);
            }
            catch (WebApiClientBusinessException e)
            {
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "HighwayController - SaveFile - WebApiClientBusinessException");
                NLogLogger.LogHighwayFileProccessEvent(e);
                return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable };
            }
            catch (WebApiClientFatalException e)
            {
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "HighwayController - SaveFile - WebApiClientFatalException");
                NLogLogger.LogHighwayFileProccessEvent(e);
                return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable };
            }
            catch (Exception e)
            {
                NLogLogger.LogHighwayFileProccessEvent(NLogType.Info, "HighwayController - SaveFile - Exception");
                NLogLogger.LogHighwayFileProccessEvent(e);
                return new HttpResponseMessage { StatusCode = HttpStatusCode.NotAcceptable };
            }
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }

        private static DateTime FromUnixTime(string unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long longresult = 0;
            var result = long.TryParse(unixTime, out longresult);

            return epoch.AddSeconds(result ? longresult : 0);
        }

        /// <summary>
        /// Verifies that the signature matches the timestamp & token.
        /// </summary>
        /// <returns>True if the signature is valid, otherwise false.</returns>
        public static bool VerifySignature(string apikey, string token, string timestamp, string signature)
        {
            var encoding = Encoding.ASCII;
            var hmacSha256 = new HMACSHA256(encoding.GetBytes(apikey));
            var cleartext = encoding.GetBytes(timestamp + token);
            var hash = hmacSha256.ComputeHash(cleartext);
            var computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();
            return computedSignature == signature;
        }

    }
}