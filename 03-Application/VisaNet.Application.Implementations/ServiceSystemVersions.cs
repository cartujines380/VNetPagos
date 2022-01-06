using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Reflection;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.Implementations
{
    public class ServiceSystemVersions : IServiceSystemVersions
    {
        public IDictionary<string, string> GetSystemVersions()
        {
            var result = new Dictionary<string, string>();

            var version = string.Empty;
            var path = string.Empty;

            //Private-APIs
            version = GetVersion(string.Empty, SystemVersionDto.CoreApiVersion);
            result.Add("CoreApi", version);

            path = ConfigurationManager.AppSettings["PathLifApiVersion"];
            version = GetVersion(path, SystemVersionDto.LifApiVersion);
            result.Add("LifApi", version);

            path = ConfigurationManager.AppSettings["PathCustomerSiteApiVersion"];
            version = GetVersion(path, SystemVersionDto.CustomerSiteApiVersion);
            result.Add("CustomerSiteApi", version);

            //Processes
            path = ConfigurationManager.AppSettings["PathPaymentProcessVersion"];
            version = GetVersion(path, SystemVersionDto.PaymentProcessVersion);
            result.Add("PaymentProcess", version);

            path = ConfigurationManager.AppSettings["PathNotificationProcessVersion"];
            version = GetVersion(path, SystemVersionDto.NotificationProcessVersion);
            result.Add("NotificationProcess", version);

            path = ConfigurationManager.AppSettings["PathConciliationProcessVersion"];
            version = GetVersion(path, SystemVersionDto.ConciliationProcessVersion);
            result.Add("ConciliationProcess", version);

            path = ConfigurationManager.AppSettings["PathCsAckProcessVersion"];
            version = GetVersion(path, SystemVersionDto.CsAckProcessVersion);
            result.Add("CsAckProcess", version);

            path = ConfigurationManager.AppSettings["PathDebitSynchronizationProcessVersion"];
            version = GetVersion(path, SystemVersionDto.DebitSynchronizationProcessVersion);
            result.Add("DebitSynchronizationProcess", version);

            return result;
        }

        private string GetVersion(string path, SystemVersionDto systemVersion)
        {
            var version = string.Empty;

            try
            {
                if (!path.EndsWith("\\"))
                {
                    path = path + "\\";
                }

                switch (systemVersion)
                {
                    //La Core-Api se devuelve directamente
                    case SystemVersionDto.CoreApiVersion:
                        version = FileVersionInfo.GetVersionInfo(Assembly.GetAssembly(typeof(ServiceSystemVersions)).Location).FileVersion;
                        break;

                    //Private-APIs - se deberian obtener por un Get a ellas
                    case SystemVersionDto.LifApiVersion:
                        path = path + "VisaNet.Services.LifApi.dll";
                        version = FileVersionInfo.GetVersionInfo(path).FileVersion;
                        break;

                    case SystemVersionDto.CustomerSiteApiVersion:
                        path = path + "VisaNet.Service.CustomerApi.dll";
                        version = FileVersionInfo.GetVersionInfo(path).FileVersion;
                        break;

                    //Processes - se obtienen por su path
                    case SystemVersionDto.PaymentProcessVersion:
                        path = path + "VisaNet.ConsoleApplication.PaymentProcess.exe";
                        version = FileVersionInfo.GetVersionInfo(path).FileVersion;
                        break;

                    case SystemVersionDto.NotificationProcessVersion:
                        path = path + "VisaNet.ConsoleApplication.PaymentProcess.exe"; //actualmente es el mismo que el Pago Programado pero corre de forma diferente
                        version = FileVersionInfo.GetVersionInfo(path).FileVersion;
                        break;

                    case SystemVersionDto.ConciliationProcessVersion:
                        path = path + "VisaNet.ConsoleApplication.ConciliationReports.exe";
                        version = FileVersionInfo.GetVersionInfo(path).FileVersion;
                        break;

                    case SystemVersionDto.CsAckProcessVersion:
                        path = path + "VisaNet.ConsoleApplication.CSAcknowledgement.exe";
                        version = FileVersionInfo.GetVersionInfo(path).FileVersion;
                        break;

                    case SystemVersionDto.DebitSynchronizationProcessVersion:
                        path = path + "VisaNet.ConsoleApplication.DebitRequestSynchronizator.exe"; //¿o deberia ser VisaNet.ConsoleApplication.DebitBotSynchronizator.exe?
                        version = FileVersionInfo.GetVersionInfo(path).FileVersion;
                        break;

                }
            }
            catch (Exception)
            {
                //Si no lo encuentra devuelve vacio
                version = string.Empty;
            }
            return version;
        }

    }
}