using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities.ExternalRequest
{
    [Table("WebhookDowns")]
    public class WebhookDown : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public string IdApp { get; set; }
        public string IdUser { get; set; }
        public string IdCard { get; set; }
        public string IdOperation { get; set; }

        public string HttpResponseCode { get; set; }

        public string CreationUser { get; set; }
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}