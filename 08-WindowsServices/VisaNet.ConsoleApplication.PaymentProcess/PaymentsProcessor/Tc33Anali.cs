using System;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.ConsoleApplication.PaymentProcess.PaymentsProcessor
{
    public class Tc33Anali
    {

        private static readonly IServiceTc33 _serviceHighway = NinjectRegister.Get<IServiceTc33>();
        
        public void Anali()
        {
            var result = _serviceHighway.Proccessfile(new Tc33Dto()
                                         {
                                             State = Tc33StateDto.Process,
                                             InputFileName = "pruebaTc33.txt",
                                             Id = Guid.NewGuid()
                                         });

            if (result == null)
            {
                NLogLogger.LogTc33Event(NLogType.Info, "result null");
            }
            else { 
                if (result.Errors.Any())
                {
                    NLogLogger.LogTc33Event(NLogType.Info, "HAY ERRORES");
                    foreach (var error in result.Errors)
                    {
                        NLogLogger.LogTc33Event(NLogType.Info, error);
                    }
                }
            }
        }

    }
}
