using System;
using System.Configuration;
using Ninject;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;

namespace VisaNet.Testing.ServiceTc33
{
    class Program
    {
        static void Main(string[] args)
        {

            var kernel = new StandardKernel();
            NinjectRegister.Register(kernel);

            var service = kernel.Get<IServiceTc33>();

            var result = service.Create(new Tc33Dto()
            {
                State = Tc33StateDto.Process,
                InputFileName = ConfigurationManager.AppSettings["Tc33FileName"],
                Id = Guid.NewGuid()
            }, true);

            var fileProcess = service.Proccessfile(result);

        }
    }
}
