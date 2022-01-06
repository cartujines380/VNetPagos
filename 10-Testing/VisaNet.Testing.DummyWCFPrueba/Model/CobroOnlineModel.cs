using System;

namespace VisaNet.Testing.DummyWCFPrueba.Model
{
    public class CobroOnlineModel
    {
        public string IdOperacion { get; set; }
        public string IdApp { get; set; }
        public FacturaOnlineModel FacturaOnlineModel { get; set; }
    }

    public class FacturaOnlineModel
    {
        public string CodComercio { get; set; }
        public string CodSucursal { get; set; }
        public string IdMerchant { get; set; }
        public string IdUsuario { get; set; }
        public string IdTarjeta { get; set; }
        public string NroFactura { get; set; }
        public string Descripcion { get; set; }
        public DateTime FchFactura { get; set; }
        public string Moneda { get; set; }
        public double MontoTotal { get; set; }
        public double MontoGravado { get; set; }
        public int Indi { get; set; }
        public bool ConsFinal { get; set; }
        public int Cuotas { get; set; }
        public string DeviceFingerprint { get; set; }
        public string IpCliente { get; set; }
        public string TelefonoCliente { get; set; }
        public DireccionEnvioClienteModel DireccionEnvioClienteModel { get; set; }
        public AuxiliarDataModel[] AuxiliarData { get; set; }
    }

    public class AuxiliarDataModel
    {
        public string Id_auxiliar { get; set; }
        public string Dato_auxiliar { get; set; }
    }

    public class DireccionEnvioClienteModel
    {
        public string Calle { get; set; }
        public string NumeroPuerta { get; set; }
        public string Complemento { get; set; }
        public string Esquina { get; set; }
        public string Barrio { get; set; }
        public string CodigoPostal { get; set; }
        public string Longitud { get; set; }
        public string Latitud { get; set; }
        public string Telefono { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }
    }
    
}
