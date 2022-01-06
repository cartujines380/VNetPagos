using System.Linq;
using Ninject;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.NLog;
using VisaNet.ConsoleApplication.PaymentProcess.PaymentsProcessor;

namespace VisaNet.ConsoleApplication.PaymentProcess
{
    class Program
    {
        static void Main(string[] args)
        {

            //args = new[] { "sucive" };
            //args = new[] { "paywithoutgetbill" };
            //args = new[] { "tc33" };
            args = new[] { "paybills" };
            //args = new[] { "tc33" };Handler
            //args = new[] { "cancelpaymenthandler" };

            var kernel = new StandardKernel();
            //NinjectRegister.Register(kernel, true); //Viejo
            NinjectRegister.RegisterThreadScope(kernel, true); //Nuevo: para threads

            //var debit = new DebitProcessorHandler();
            var automaticPayment = NinjectRegister.Get<AutomaticPaymentHandler>();

            if (args.Any())
            {
                var method = args.First();
                var days = 0;
                if (args.Length > 1)
                {
                    days = int.Parse(args[1]);
                }
                switch (method.ToLower())
                {
                    case "paybills":
                        automaticPayment.Start();
                        break;
                    //case "sucivebatchfile":
                    //    process.SuciveBatchConsiliation(days);
                    //    break;
                    case "cancelpaymenthandler":
                        var cancel = new CancelPaymentHandler();
                        cancel.Start();
                        break;
                    default:
                        NLogLogger.LogEvent(NLogType.Info, string.Format("La opción ingresada no existe: {0}", method));
                        break;
                }
            }
            else
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("No se ingreso opción"));
            }
        }

    }
}