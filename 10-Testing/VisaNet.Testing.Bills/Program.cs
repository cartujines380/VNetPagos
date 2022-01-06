using System;
using System.Collections.Generic;
using Ninject;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Testing.Bills
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            NinjectRegister.Register(kernel);
            
            var serviceBill = kernel.Get<IServiceBill>();

            try
            {
                serviceBill.GetBillsForAnonymousUser(new AnonymousUserBillFilterDto()
                {
                    AnonymousUserDto = new AnonymousUserDto() { Email = "gvarini@hexacta.com" },
                    ServiceId = Guid.Parse("90d65f41-9836-429b-ac20-1525d3880845"),
                    References = new Dictionary<string, string>() { { "Número de abonado", "131895" } }
                });
            }
            catch (Exception)
            {
            }

            try
            {
                serviceBill.GetBillsForAnonymousUser(new AnonymousUserBillFilterDto()
                {
                    AnonymousUserDto = new AnonymousUserDto() { Email = "gvarini@hexacta.com" },
                    ServiceId = Guid.Parse("90d65f41-9836-429b-ac20-1525d3880845"),
                    References = new Dictionary<string, string>() { { "Número de abonado", "129494" } }
                });
            }
            catch (Exception)
            {
            }

            try
            {
                serviceBill.GetBillsForAnonymousUser(new AnonymousUserBillFilterDto()
                {
                    AnonymousUserDto = new AnonymousUserDto() { Email = "gvarini@hexacta.com" },
                    ServiceId = Guid.Parse("ba82d610-4e31-43cb-af04-6f603cfadce7"),
                    References = new Dictionary<string, string>() { { "Cuenta Corriente", "1021228" } }
                });
            }
            catch (Exception)
            {
            }
            
        }
    }
}
