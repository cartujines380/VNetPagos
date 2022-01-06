using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    /// <summary>
    /// Objeto que contiene los datos de la factura a pagar
    /// </summary>
    /// <value> Factura a cobrar de forma online </value>
    [DataContract]
    public class FacturaOnline
    {
        /// <summary>
        /// <para /> Código del comercio que asigna VisaNet al momento del alta de una nueva Institución. 
        /// <para /> El campo no puede exceder el largo de 7 caracteres.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// <para /> Si IdMerchant viene vacío, este es un campo obligatorio.
        /// </summary>
        /// <value>El campo no puede exceder el largo de 7 caracteres</value>
        [DataMember]
        [StringLength(7, ErrorMessage = "El campo CodComercio excede el largo máximo permitido (7).")]
        public string CodComercio { get; set; }

        /// <summary>
        /// <para /> Código de Sucursal que asigna VisaNet al momento del alta de una nueva Institución.
        /// <para /> El campo no puede exceder el largo de 3 caracteres.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// <para /> Si IdMerchant viene vacío, este es un campo obligatorio.
        /// </summary>
        /// <value>El campo no puede exceder el largo de 3 caracteres</value>
        [DataMember]
        [StringLength(3, ErrorMessage = "El campo CodSucursal excede el largo máximo permitido (3).")]
        public string CodSucursal { get; set; }

        /// <summary>
        /// <para /> Es el valor asignado por VisaNet en el proceso de creación de Comercio.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// <para /> Si CodComercio y CodSucursal viene vacío, este es un campo obligatorio.
        /// </summary>
        [DataMember]
        public string IdMerchant { get; set; }
        
        /// <summary>
        /// <para /> Es el valor devuelto por VisaNetPagos en el proceso de asociación de Usuario.
        /// <para> El campo deberá ser de 36 caracteres.</para>
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>El campo deberá ser de 36 caracteres.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo IdUsuario es obligatorio.")]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "El campo IdUsuario excede el largo máximo permitido (36).")]
        public string IdUsuario { get; set; }

        /// <summary>
        /// <para /> Es el valor devuelto por VisaNetPagos en el proceso de asociación de Usuario que identifica una Tarjeta.
        /// <para> El campo deberá ser de 36 caracteres.</para>
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>El campo deberá ser de 36 caracteres.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo IdUsuario es obligatorio.")]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "El campo IdTarjeta excede el largo máximo permitido (36).")]
        public string IdTarjeta { get; set; }

        /// <summary>
        /// <para /> Número de factura asociada.
        /// <para> El campo no puede exceder el largo de 100 caracteres.</para>
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>El campo deberá ser de 100 caracteres.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo NroFactura es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo IdTarjeta excede el largo máximo permitido (100).")]
        public string NroFactura { get; set; }

        /// <summary>
        /// <para /> Descripción a mostrar asociada a la factura. 
        /// <para> El campo no puede exceder el largo de 200 caracteres.</para>
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo deberá ser de 200 caracteres.</value>
        [DataMember]
        [StringLength(200, ErrorMessage = "El campo Descripcion excede el largo máximo permitido (200).")]
        public string Descripcion { get; set; }

        /// <summary>
        /// <para /> Fecha de emisión de la factura. 
        /// <para /> Tipo de dato <see cref="DateTime"> DateTime</see>
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value> Fecha de emisión.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo FchFactura es obligatorio.")]
        public DateTime FchFactura { get; set; }

        /// <summary>
        /// <para /> Moneda de la factura.
        /// <para> El campo deberá ser "N" para moneda nacional (pesos uruguayos) o "D" para dólares americanos.</para>
        /// <para /> Tipo de dato <see cref="String"> String</see>
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>El campo deberá ser "N" para moneda nacional (pesos uruguayos) o "D" para dólares americanos.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo Moneda es obligatorio.")]
        [StringLength(1, MinimumLength = 1, ErrorMessage = "El campo Moneda excede el largo máximo permitido (1).")]
        [RegularExpression(@"[D|N|d|n]{1}$", ErrorMessage = "El campo Moneda solo puede ser 'N' o 'D'.")]
        public string Moneda { get; set; }

        /// <summary>
        /// <para /> Monto total de la factura. 
        /// <para> El campo deberá ser Decimal de 18,2. </para> 
        /// <para /> Tipo de dato <see cref="System.Decimal"> Decimal </see>
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>El campo deberá ser Decimal de 18,2. </value>
        [DataMember]
        [Required(ErrorMessage = "El campo MontoTotal es obligatorio.")]
        public double MontoTotal { get; set; }

        /// <summary>
        /// <para /> Monto gravado de IVA sobre el cual se aplicarán los descuentos correspondientes a la ley 19.210 (si corresponde). En caso que no se complete, se tomará como 0. 
        /// <para> El campo deberá ser Decimal de 18,2. </para> 
        /// <para /> Tipo de dato <see cref="System.Decimal"> Decimal </see>
        /// </summary>
        /// <value>El campo deberá ser Decimal de 18,2. </value>
        [DataMember]
        public double MontoGravado { get; set; }

        /// <summary>
        /// <para /> Indicador de Devolución de Impuestos. 
        /// <para /> El campo deberá ser : 0 – No Develve Impuestos. 1 – Devuelve impuestos acorde a la ley en el comercio configurada por VisaNet.
        /// <para /> Tipo de dato <see cref="short"> Short </see>
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>El campo deberá ser : 0 – No Develve Impuestos. 1 – Devuelve impuestos acorde a la ley en el comercio configurada por VisaNet.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo Indi es obligatorio.")]
        [Range(0, 99, ErrorMessage = "El campo Indi excede el largo máximo permitido (2).")]
        public int Indi { get; set; }

        /// <summary>
        /// <para /> Indicador sobre si es una factura a consumidor final o no 
        /// <para /> El campo deberá ser : "False" si no es consumidor final, "True" si lo es.
        /// <para /> Tipo de dato <see cref="bool"> bool</see>
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>El campo deberá ser : "False" si no es consumidor final, "True" si lo es.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo ConsFinal es obligatorio.")]
        public bool ConsFinal { get; set; }

        /// <summary>
        /// <para /> No aplica actualmente. En caso que no se complete, se tomará como 0. 
        /// <para /> Tipo de dato <see cref="short"> Short </see>
        /// </summary>
        /// <value>No aplica actualmente. </value>
        [DataMember]
        public int Cuotas { get; set; }
        
        /// <summary>
        /// <para /> Cybersource Device fingerprint generado por el comercio que invoca al web service. Leer el siguiente documento para entender sobre como se genera el device fingerprint. http://www.cybersource.com/content/dam/cybersource/Secure_Acceptance_WM_Quick_Start_Guide.pdf
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value> Cybersource Device fingerprint generado por el comercio que invoca al web service.</value>
        [DataMember]
        public string DeviceFingerprint { get; set; }

        /// <summary>
        /// <para /> Dirección IP del cliente que quiere relizar el checkout.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value> Dirección IP del cliente que quiere relizar el checkout.</value>
        [DataMember]
        public string IpCliente { get; set; }

        /// <summary>
        /// <para /> Teléfono del cliente que realiza la transacción.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>. Solo pueden ser characteres númerico.
        /// </summary>
        /// <value>Teléfono del cliente que realiza la transacción.</value>
        [DataMember]
        public string TelefonoCliente { get; set; }

        /// <summary>
        /// <para /> Dirección de envio ingresada por el cliente para el checkout.
        /// </summary>
        /// <value> Dirección de envio ingresada por el cliente para el checkout.</value>
        [DataMember]
        public CustomerShippingAddres DireccionEnvioCliente { get; set; }

        /// <summary>
        /// <para /> En coordinación con VisaNet, se podrían incluir campos auxiliares para funcionalidades específicas.
        /// </summary>
        /// <value> En coordinación con VisaNet, se podrían incluir campos auxiliares para funcionalidades específicas.</value>
        [DataMember]
        public ICollection<AuxiliarData> AuxiliarData { get; set; }
        
    }

    /// <summary>
    /// Objeto que contiene datos extras realcionados con la transacción.
    /// </summary>
    /// <value> Objeto que contiene datos extras realcionados con la transacción</value>
    [DataContract]
    public class AuxiliarData
    {
        /// <summary>
        /// Identificador de campo auxiliar
        /// <para /> Tipo de dato <see cref="String"> String </see>
        /// </summary>
        /// <value>Identificador de campo auxiliar</value>
        [DataMember]
        public string Id_auxiliar { get; set; }

        /// <summary>
        /// Datos del campo auxiliar
        /// <para /> Tipo de dato <see cref="String"> String </see>
        /// </summary>
        /// <value>Datos del campo auxiliar</value>
        [DataMember]
        public string Dato_auxiliar { get; set; }
    }

    /// <summary>
    /// Objeto que contiene los datos de la dirección de envio.
    /// </summary>
    /// <value> Objeto que contiene los datos de la dirección de envio </value>
    [DataContract]
    public class CustomerShippingAddres
    {
        /// <summary>
        /// <para /> Nombre de la calle.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>Nombre de la calle.</value>
        [DataMember]
        public string Calle { get; set; }

        /// <summary>
        /// <para /> Número de puerta.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>Número de puerta.</value>
        [DataMember]
        public string NumeroPuerta { get; set; }

        /// <summary>
        /// <para /> Otro detalle para encontrar la dirección. Caso número de apartamento, piso, etc.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>Otro detalle para encontrar la dirección. Caso número de apartamento, piso, etc</value>
        [DataMember]
        public string Complemento { get; set; }

        /// <summary>
        /// <para /> Nombre de la calle que corta la calle principal.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>Nombre de la calle que corta la calle principal.</value>
        [DataMember]
        public string Esquina { get; set; }

        /// <summary>
        /// <para /> Nombre del barrio.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>Nombre del barrio.</value>
        [DataMember]
        public string Barrio { get; set; }

        /// <summary>
        /// <para /> Código postal.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>Código postal.</value>
        [DataMember]
        public string CodigoPostal { get; set; }

        /// <summary>
        /// <para /> Longitud de la dirección.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>Longitud de la dirección.</value>
        [DataMember]
        public string Longitud { get; set; }

        /// <summary>
        /// <para /> Latitud de la dirección.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>Latitud de la dirección. </value>
        [DataMember]
        public string Latitud { get; set; }

        /// <summary>
        /// <para /> Teléfono de la dirección.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>. Solo pueden ser characteres númerico.
        /// </summary>
        /// <value>Teléfono de la dirección.</value>
        [DataMember]
        public string Telefono { get; set; }
        /// <summary>
        /// <para /> Ciudad de la dirección.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>. 
        /// </summary>
        /// <value>Ciudad de la dirección.</value>
        [DataMember]
        public string Ciudad { get; set; }
        /// <summary>
        /// <para /> País de la dirección. El pais tiene que respetar el ISO 3166 de dos caracteres (http://apps.cybersource.com/library/documentation/sbc/quickref/countries_alpha_list.pdf)
        /// <para /> Tipo de dato <see cref="System.String"> String </see>. 
        /// </summary>
        /// <value>País de la dirección.</value>
        [StringLength(2, MinimumLength = 2, ErrorMessage = "El campo Pais excede el largo máximo permitido (2).")]
        [DataMember]
        public string Pais { get; set; }
    }
}