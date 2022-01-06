using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Components.Geocom.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace Geocom
{
    public class GeocomAccess : IGeocomAccess
    {
        private readonly IGeocomMa _geocomMa;
        private readonly IGeocomRo _geocomRo;
        private readonly IGeocomCa _geocomCa;
        private readonly IGeocomFo _geocomFo;

        public GeocomAccess(IGeocomMa geocomMa, IGeocomRo geocomRo, IGeocomCa geocomCa, IGeocomFo geocomFo)
        {
            _geocomMa = geocomMa;
            _geocomRo = geocomRo;
            _geocomCa = geocomCa;
            _geocomFo = geocomFo;
        }

        public List<BillGeocomDto> GetBills(string[] refs, string param, string type, int dpto)
        {
            var temp = refs[5];
            //Si el tipo de transaccion es 1, ya voy por idpadron.
            if (type.Equals("1"))
            {
                return GetBillsIdPadron(int.Parse(refs[0]), dpto, param);
            }
            //si ya viene el idpadron, obtengo derecho de ahi las facturas. Si da un error, voy por el otro camino.
            if (temp != null && !string.IsNullOrEmpty(temp))
            {
                try
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("Geocom - Consulta por facturas. Departamento: {0}, Tipo clave: {1}, Id Padron: {2}", dpto, param, temp));
                    return GetBillsIdPadron(int.Parse(temp), dpto, param);
                }
                catch (Exception)
                {
                    
                    throw;
                }
                
            }
            NLogLogger.LogEvent(NLogType.Info, string.Format("Geocom - Consulta por facturas. Departamento: {0}, Tipo clave: {1}, Refs: {2}", dpto, param,
                string.Join(";", refs.Where(s => !String.IsNullOrEmpty(s)).ToArray())));
            switch (dpto)
            {
                case (int)DepartamentDtoType.Artigas:
//                    return _geocom1.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Canelones:
                    return _geocomCa.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Cerro_Largo:
//                    return _geocom3.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Colonia:
//                    return _geocom4.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Durazno:
                //  return _geocom5.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Flores:
                //  return _geocom6.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Florida:
                    return _geocomFo.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Lavalleja:
                //  return _geocom8.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Maldonado:
                    return _geocomMa.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Montevideo:
                //  return _geocom10.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Paysandu:
                //  return _geocom11.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Rio_Negro:
                //  return _geocom12.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Rivera:
                //  return _geocom13.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Rocha:
                    return _geocomRo.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Salto:
                //  return _geocom15.GetBills(refs, param, type);
                case (int)DepartamentDtoType.San_Jose:
                //  return _geocom16.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Soriano:
                //  return _geocom17.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Tacuarembo:
                //  return _geocom18.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Treinta_y_Tres:
                //  return _geocom19.GetBills(refs, param, type);
                    break;
            }
            return null;
        }
        public List<BillGeocomDto> GetBillsIdPadron(int idPadron, int depto, string param)
        {
            NLogLogger.LogEvent(NLogType.Info, string.Format("Geocom - Consulta por facturas. Departamento: {0}, Tipo clave: {1}, Id Padron: {2}", depto, param, idPadron));
            switch (depto)
            {
                case (int)DepartamentDtoType.Artigas:
                //    return _geocom1.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Canelones:
                    return _geocomCa.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Cerro_Largo:
                //  return _geocom3.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Colonia:
                //    return _geocom4.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Durazno:
                //  return _geocom5.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Flores:
                //  return _geocom6.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Florida:
                    return _geocomFo.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Lavalleja:
                //  return _geocom8.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Maldonado:
                    return _geocomMa.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Montevideo:
                //  return _geocom10.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Paysandu:
                //  return _geocom11.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Rio_Negro:
                //  return _geocom12.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Rivera:
                //  return _geocom13.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Rocha:
                    return _geocomRo.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Salto:
                //  return _geocom15.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.San_Jose:
                //  return _geocom16.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Soriano:
                //  return _geocom17.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Tacuarembo:
                //  return _geocom18.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Treinta_y_Tres:
                //  return _geocom19.GetBillsWithCc(idPadron, param);
                    break;
            }
            return null;
        }
        public BillGeocomDto CheckIfBillPayable(string lines, int idPadron, int depto, string param)
        {
            switch (depto)
            {
                case (int)DepartamentDtoType.Artigas:
                //   return _geocom1.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Canelones:
                    return _geocomCa.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Cerro_Largo:
                //  return _geocom3.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Colonia:
                //  return _geocom4.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Durazno:
                //  return _geocom5.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Flores:
                //  return _geocom6.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Florida:
                    return _geocomFo.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Lavalleja:
                //  return _geocom8.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Maldonado:
                    return _geocomMa.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Montevideo:
                //  return _geocom10.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Paysandu:
                //  return _geocom11.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Rio_Negro:
                //  return _geocom12.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Rivera:
                //  return _geocom13.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Rocha:
                  return _geocomRo.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Salto:
                //  return _geocom15.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.San_Jose:
                //  return _geocom16.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Soriano:
                //  return _geocom17.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Tacuarembo:
                //  return _geocom18.CheckIfBillPayable(lines, idPadron, param);
                case (int)DepartamentDtoType.Treinta_y_Tres:
                //  return _geocom19.CheckIfBillPayable(lines, idPadron, param);
                    break;
            }
            return null;
        }

        public long ConfirmPayment(int numeroPreFactura, string sucursal, string transactionId, int depto, string param)
        {
            NLogLogger.LogEvent(NLogType.Info, string.Format("Geocom - Intento pago de facturas. Departamento: {0}, Numero Pre Factura: {1}, Transaction Id: {2}", depto, numeroPreFactura, transactionId));
            var geocomTransaction = new long();
            switch (depto)
            {
                case (int)DepartamentDtoType.Artigas:
                    //    geocomTransaction = _geocom1.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
                case (int)DepartamentDtoType.Canelones:
                    geocomTransaction = _geocomCa.ConfirmarPago(numeroPreFactura, sucursal, transactionId, param);
                    break;
                case (int)DepartamentDtoType.Cerro_Largo:
                    //geocomTransaction = _geocom3.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
                case (int)DepartamentDtoType.Colonia:
                    //geocomTransaction = _geocom4.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
                case (int)DepartamentDtoType.Durazno:
                    //geocomTransaction = _geocom5.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
                case (int)DepartamentDtoType.Flores:
                    //geocomTransaction = _geocom6.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
                case (int)DepartamentDtoType.Florida:
                    geocomTransaction = _geocomFo.ConfirmarPago(numeroPreFactura, sucursal, transactionId, param);
                    break;
                case (int)DepartamentDtoType.Lavalleja:
                    //geocomTransaction = _geocom8.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
                case (int)DepartamentDtoType.Maldonado:
                    geocomTransaction = _geocomMa.ConfirmarPago(numeroPreFactura, sucursal, transactionId, param);
                    break;
                case (int)DepartamentDtoType.Montevideo:
                    //geocomTransaction = _geocom10.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
                case (int)DepartamentDtoType.Paysandu:
                    //geocomTransaction = _geocom11.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
                case (int)DepartamentDtoType.Rio_Negro:
                    //geocomTransaction = _geocom12.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
                case (int)DepartamentDtoType.Rivera:
                    //geocomTransaction = _geocom13.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
                case (int)DepartamentDtoType.Rocha:
                    geocomTransaction = _geocomRo.ConfirmarPago(numeroPreFactura, sucursal, transactionId, param);
                    break;
                case (int)DepartamentDtoType.Salto:
                    //geocomTransaction = _geocom15.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
                case (int)DepartamentDtoType.San_Jose:
                    //geocomTransaction = _geocom16.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
                case (int)DepartamentDtoType.Soriano:
                    //geocomTransaction = _geocom17.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
                case (int)DepartamentDtoType.Tacuarembo:
                    //geocomTransaction = _geocom18.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
                case (int)DepartamentDtoType.Treinta_y_Tres:
                    //geocomTransaction = _geocom19.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                    break;
            }
            if(geocomTransaction < 1)
                throw new ProviderBusinessException(CodeExceptions.GEOCOM_CONFIRMATION_ERROR);
            return geocomTransaction;
        }
        public int CheckAccount(string[] refs, string param, string type, int dpto)
        {
            NLogLogger.LogEvent(NLogType.Info, string.Format("Geocom - Consulta por cuenta. Departamento: {0}, Tipo clave: {1}, Refs: {2}, Type {3}", dpto, param,
                string.Join(";", refs.Where(s => !String.IsNullOrEmpty(s)).ToArray()), type));

            switch (dpto)
            {
                case (int)DepartamentDtoType.Artigas:
                //    return _geocom1.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Canelones:
                    return _geocomCa.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Cerro_Largo:
                //  return _geocom3.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Colonia:
                //  return _geocom4.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Durazno:
                //  return _geocom5.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Flores:
                //  return _geocom6.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Florida:
                    return _geocomFo.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Lavalleja:
                //  return _geocom8.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Maldonado:
                    return _geocomMa.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Montevideo:
                //  return _geocom10.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Paysandu:
                //  return _geocom11.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Rio_Negro:
                //  return _geocom12.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Rivera:
                //  return _geocom13.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Rocha:
                    return _geocomRo.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Salto:
                //  return _geocom15.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.San_Jose:
                //  return _geocom16.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Soriano:
                //  return _geocom17.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Tacuarembo:
                //  return _geocom18.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Treinta_y_Tres:
                //  return _geocom19.CheckAccount(refs, param, type);
                    break;
            }
            return -1;
        }
        public List<ConciliationGeocomDto> GetConciliation(DateTime from, DateTime to, List<int> dptos)
        {
            var result = new List<ConciliationGeocomDto>();
            foreach (var dpto in dptos)
            {
                if (dpto > 0)
                {
                    var f = from.DayOfYear;
                    var t = to.DayOfYear;
                    var day = from;
                    while (t >= f)
                    {
                        try
                        {
                            result.AddRange(GetConciliationDeptos(day, dpto));
                        }
                        catch (Exception e)
                        {
                            NLogLogger.LogEvent(NLogType.Error, string.Format("GEOCOM - No se pudo obtener las transacciones para el departamento {0} para la fecha {1}", dpto,day));
                            NLogLogger.LogEvent(e);
                        }
                        
                        day = day.AddDays(1);
                        f = day.DayOfYear;
                    }
                }
                
            }
            return result;
        }
        private IEnumerable<ConciliationGeocomDto> GetConciliationDeptos(DateTime date, int dpto)
        {
            switch (dpto)
            {
                case (int)DepartamentDtoType.Artigas:
                //  return _geocom1.Conciliation(date);
                case (int)DepartamentDtoType.Canelones:
                    return _geocomCa.Conciliation(date);
                case (int)DepartamentDtoType.Cerro_Largo:
                //  return _geocom3.Conciliation(date);
                case (int)DepartamentDtoType.Colonia:
                //  return _geocom4.Conciliation(date);
                case (int)DepartamentDtoType.Durazno:
                //  return _geocom5.Conciliation(date);
                case (int)DepartamentDtoType.Flores:
                //  return _geocom6.Conciliation(date);
                case (int)DepartamentDtoType.Florida:
                    return _geocomFo.Conciliation(date);
                case (int)DepartamentDtoType.Lavalleja:
                //  return _geocom8.Conciliation(date);
                case (int)DepartamentDtoType.Maldonado:
                    return _geocomMa.Conciliation(date);
                case (int)DepartamentDtoType.Montevideo:
                //  return _geocom10.Conciliation(date);
                case (int)DepartamentDtoType.Paysandu:
                //  return _geocom11.Conciliation(date);
                case (int)DepartamentDtoType.Rio_Negro:
                //  return _geocom12.Conciliation(date);
                case (int)DepartamentDtoType.Rivera:
                //  return _geocom13.Conciliation(date);
                case (int)DepartamentDtoType.Rocha:
                    return _geocomRo.Conciliation(date);
                case (int)DepartamentDtoType.Salto:
                //  return _geocom15.Conciliation(date);
                case (int)DepartamentDtoType.San_Jose:
                //  return _geocom16.Conciliation(date);
                case (int)DepartamentDtoType.Soriano:
                //  return _geocom17.Conciliation(date);
                case (int)DepartamentDtoType.Tacuarembo:
                //  return _geocom18.Conciliation(date);
                case (int)DepartamentDtoType.Treinta_y_Tres:
                //  return _geocom19.Conciliation(date);
                    break;
            }
            return null;
        }

    }
}
