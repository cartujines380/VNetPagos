using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.FileSharing.Controllers
{
    public class FileSharingController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage DeleteFile(string fileName)
        {
            var path = Path.Combine(ConfigurationManager.AppSettings["SharedImagesFolder"], fileName);
            var fileInfo = new FileInfo(path);
            if (fileInfo.Exists) fileInfo.Delete();
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPut]
        public HttpResponseMessage PostFile([FromBody] dynamic dynamicFile)
        {
            var fileName = (string)dynamicFile.FileName;
            var file = (string)dynamicFile.File;
            var image = ImageFromBase64String(file);
            image.Save(Path.Combine(ConfigurationManager.AppSettings["SharedImagesFolder"], fileName));
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        /// <summary>
        /// Creates a new image from the given base64 encoded string.
        /// </summary>
        /// <param name="base64String">The encoded image data as a string.</param>
        Image ImageFromBase64String(string base64String)
        {
            using (MemoryStream stream = new MemoryStream(
                Convert.FromBase64String(base64String)))
            using (Image sourceImage = Image.FromStream(stream))
            {
                return new Bitmap(sourceImage);
            }
        }

    }
}