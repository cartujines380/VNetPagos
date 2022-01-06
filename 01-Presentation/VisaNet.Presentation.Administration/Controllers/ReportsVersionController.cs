using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class ReportsVersionController : Controller
    {
        private readonly ISystemVersionsClientService _systemVersionsClientService;

        public ReportsVersionController(ISystemVersionsClientService systemVersionsClientService)
        {
            _systemVersionsClientService = systemVersionsClientService;
        }

        [CustomAuthentication(Actions.ReportsVersionDetails)]
        public async Task<ActionResult> Index()
        {
            var model = new SystemVersionsModel();

            //El Admin se obtiene local
            model.AdminVersion = FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(ReportsVersionController)).Location).FileVersion;

            //Las Webs se obtienen mediante un Get a la URL especificada
            model = await GetWebVersions(model);

            //Las Public-APIs se obtienen mediante un Get a la URL especificada
            model = await GetPublicApiVersions(model);

            //Las Private-APIs y los Procesos se obtienen mediante API-Core
            var versionsDictionary = await _systemVersionsClientService.GetSystemVersions();
            model = GetPrivateApiVersions(model, versionsDictionary);
            model = GetProcessesVersions(model, versionsDictionary);

            return View(model);
        }

        private async Task<SystemVersionsModel> GetWebVersions(SystemVersionsModel model)
        {
            var urlWebVersion = ConfigurationManager.AppSettings["UrlWebVersion"];
            model.WebVersion = await RequestVersion(urlWebVersion);

            var urlVonRegisterVersion = ConfigurationManager.AppSettings["UrlVonRegisterVersion"];
            model.VonRegisterVersion = await RequestVersion(urlVonRegisterVersion);

            var urlVisaNetOnVersion = ConfigurationManager.AppSettings["UrlVisaNetOnVersion"];
            model.VisaNetOnVersion = await RequestVersion(urlVisaNetOnVersion);

            var urlCustomerSiteVersion = ConfigurationManager.AppSettings["UrlCustomerSiteVersion"];
            model.CustomerSiteVersion = await RequestVersion(urlCustomerSiteVersion);

            return model;
        }

        private async Task<SystemVersionsModel> GetPublicApiVersions(SystemVersionsModel model)
        {
            var urlLifPublicApiVersion = ConfigurationManager.AppSettings["UrlLifPublicApiVersion"];
            model.LifPublicApiVersion = await RequestVersion(urlLifPublicApiVersion);

            var urlNotificationPublicApiVersion = ConfigurationManager.AppSettings["UrlNotificationPublicApiVersion"];
            model.NotificationPublicApiVersion = await RequestVersion(urlNotificationPublicApiVersion);

            return model;
        }

        private SystemVersionsModel GetPrivateApiVersions(SystemVersionsModel model, IDictionary<string, string> versionsDictionary)
        {
            model.CoreApiVersion = SetValueOrErrorMessage(versionsDictionary, "CoreApi");
            model.LifApiVersion = SetValueOrErrorMessage(versionsDictionary, "LifApi");
            model.CustomerSiteApiVersion = SetValueOrErrorMessage(versionsDictionary, "CustomerSiteApi");
            return model;
        }

        private SystemVersionsModel GetProcessesVersions(SystemVersionsModel model, IDictionary<string, string> versionsDictionary)
        {
            model.PaymentProcessVersion = SetValueOrErrorMessage(versionsDictionary, "PaymentProcess");
            model.NotificationProcessVersion = SetValueOrErrorMessage(versionsDictionary, "NotificationProcess");
            model.ConciliationProcessVersion = SetValueOrErrorMessage(versionsDictionary, "ConciliationProcess");
            model.CsAckProcessVersion = SetValueOrErrorMessage(versionsDictionary, "CsAckProcess");
            model.DebitSynchronizationProcessVersion = SetValueOrErrorMessage(versionsDictionary, "DebitSynchronizationProcess");
            return model;
        }

        private async Task<string> RequestVersion(string url)
        {
            const string emptyVersion = "No se pudo obtener.";
            var version = emptyVersion;

            if (!string.IsNullOrEmpty(url))
            {
                try
                {
                    var uri = new Uri(url);
                    using (var client = new HttpClient())
                    {
                        version = await client.GetStringAsync(uri);
                        if (version.StartsWith("\"") && version.EndsWith("\"") && version.Length > 2)
                        {
                            var len = version.Length;
                            version = version.Substring(1, len - 2);
                        }
                    }
                }
                catch (Exception)
                {
                    version = emptyVersion;
                }
            }

            return version;
        }

        private string SetValueOrErrorMessage(IDictionary<string, string> versionsDictionary, string key)
        {
            const string emptyVersion = "No se pudo obtener.";
            var message = versionsDictionary.ContainsKey(key) && !string.IsNullOrEmpty(versionsDictionary[key]) ? versionsDictionary[key] : emptyVersion;
            return message;
        }

    }
}