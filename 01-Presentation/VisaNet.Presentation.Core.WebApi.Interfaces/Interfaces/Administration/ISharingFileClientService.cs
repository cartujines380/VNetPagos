using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface ISharingFileClientService
    {
        Task PostFile(HttpPostedFileBase file, string fileName);
        Task DeleteFile(String fileName);

        //System Versions
        Task<IDictionary<string, string>> GetAllVersions();
    }
}
