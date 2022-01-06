using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using VisaNet.Banred.Mock.WcfBanredTest;

namespace VisaNet.Banred.Mock
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class BanredWsPagosBancosMock : PagosBancos
    {
        public consultaEntesDisponiblesResponse consultaEntesDisponibles(consultaEntesDisponiblesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<consultaEntesDisponiblesResponse> consultaEntesDisponiblesAsync(consultaEntesDisponiblesRequest request)
        {
            throw new NotImplementedException();
        }

        public consultaFacturasEnteResponse consultaFacturasEnte(consultaFacturasEnteRequest request)
        {
            switch (request.Body.idAgenteExterno)
            {
                case "A1111":
                    return new consultaFacturasEnteResponse(new consultaFacturasEnteResponseBody());
                case "A1112":
                    break;
                case "A2221":
                    break;
                case "A2222":
                    break;
                case "A2223":
                    break;
                case "A2224":
                    break;
                case "A2225":
                    break;
                case "A2226":
                    break;
                case "A2227":
                    break;
                case "A2228":
                    break;
                case "A2229":
                    break;
                default:
                    break;
            }
            throw new NotImplementedException();
        }

        public Task<consultaFacturasEnteResponse> consultaFacturasEnteAsync(consultaFacturasEnteRequest request)
        {
            throw new NotImplementedException();
        }

        public pagarFacturaEnteResponse pagarFacturaEnte(pagarFacturaEnteRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<pagarFacturaEnteResponse> pagarFacturaEnteAsync(pagarFacturaEnteRequest request)
        {
            throw new NotImplementedException();
        }

        public confirmarOperacionResponse confirmarOperacion(confirmarOperacionRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<confirmarOperacionResponse> confirmarOperacionAsync(confirmarOperacionRequest request)
        {
            throw new NotImplementedException();
        }

        public consultaEstadoOperacionResponse consultaEstadoOperacion(consultaEstadoOperacionRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<consultaEstadoOperacionResponse> consultaEstadoOperacionAsync(consultaEstadoOperacionRequest request)
        {
            throw new NotImplementedException();
        }

        public consultaEstadoPagoResponse consultaEstadoPago(consultaEstadoPagoRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<consultaEstadoPagoResponse> consultaEstadoPagoAsync(consultaEstadoPagoRequest request)
        {
            throw new NotImplementedException();
        }

        public resetearEpinResponse resetearEpin(resetearEpinRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<resetearEpinResponse> resetearEpinAsync(resetearEpinRequest request)
        {
            throw new NotImplementedException();
        }

    }
}