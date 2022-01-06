using System.Configuration;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.ConsoleApplication.PaymentProcess.PaymentsProcessor
{
    public class CancelPaymentHandler
    {

        private static readonly IServicePayment _servicePayment = NinjectRegister.Get<IServicePayment>();

        public void Start()
        {
            NLogLogger.LogEvent(NLogType.Info, "INICIA PROCESO DE CANCELACION");
            var transactionsList = ConfigurationManager.AppSettings["CsTrnsToCancel"];
            if (!string.IsNullOrEmpty(transactionsList))
            {
                var trns = transactionsList.Split(';');

                foreach (var trn in trns.Where(trn => !string.IsNullOrEmpty(trn)))
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("Intento cancelacion de trns {0}", trn));
                    //var result = _servicePayment.CancelPayment(trn);
                    var payment = _servicePayment.AllNoTracking(null, x => x.TransactionNumber == trn).FirstOrDefault();
                    if (payment.PaymentStatus != PaymentStatusDto.Done)
                    {
                        NLogLogger.LogEvent(NLogType.Info,
                            string.Format("La transacción {0} se encuentra en estado {1}", trn,
                                payment.PaymentStatus.ToString()));
                    }
                }
            }
            else
            {
                NLogLogger.LogEvent(NLogType.Info, "NO HAY TRANSACCIONES EN CONFIG");
            }
        }
    }
}
