using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    /// <summary>
    /// Resultado de la opreación consulta de comercios
    /// </summary>
    [DataContract]
    public class VNPRespuestaConsultaComercios
    {
        /// <summary>
        /// <para /> Valor que indica el resultado de la operación. 
        /// 0 indica que la transacción de pago de la factura se completó exitosamente, y cualquier valor distinto a 0 indica un error. 
        /// En caso de ocurrir un error se anulará toda la transacción. 
        /// <para /> Tipo de dato <see cref="short"> Short</see>
        /// </summary>
        /// <value>Valor que indica el resultado de la operación. </value>
        [DataMember]
        public int CodResultado { get; set; }

        /// <summary>
        /// <para /> Descripción indicando el resultado de la operación ejecutada
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// </summary>
        /// <value>Descripción indicando el resultado de la operación ejecutada.</value>
        [DataMember]
        public string DescResultado { get; set; }

        /// <summary>
        /// <para /> Identificador de la operación ejecutada. Este valor es el mismo que se recibió como parametro de entrada
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// </summary>
        /// <value>Identificador de la operación ejecutada. Este valor es el mismo que se recibió como parametro de entrada.</value>
        [DataMember]
        public string IdOperacion { get; set; }
        
        /// <summary>
        /// Listado de comercios
        /// </summary>
        /// <value>Listado de comercios en VisaNet.</value>
        [DataMember]
        public ICollection<Comercio> Comercios { get; set; }

    }

    /// <summary>
    /// Objeto comercio y sus datos
    /// </summary>
    [DataContract]
    public class Comercio
    {
        /// <summary>
        /// <para /> Nombre asignado al comercio por VisaNet
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// </summary>
        /// <value>Nombre asignado al comercio por VisaNet.</value>
        [DataMember]
        public string Nombre { get; set; }

        /// <summary>
        /// <para /> Código del comercio que asigna VisaNet al momento del alta de una nueva Institución. 
        /// <para /> El campo no puede exceder el largo de 7 caracteres.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo no puede exceder el largo de 7 caracteres</value>
        [DataMember]
        public string CodComercio { get; set; }

        /// <summary>
        /// <para /> Código de Sucursal que asigna VisaNet al momento del alta de una nueva Institución.
        /// <para /> El campo no puede exceder el largo de 3 caracteres.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        /// <value>El campo no puede exceder el largo de 3 caracteres</value>
        [DataMember]
        public string CodSucursal { get; set; }

        /// <summary>
        /// <para /> Es el valor asignado por VisaNet en el proceso de creación de Comercio.
        /// <para /> Tipo de dato <see cref="System.String"> String </see>.
        /// </summary>
        [DataMember]
        public string IdMerchant { get; set; }

        /// <summary>
        /// <para /> Estado del comercio en VisaNet
        /// <para /> Tipo de dato <see cref="bool"> bool</see>
        /// </summary>
        /// <value>Estado del comercio en VisaNet.</value>
        [DataMember]
        public bool Active { get; set; }
        
    }
}