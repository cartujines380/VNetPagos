using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Utilities.Exportation.ExtensionMethods;
using VisaNet.WebService.VisaWCF.EntitiesDto;
using VisaNet.WebService.VisaWCF.NLog;

namespace VisaNet.WebService.VisaWCF.Mappers
{
    public static class NotificationMapper
    {
        public static NLogType ToNLogType(this NotificationType logType)
        {
            NLogType nLogType = NLogType.Info;
            switch (logType)
            {
                case NotificationType.Ok: nLogType = NLogType.Info;
                    break;
                case NotificationType.Error: nLogType = NLogType.Error;
                    break;
            }
            return nLogType;
        }

        public static String[] ToParamsArray(this NotificationData data)
        {
            var paramsList = new List<String> { data.PaymentPlatform };

            paramsList.Add(data.NotificationType.ToString());
            paramsList.Add(data.Operation.ToString());
            if (data.Message != null) paramsList.Add(data.Message);

            return paramsList.ToArray();
        }
    }
}