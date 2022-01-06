using System;
using Ninject;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.ConsoleApplication.NotificationProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Inicio -- Chequeo de estado y renvio de pendientes"));

                var kernel = new StandardKernel();
                NinjectRegister.Register(kernel);

                NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Inicio -- Chequeo de mails"));
                var emailService = kernel.Get<IServiceEmailMessage>();
                emailService.CheckAllEmailStatus();
                NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Fin    -- Chequeo de mails"));

                NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Inicio -- Renvio de mails"));
                emailService.SendAllPendingEmails();
                NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Fin    -- Renvio de mails"));

                NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Fin -- Chequeo de estado y renvio de pendientes"));
            }
            catch (Exception exception)
            {
                NLogLogger.LogEmailNotificationEvent(NLogType.Info, string.Format("Exception"));
                NLogLogger.LogEmailNotificationEvent(exception);
                throw exception;
            }
            
        }
    }
}
