using System;
using NLog;
using VisaNet.Common.Logging.NLog;
using VisaNet.WebService.VisaWCF.EntitiesDto;

namespace VisaNet.WebService.VisaWCF.NLog
{
    public enum NLogType
    {
        Fatal,
        Error,
        Warn,
        Info,
        Debug,
        Trace,
    }

    public static class CustomLogger
    {
        public static void LogEvent(Exception e)
        {
            LogEvent(NLogType.Error, e.Message);
            LogEvent(NLogType.Error, e.StackTrace);

            if (e.InnerException != null)
                LogEvent(e);
        }
       
        public static void LogEvent(NLogType type, string message)
        {
            //Graylog Logger
            GraylogLogger.LogEvent(new GraylogLog
            {
                Host = LogPlatform.Itau,
                Data = message,
                Level = Common.Logging.NLog.NLogType.Error
            });

            var Logger = LogManager.GetLogger("Default");

            switch (type)
            {
                case NLogType.Fatal: Logger.Fatal(message); break;
                case NLogType.Error: Logger.Error(message); break;
                case NLogType.Warn: Logger.Warn(message); break;
                case NLogType.Info: Logger.Info(message); break;
                case NLogType.Debug: Logger.Debug(message); break;
                case NLogType.Trace: Logger.Trace(message); break;
                default: Logger.Info(message); break;
            }
        }

        public static void LogGetServicesEvent(ServicesData data)
        {
            var info = string.Format("Datos: PaymentPlatform '{0}'.", data.PaymentPlatform);
            LogEvent(NLogType.Info, info);
        }

        public static void LogSearchBillsEvent(BillsData data)
        {
            var info = string.Format("Datos: PaymentPlatform '{0}', ServiceId '{1}', ServiceReferenceNumber '{2}', ServiceReferenceNumber2 '{3}', ServiceReferenceNumber3 '{4}', " +
                                         "ServiceReferenceNumber4 '{5}', ServiceReferenceNumber5 '{6}', ServiceReferenceNumber6 '{7}', GatewayEnumDto '{8}'.",
                                         data.PaymentPlatform, data.ServiceId, data.ServiceReferenceNumber, data.ServiceReferenceNumber2, data.ServiceReferenceNumber3,
                                         data.ServiceReferenceNumber4, data.ServiceReferenceNumber5, data.ServiceReferenceNumber6, data.GatewayEnumDto);
            LogEvent(NLogType.Info, info);
        }

        public static void LogPaymentEvent(PaymentData data)
        {
            var info = string.Format("Datos: PaymentPlatform '{0}', ServiceId '{1}'", data.PaymentPlatform, data.ServiceId);

            if (data.Bill != null)
            {
                info += string.Format(", BillId '{0}', BillNumber '{1}', ExpirationDate '{2}', Currency '{3}', Gateway '{4}', Description '{5}', GatewayTransactionId '{6}', " +
                        "Payable '{7}', FinalConsumer '{8}', TotalAmount '{9}', TotalTaxedAmount '{10}', ServiceId '{11}', Discount '{12}', " +
                        "DiscountApplyed '{13}', AmountToCybersource '{14}', ServiceReferenceNumber '{15}', ServiceReferenceNumber2 '{16}', ServiceReferenceNumber3 '{17}', " +
                        "ServiceReferenceNumber4 '{18}', ServiceReferenceNumber5 '{19}', ServiceReferenceNumber6 '{20}'",
                        data.Bill.BillId, data.Bill.BillNumber, data.Bill.ExpirationDate, data.Bill.Currency, data.Bill.Gateway, data.Bill.Description, data.Bill.GatewayTransactionId,
                        data.Bill.Payable, data.Bill.FinalConsumer, data.Bill.TotalAmount, data.Bill.TotalTaxedAmount, data.Bill.ServiceId, data.Bill.Discount,
                        data.Bill.DiscountApplyed, data.Bill.AmountToCybersource, data.Bill.ServiceReferenceNumber, data.Bill.ServiceReferenceNumber2, data.Bill.ServiceReferenceNumber3,
                        data.Bill.ServiceReferenceNumber4, data.Bill.ServiceReferenceNumber5, data.Bill.ServiceReferenceNumber6);
            }
            if (data.UserInfo != null)
            {
                info += string.Format(", Email '{0}', Ci '{1}', Address '{2}', Name '{3}', Surname '{4}'",
                    data.UserInfo.Email, data.UserInfo.Ci, data.UserInfo.Address, data.UserInfo.Name, data.UserInfo.Surname);
            }
            if (data.CardData != null)
            {
                info += string.Format(", CardMaskedNumber '{0}', CardDueDate '{1}', CardName '{2}', CardBinNumbers '{3}'",
                        data.CardData.MaskedNumber, data.CardData.DueDate, data.CardData.Name, data.CardData.CardBinNumbers);
            }
            if (data.CyberSourceData != null)
            {
                info += string.Format(", TransactionId '{0}'", data.CyberSourceData.TransactionId);
            }

            LogEvent(NLogType.Info, info);
        }

        public static void LogPreprocessPaymentEvent(PreprocessPaymentData data)
        {
            var infoBills = "";
            if (data.Bills != null)
            {
                foreach (var bill in data.Bills)
                {
                    infoBills += "{ BillId " + bill.BillId + ", BillNumber " + bill.BillNumber + ", ServiceId " + bill.ServiceId + ", Gateway " + bill.Gateway + ", ExpirationDate " + bill.ExpirationDate +
                        ", Currency " + bill.Currency + ", Description " + bill.Description + ", GatewayTransactionId " + bill.GatewayTransactionId + ", Payable " + bill.Payable +
                        ", FinalConsumer " + bill.FinalConsumer + ", TotalAmount " + bill.TotalAmount + ", IdPadron " + bill.CensusId + ", Lines " + bill.Lines + "}, ";
                }
            }
            var info = string.Format("Datos: PaymentPlatform '{0}', Bills: '{1}'.", data.PaymentPlatform, infoBills);
            LogEvent(NLogType.Info, info);
        }

        public static void LogSearchPaymentsEvent(SearchPaymentsData data)
        {
            var info = string.Format("Datos: PaymentPlatform '{0}'.", data.PaymentPlatform);
            LogEvent(NLogType.Info, info);
        }
    }
}
