using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Components.Sucive.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace Sucive
{
    public class SuciveAccess : ISuciveAccess
    {
        private readonly ISucive1 _sucive1;
        private readonly ISucive2 _sucive2;
        private readonly ISucive3 _sucive3;
        private readonly ISucive4 _sucive4;
        private readonly ISucive5 _sucive5;
        private readonly ISucive6 _sucive6;
        private readonly ISucive7 _sucive7;
        private readonly ISucive8 _sucive8;
        private readonly ISucive9 _sucive9;
        private readonly ISucive10 _sucive10;
        private readonly ISucive11 _sucive11;
        private readonly ISucive12 _sucive12;
        private readonly ISucive13 _sucive13;
        private readonly ISucive14 _sucive14;
        private readonly ISucive15 _sucive15;
        private readonly ISucive16 _sucive16;
        private readonly ISucive17 _sucive17;
        private readonly ISucive18 _sucive18;
        private readonly ISucive19 _sucive19;

        public SuciveAccess(ISucive1 sucive1, ISucive2 sucive2, ISucive3 sucive3, ISucive4 sucive4, ISucive5 sucive5,
            ISucive6 sucive6, ISucive7 sucive7, ISucive8 sucive8, ISucive9 sucive9,
            ISucive10 sucive10, ISucive11 sucive11, ISucive12 sucive12, ISucive13 sucive13,
            ISucive14 sucive14, ISucive15 sucive15, ISucive16 sucive16, ISucive17 sucive17, ISucive18 sucive18, ISucive19 sucive19)
        {
            _sucive1 = sucive1;
            _sucive2 = sucive2;
            _sucive3 = sucive3;
            _sucive4 = sucive4;
            _sucive5 = sucive5;
            _sucive6 = sucive6;
            _sucive7 = sucive7;
            _sucive8 = sucive8;
            _sucive9 = sucive9;
            _sucive10 = sucive10;
            _sucive11 = sucive11;
            _sucive12 = sucive12;
            _sucive13 = sucive13;
            _sucive14 = sucive14;
            _sucive15 = sucive15;
            _sucive16 = sucive16;
            _sucive17 = sucive17;
            _sucive18 = sucive18;
            _sucive19 = sucive19;
        }

        public List<BillSuciveDto> GetBills(string[] refs, string param, string type, int dpto)
        {

            var temp = refs[5];

            //si ya viene el idpadron, obtengo derecho de ahi las facturas. Si da un error, voy por el otro camino.
            if (temp != null && !string.IsNullOrEmpty(temp))
            {
                try
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("Sucive - Consulta por facturas. Departamento: {0}, Tipo clave: {1}, Id Padron: {2}", dpto, param, temp));
                    return GetBillsIdPadron(int.Parse(temp), dpto, param);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            NLogLogger.LogEvent(NLogType.Info, string.Format("Sucive - Consulta por facturas. Departamento: {0}, Tipo clave: {1}, Refs: {2}", dpto, param,
                string.Join(";", refs.Where(s => !String.IsNullOrEmpty(s)).ToArray())));
            switch (dpto)
            {
                case (int)DepartamentDtoType.Artigas:
                    return _sucive1.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Canelones:
                    return _sucive2.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Cerro_Largo:
                    return _sucive3.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Colonia:
                    return _sucive4.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Durazno:
                    return _sucive5.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Flores:
                    return _sucive6.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Florida:
                    return _sucive7.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Lavalleja:
                    return _sucive8.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Maldonado:
                    return _sucive9.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Montevideo:
                    return _sucive10.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Paysandu:
                    return _sucive11.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Rio_Negro:
                    return _sucive12.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Rivera:
                    return _sucive13.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Rocha:
                    return _sucive14.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Salto:
                    return _sucive15.GetBills(refs, param, type);
                case (int)DepartamentDtoType.San_Jose:
                    return _sucive16.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Soriano:
                    return _sucive17.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Tacuarembo:
                    return _sucive18.GetBills(refs, param, type);
                case (int)DepartamentDtoType.Treinta_y_Tres:
                    return _sucive19.GetBills(refs, param, type);
            }
            return null;
        }

        public List<BillSuciveDto> GetBillsIdPadron(int idPadron, int depto, string param)
        {
            NLogLogger.LogEvent(NLogType.Info, string.Format("Sucive - Consulta por facturas. Departamento: {0}, Tipo clave: {1}, Id Padron: {2}", depto, param, idPadron));
            switch (depto)
            {
                case (int)DepartamentDtoType.Artigas:
                    return _sucive1.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Canelones:
                    return _sucive2.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Cerro_Largo:
                    return _sucive3.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Colonia:
                    return _sucive4.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Durazno:
                    return _sucive5.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Flores:
                    return _sucive6.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Florida:
                    return _sucive7.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Lavalleja:
                    return _sucive8.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Maldonado:
                    return _sucive9.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Montevideo:
                    return _sucive10.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Paysandu:
                    return _sucive11.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Rio_Negro:
                    return _sucive12.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Rivera:
                    return _sucive13.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Rocha:
                    return _sucive14.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Salto:
                    return _sucive15.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.San_Jose:
                    return _sucive16.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Soriano:
                    return _sucive17.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Tacuarembo:
                    return _sucive18.GetBillsWithCc(idPadron, param);
                case (int)DepartamentDtoType.Treinta_y_Tres:
                    return _sucive19.GetBillsWithCc(idPadron, param);
            }
            return null;
        }

        public BillSuciveDto CheckIfBillPayable(string lines, int idPadron, int depto, string param)
        {
            NLogLogger.LogEvent(NLogType.Info, string.Format("Sucive - CheckIfBillPayable. Departamento: {0}, Lineas: {1}, idPadron: {2}, parametro {3}", depto, lines, idPadron, param));
            BillSuciveDto dto = null;
            try
            {
                switch (depto)
                {
                    case (int)DepartamentDtoType.Artigas:
                        dto = _sucive1.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Canelones:
                        dto = _sucive2.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Cerro_Largo:
                        dto = _sucive3.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Colonia:
                        dto = _sucive4.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Durazno:
                        dto = _sucive5.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Flores:
                        dto = _sucive6.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Florida:
                        dto = _sucive7.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Lavalleja:
                        dto = _sucive8.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Maldonado:
                        dto = _sucive9.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Montevideo:
                        dto = _sucive10.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Paysandu:
                        dto = _sucive11.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Rio_Negro:
                        dto = _sucive12.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Rivera:
                        dto = _sucive13.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Rocha:
                        dto = _sucive14.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Salto:
                        dto = _sucive15.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.San_Jose:
                        dto = _sucive16.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Soriano:
                        dto = _sucive17.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Tacuarembo:
                        dto = _sucive18.CheckIfBillPayable(lines, idPadron, param); break;
                    case (int)DepartamentDtoType.Treinta_y_Tres:
                        dto = _sucive19.CheckIfBillPayable(lines, idPadron, param); break;
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("Sucive - CheckIfBillPayable Exepcion. Departamento: {0}, Lineas: {1}, idPadron: {2}, parametro {3}", depto, lines, idPadron, param));
                NLogLogger.LogEvent(exception);
            }
            NLogLogger.LogEvent(NLogType.Info, string.Format("Sucive - CheckIfBillPayable resultado idPadron: {0}, nro pre factura {1}", idPadron, dto != null ? dto.SucivePreBillNumber : ""));
            return dto;
        }

        public long ConfirmPayment(long numeroPreFactura, string sucursal, string transactionId, int depto)
        {
            NLogLogger.LogEvent(NLogType.Info, string.Format("Sucive - Intento pago de facturas. Departamento: {0}, Numero Pre Factura: {1}, Transaction Id: {2}", depto, numeroPreFactura, transactionId));
            var suciveTransaction = long.Parse("-1");
            try
            {
                switch (depto)
                {
                    case (int)DepartamentDtoType.Artigas:
                        suciveTransaction = _sucive1.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Canelones:
                        suciveTransaction = _sucive2.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Cerro_Largo:
                        suciveTransaction = _sucive3.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Colonia:
                        suciveTransaction = _sucive4.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Durazno:
                        suciveTransaction = _sucive5.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Flores:
                        suciveTransaction = _sucive6.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Florida:
                        suciveTransaction = _sucive7.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Lavalleja:
                        suciveTransaction = _sucive8.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Maldonado:
                        suciveTransaction = _sucive9.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Montevideo:
                        suciveTransaction = _sucive10.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Paysandu:
                        suciveTransaction = _sucive11.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Rio_Negro:
                        suciveTransaction = _sucive12.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Rivera:
                        suciveTransaction = _sucive13.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Rocha:
                        suciveTransaction = _sucive14.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Salto:
                        suciveTransaction = _sucive15.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.San_Jose:
                        suciveTransaction = _sucive16.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Soriano:
                        suciveTransaction = _sucive17.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Tacuarembo:
                        suciveTransaction = _sucive18.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                    case (int)DepartamentDtoType.Treinta_y_Tres:
                        suciveTransaction = _sucive19.ConfirmarPago(numeroPreFactura, sucursal, transactionId);
                        break;
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("Sucive - Excepcion.  Departamento: {0}, Numero Pre Factura: {1}, Transaction Id: {2}", depto, numeroPreFactura, transactionId));
                NLogLogger.LogEvent(exception);
            }
            NLogLogger.LogEvent(NLogType.Info, string.Format("Sucive - Intento pago realizado. Departamento: {0}, Numero Pre Factura: {1}, Transaction Id: {2}", depto, numeroPreFactura, transactionId));
            if (suciveTransaction == -1)
                throw new ProviderBusinessException(CodeExceptions.SUCIVE_CONFIRMATION_ERROR);
            return suciveTransaction;
        }

        public int CheckAccount(string[] refs, string param, string type, int dpto)
        {
            switch (dpto)
            {
                case (int)DepartamentDtoType.Artigas:
                    return _sucive1.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Canelones:
                    return _sucive2.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Cerro_Largo:
                    return _sucive3.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Colonia:
                    return _sucive4.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Durazno:
                    return _sucive5.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Flores:
                    return _sucive6.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Florida:
                    return _sucive7.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Lavalleja:
                    return _sucive8.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Maldonado:
                    return _sucive9.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Montevideo:
                    return _sucive10.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Paysandu:
                    return _sucive11.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Rio_Negro:
                    return _sucive12.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Rivera:
                    return _sucive13.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Rocha:
                    return _sucive14.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Salto:
                    return _sucive15.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.San_Jose:
                    return _sucive16.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Soriano:
                    return _sucive17.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Tacuarembo:
                    return _sucive18.CheckAccount(refs, param, type);
                case (int)DepartamentDtoType.Treinta_y_Tres:
                    return _sucive19.CheckAccount(refs, param, type);
            }
            return -1;
        }

        public List<ConciliationSuciveDto> GetConciliation(DateTime from, DateTime to, List<int> dptos)
        {
            var result = new List<ConciliationSuciveDto>();
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
                            var count = GetConciliationDeptos(day, dpto);
                            NLogLogger.LogEvent(NLogType.Info, string.Format("SUCIVE - Obtuve {0} transacciones ", count != null ? count.Count() : 0));
                            result.AddRange(count);
                        }
                        catch (Exception e)
                        {
                            NLogLogger.LogEvent(NLogType.Error, string.Format("SUCIVE - No se pudo obtener las transacciones para el departamento {0} para la fecha {1}", dpto, day));
                            NLogLogger.LogEvent(e);
                        }

                        day = day.AddDays(1);
                        f = day.DayOfYear;
                    }
                }

            }
            return result;
        }

        private IEnumerable<ConciliationSuciveDto> GetConciliationDeptos(DateTime date, int dpto)
        {
            NLogLogger.LogEvent(NLogType.Info, string.Format("SUCIVE - Obtengo transacciones para el departamento {0} para la fecha {1}", (DepartamentDtoType)dpto, date.ToString("d")));
            switch (dpto)
            {
                case (int)DepartamentDtoType.Artigas:
                    return _sucive1.Conciliation(date);
                case (int)DepartamentDtoType.Canelones:
                    return _sucive2.Conciliation(date);
                case (int)DepartamentDtoType.Cerro_Largo:
                    return _sucive3.Conciliation(date);
                case (int)DepartamentDtoType.Colonia:
                    return _sucive4.Conciliation(date);
                case (int)DepartamentDtoType.Durazno:
                    return _sucive5.Conciliation(date);
                case (int)DepartamentDtoType.Flores:
                    return _sucive6.Conciliation(date);
                case (int)DepartamentDtoType.Florida:
                    return _sucive7.Conciliation(date);
                case (int)DepartamentDtoType.Lavalleja:
                    return _sucive8.Conciliation(date);
                case (int)DepartamentDtoType.Maldonado:
                    return _sucive9.Conciliation(date);
                case (int)DepartamentDtoType.Montevideo:
                    return _sucive10.Conciliation(date);
                case (int)DepartamentDtoType.Paysandu:
                    return _sucive11.Conciliation(date);
                case (int)DepartamentDtoType.Rio_Negro:
                    return _sucive12.Conciliation(date);
                case (int)DepartamentDtoType.Rivera:
                    return _sucive13.Conciliation(date);
                case (int)DepartamentDtoType.Rocha:
                    return _sucive14.Conciliation(date);
                case (int)DepartamentDtoType.Salto:
                    return _sucive15.Conciliation(date);
                case (int)DepartamentDtoType.San_Jose:
                    return _sucive16.Conciliation(date);
                case (int)DepartamentDtoType.Soriano:
                    return _sucive17.Conciliation(date);
                case (int)DepartamentDtoType.Tacuarembo:
                    return _sucive18.Conciliation(date);
                case (int)DepartamentDtoType.Treinta_y_Tres:
                    return _sucive19.Conciliation(date);
            }
            return null;
        }

        public BillSuciveDto GenerateAnnualPatente(string[] refs, string param, string type, int dpto)
        {
            var bills = GetBills(refs, param, type, dpto);

            //SI NO HAY FACTURAS, NO HAY PATENTE ANUAL
            if (bills == null || !bills.Any())
                return null;

            var now = DateTime.Now;

            //SI NO ESTOY EN ENERO, NO HAY FACTURA ANUAL CON DESCUENTO
            if (now.Month != 1)
                return null;

            var firstBill =
                bills.Where(x => x.Year.Equals(now.Year.ToString()) && x.Codigo.Equals("200")).OrderBy(x => x.ExpirationDate).FirstOrDefault();

            //SI NO HAY FACTURA DE PATENTE O NOS PASAMOS DEL DIA DE VENCIMIENTO DE LA PRIMERA
            if (firstBill == null || now.Day > firstBill.ExpirationDate.Day)
                return null;

            var lines = string.Empty;
            var billsCount = 0;
            foreach (var billSuciveDto in bills.Where(x => x.Year.Equals(now.Year.ToString())).OrderBy(x => x.ExpirationDate))
            {
                if (billSuciveDto.Codigo.Equals("200"))
                {
                    billsCount++;
                    lines += billSuciveDto.Line;
                }
            }

            if (string.IsNullOrEmpty(lines))
                return null;

            if(billsCount != 6)
                return null;

            var bill = this.CheckIfBillPayable(lines, firstBill.IdPadron, dpto, param);

            return bill;
        }
    }
}