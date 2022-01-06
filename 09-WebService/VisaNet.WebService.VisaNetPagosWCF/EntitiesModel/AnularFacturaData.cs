﻿using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaNetPagosWCF.EntitiesModel
{
    /// <summary>
    /// Objeto con los datos a utilizar para anular una transacción
    /// </summary>
    [DataContract]
    public class AnularFacturaData
    {
        /// <summary>
        /// <para /> Indicador que identifica la operación del lado de la APP.
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>Indicador que identifica la operación del lado de la APP.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo IdOperacion es obligatorio.")]
        public string IdOperacion { get; set; }

        /// <summary>
        /// <para /> Código asignado por VisaNet al momento del alta de una nueva App.
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>Código asignado por VisaNet al momento del alta de una nueva App.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo IdApp es obligatorio.")]
        public string IdApp { get; set; }

        /// <summary>
        /// <para /> Indicador de la operación enviada por la App mediante CobroFacturaOnlineApp que se desea anular.
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>Indicador de la operación enviada por la App mediante CobroFacturaOnlineApp que se desea anular.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo IdOperacionCobro es obligatorio.")]
        public string IdOperacionCobro { get; set; }

        /// <summary>
        /// <para />Firma Digital, la cual se realiza haciendo un hash de todos los parámetros de entrada.
        /// <para />El orden de los parametros es el siguiente: IdApp, IdOperacion, IdOperacionCobro
        /// <para /> Tipo de dato <see cref="System.String"> String</see>
        /// <para /> Este es un campo obligatorio.
        /// </summary>
        /// <value>Firma Digital, la cual se realiza haciendo un hash de todos los parámetros de entrada.</value>
        [DataMember]
        [Required(ErrorMessage = "El campo FirmaDigital es obligatorio.")]
        public string FirmaDigital { get; set; }
    }
}