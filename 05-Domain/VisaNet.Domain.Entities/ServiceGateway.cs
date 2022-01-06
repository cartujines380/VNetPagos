using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("ServiceGateway")]
    [TrackChanges]
    public class ServiceGateway : EntityBase
    {
        [Key]
        [TrackChangesAditionalInfo(Index = 0)]
        public override Guid Id { get; set; }

        public Guid GatewayId { get; set; }
        public virtual Gateway Gateway { get; set; }

        public bool SendExtract { get; set; }

        public bool Active { get; set; }
        [MaxLength(100)]
        public string ReferenceId { get; set; }
        [MaxLength(100)]
        public string ServiceType { get; set; }

        //PARA CARRETERA ReferenceId = COD COMERCIO. ServiceType = COD SUCURSAL
        //PARA IMPORTE ReferenceId = MONTO MINIMO PESOS. ServiceType = MONTO MAXIMO PESOS. AuxiliarData = MONTO MINIMO DOLARES. AuxiliarData2 = MONTO MAXIMO DOLARES
        //PARA LINKPAYMENT ReferenceId = SEGUNDOS DE VIDA DEL TOKEN

        public string AuxiliarData { get; set; }
        public string AuxiliarData2 { get; set; }
    }
}
